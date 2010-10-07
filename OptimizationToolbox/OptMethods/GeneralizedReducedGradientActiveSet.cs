using System;

using System.Collections.Generic;
using System.Collections;
using StarMathLib;

namespace OptimizationToolbox
{
    public class GeneralizedReducedGradientActiveSet : abstractOptMethod
    {
        #region Fields
        /* the following lists indicate what elements of x are decision variable,
         * Xd and which are constrained or dependent variables, Xc. These are created
         * in the function "divideXintoDecisionAndDependentVars", and can be accessed
         * and changed by the properties: xc and xd. */
        List<int> xcIndices;
        List<int> xdIndices;

        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */
        double[] xkLast, gradF, dk;

        /* fk is the value of f(xk). */
        double fk;

        /* alphaStar is what is returned by the line search (1-D) search method. It is used
         * to update xk. */
        double alphaStar;

        /* gradA is the gradient of the active constraints as an m by n matrix. It is then
         * divided into two subsets: the gradient with respect to (wrt) xc which would be a
         * square m-by-m matrix, and the grade w.r.t. xd which is m-by-n-m. The inverse of the
         * former is required to solve for xc. */
        double[,] gradA, gradA_wrt_xc, gradA_wrt_xd;
        double[,] invGradA_wrt_xc;

        /* divideX is used to schedule when x should be divided into Xc and Xd. This needs to 
         * happen at the very beginning but also whenever the active set changes from the line
         * search. This is also used to inform the search direction that it should be reset. */
        Boolean divideX;
        /* The following is the inner epsilon for updating Xc. */
        double iL_epsilon;
        #endregion

        #region Constructor
        public GeneralizedReducedGradientActiveSet() : this(true) { }
        public GeneralizedReducedGradientActiveSet(Boolean defaultMethod)
            : this(0.0001, 0.0001, 500, 5, defaultMethod) { }
        public GeneralizedReducedGradientActiveSet(double eps, double ILeps, int innerMax, int outerMax, Boolean defaultMethod)
        {
            this.epsilon = eps;
            this.iL_epsilon = ILeps;
            feasibleInnerLoopMax = innerMax;
            feasibleOuterLoopMax = outerMax;
            if (defaultMethod) setUpDefaultMethods();
        }
        private void setUpDefaultMethods()
        {
            this.Add(new ArithmeticMean(epsilon, 1, 100));
            this.lineSearchMethod.SetOptimizationDetails(this);
            this.Add(new FletcherReevesDirection());
            this.Add(new MultipleANDConvergenceConditions(5, 0.01, 0.01));
        }
        #endregion

        #region Xd and Xc properties
        /* these are completely dependent on the list of indices for Xc and Xd.
         * They do not change what variables are in each, but they can be used
         * to access (get) and change (set) the values of Xc and Xd. */
        public double[] xc
        {
            get
            {
                double[] vars = new double[xcIndices.Count];
                for (int i = 0; i < xcIndices.Count; i++)
                    vars[i] = x[xcIndices[i]];
                return vars;
            }
            set
            {
                if ((xcIndices != null) && (xcIndices.Count != 0) &&
                    (xcIndices.Count == value.GetLength(0)))
                    for (int i = 0; i != xcIndices.Count; i++)
                        x[xcIndices[i]] = value[i];
            }
        }

        // these properties will use the x*Indices
        public double[] xd
        {
            get
            {
                double[] vars = new double[xdIndices.Count];
                for (int i = 0; i < xdIndices.Count; i++)
                    vars[i] = x[xdIndices[i]];
                return vars;
            }
            set
            {
                if ((xdIndices != null) && (xdIndices.Count != 0) &&
                    (xdIndices.Count == value.GetLength(0)))
                    for (int i = 0; i != xdIndices.Count; i++)
                        x[xdIndices[i]] = value[i];
            }
        }
        #endregion

        #region Main Function, run
        protected override double run(out double[] xStar)
        {
            //evaluate f(x0)
            fStar = fk = calc_f(x);
            dk = new double[n];

            /* this is the iteration counter for updating Xc it's compared with feasibleOuterLoopMax. */
            int outerFeasibleK;
            foreach (equality c in h)
                active.Add(c);
            /* this forces formulateActiveSetAndGradients to do the division. */
            divideX = true;

            do
            {
                gradF = calc_f_gradient(x);
                formulateActiveSetAndGradients(x, divideX);
                calculateReducedGradientSearchDirection();

                /* whether or not the previous two lines of code reformulated Xc and Xd, we now
                 * set it to false so that it won't do so next iteration. However, there are two
                 * places in which this might change. First, if the update for Xc requires a lot of
                 * effort and the alphaStar needs to be reduced; and Second, in formulateActive... if
                 * there are new violated constraints or an old constraint is no longer infeasible. */
                divideX = false;

                /* use line search (e.g. arithmetic mean) to find alphaStar */
                alphaStar = lineSearchMethod.findAlphaStar(x, dk);

                xkLast = x;
                x = StarMath.add(xkLast, StarMath.multiply(alphaStar, dk));
                outerFeasibleK = 0;
                while (!updateXc() && (++outerFeasibleK <= feasibleOuterLoopMax))
                {
                    alphaStar /= 2;
                    x = StarMath.add(xkLast, StarMath.multiply(alphaStar, dk));
                    divideX = true;
                }
                k++;
                fk = calc_f(x);

                SearchIO.output("x(" + k.ToString() + ") = " + StarMath.MakePrintString(x) + " = " + fk.ToString("0.00"), 4);
            }
            while (notConverged(k, fk, x, gradF));

            fStar = fk;
            xStar = x;
            return fStar;
        }
        #endregion

        private void formulateActiveSetAndGradients(double[] xk, Boolean divideBool)
        {
            /* this list is ordered from largest to smallest. We only want to introduce ONE new member 
             * into the active set. Which would be the first element (j.e.value) of this sorted list. */
            SortedList<double, constraint> newInfeasibles
                = new SortedList<double, constraint>(new optimizeSort(optimize.maximize));

            divideX = divideBool;

            /* this foreach loop can remove any number of g's from the active set, and puts
             * any newly violated g's into the sorted list newInfeasbles. */
            foreach (inequality c in g)
            {
                double gVal = c.calculate(xk);
                if (gVal < 0.0)
                {
                    if (active.Contains(c))
                    {
                        active.Remove(c);
                        divideX = true;
                    }
                }
                else newInfeasibles.Add(gVal, c);
            }
            /* only want to add one newInfeasible to the list of active. Hence the break statement. */
            foreach (KeyValuePair<double, constraint> kvp in newInfeasibles)
                if (!active.Contains(kvp.Value))
                {
                    active.Add(kvp.Value);
                    divideX = true;
                    break;
                }
            if (divideX)
            {
                m = active.Count;
                gradA = calc_active_gradient(xk);
                divideX = divideXintoDecisionAndDependentVars();
                if (divideX)
                {
                    divideGradA_intoXcAndXdParts();
                    invGradA_wrt_xc = StarMath.inverseUpper(gradA_wrt_xc);
                }
            }
        }

        private Boolean divideXintoDecisionAndDependentVars()
        {
            // divide x into xc and xd using automated Guassian eliminiation approach
            Boolean differentFromBefore = false;
            double maxForRow;
            int maxCol;
            double coeff;
            List<int> xcOldices = xcIndices;
            xcIndices = new List<int>(m);
            xdIndices = new List<int>(n - m);
            for (int i = 0; i < m; i++)
            {
                //if (j < q) maxCol = n - j - 1;
                //else
                //{
                maxForRow = 0.0;
                maxCol = -1;
                for (int j = 0; j < n; j++)
                    if (Math.Abs(this.gradA[i, j]) > maxForRow)
                    {
                        maxForRow = Math.Abs(gradA[i, j]);
                        maxCol = j;
                    }
                //}
                xcIndices.Add(maxCol);
                // if outerK equal m-1 continue!
                for (int ii = i + 1; ii < m; ii++)
                {
                    // first we find B coefficent to multiply all elements of row outerK with
                    coeff = gradA[ii, maxCol] / gradA[i, maxCol];
                    for (int j = 0; j < n; j++)
                        gradA[ii, j] -= coeff * gradA[i, j];
                }
            }
            for (int i = 0; i < n; i++)
                if (!xcIndices.Contains(i))
                    xdIndices.Add(i);
            if ((xcOldices == null) || (xcOldices.Count != xcIndices.Count)) differentFromBefore = true;
            else foreach (int i in xcOldices)
                    if (!xcIndices.Contains(i)) differentFromBefore = true;
            return differentFromBefore;
        }

        private void divideGradA_intoXcAndXdParts()
        {
            gradA_wrt_xc = new double[m, m];

            for (int i = 0; i < m; i++)
                for (int j = 0; j < m; j++)
                    gradA_wrt_xc[j, i] = gradA[j, xcIndices[i]];

            gradA_wrt_xd = new double[m, n - m];
            for (int i = 0; i < n - m; i++)
                for (int j = 0; j < m; j++)
                    gradA_wrt_xd[j, i] = gradA[j, xdIndices[i]];
        }

        private void calculateReducedGradientSearchDirection()
        {
            double[] gradFXc = new double[m];
            double[] gradFXd = new double[n - m];
            for (int i = 0; i < n - m; i++)
                gradFXd[i] = gradF[xdIndices[i]];
            for (int i = 0; i < m; i++)
                gradFXc[i] = gradF[xcIndices[i]];
            //how to handle the problem when slack goes to zero/epsilon and the invgrad then goes to infinity
            double[] dir_Xd =
            StarMath.subtract(gradFXd,
            StarMath.multiply(gradFXc,
            StarMath.multiply(invGradA_wrt_xc, gradA_wrt_xd)));
            double[] dir_Xc =
                StarMath.multiply(-1.0,
                StarMath.multiply(invGradA_wrt_xc,
                StarMath.multiply(gradA_wrt_xd, dir_Xd)));
            for (int i = 0; i < n - m; i++)
                dk[xdIndices[i]] = dir_Xd[i];
            for (int i = 0; i < m; i++)
                dk[xcIndices[i]] = dir_Xc[i];

            dk = searchDirMethod.find(x, dk, fk, divideX);

        }

        private bool updateXc()
        {
            int innerFeasibleK = 0;
            double[] xcOld;
            double[] dir_Xd = new double[n - m];

            for (int i = 0; i < n - m; i++)
                dir_Xd[i] = dk[xdIndices[i]];

            xcOld = xc;
            xc = StarMath.subtract(xcOld, StarMath.multiply(invGradA_wrt_xc, calc_active_vector(x)));
            while (StarMath.norm1(xc, xcOld) / StarMath.norm1(xc) > iL_epsilon)
            {
                gradA_wrt_xc = calc_active_gradient(x, xcIndices);
                invGradA_wrt_xc = StarMath.inverse(gradA_wrt_xc);
                if (++innerFeasibleK == feasibleInnerLoopMax) return false;
                xcOld = xc;
                xc = StarMath.subtract(xcOld, StarMath.multiply(invGradA_wrt_xc, calc_active_vector(x)));
            }
            return true;
        }


    }
}
