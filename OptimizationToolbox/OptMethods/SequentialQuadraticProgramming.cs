using System;

using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// 
    /// </summary>
    public class SequentialQuadraticProgramming : abstractOptMethod
    {
        #region Fields
        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */
        double[] xk, gradF, dk;

        /* fk is the value of f(xk). */
        double fk;

        /* alphaStar is what is returned by the line search (1-D) search method. It is used
         * to update xk. */
        double alphaStar;

        /* A is the gradient of the active constraints as an m by n matrix. */
        double[,] A;
        double[] activeVals;

        /* Lagrange multipliers for active constraints. */
        double[] lambdas;
        #endregion

        #region Constructor
        public SequentialQuadraticProgramming() : this(true) { }
        public SequentialQuadraticProgramming(Boolean defaultMethod)
            : this(0.0001, defaultMethod) { }
        public SequentialQuadraticProgramming(double eps, Boolean defaultMethod)
        {
            this.SearchDirectionMethodNeeded = false;
            this.epsilon = eps;
            if (defaultMethod) setUpDefaultMethods();
        }
        private void setUpDefaultMethods()
        {
            this.Add(new SQPSimpleHalver(this, 0.25, 100));
            this.Add(new linearExteriorPenaltyMax(this, 1.0));
            this.Add(new MultipleANDConvergenceConditions(5, 0.01, 0.01));
        }
        #endregion

        #region Main Function, run
        /// <summary>
        /// Runs from the specified x0.
        /// </summary>
        /// <param name="x0">The initial candidate, x0.</param>
        /// <param name="xStar">The x-star candidate is returned as an out var.</param>
        /// <returns>the value of the objective function at xStar.</returns>
        public override double run(double[] x0, out double[] xStar)
        {
            #region Initialization
            /* Initialize xStar so that something can be returned if the search crashes. */
            if (x0 != null) xStar = (double[])x0.Clone();
            else xStar = new double[0];
            /* initialize and check is part of the abstract class. GRG requires a feasible start point
             * so if none is found, we return infinity.*/
            if (!initializeAndCheck(ref x0)) return fStar;
            xk = (double[])x0.Clone();
            //evaluate f(x0)
            fStar = fk = calc_f(xk);

            // the search direction is initialized.
            dk = new double[n];

            // k = 0 --> iteration counter
            k = 0;
            double initAlpha = 0.0;
            active.AddRange(h);
            #endregion


            do
            {
                gradF = calc_f_gradient(xk);
                A = formulateActiveSetAndGradients(xk);
                dk = calculateSQPSearchDirection(xk, gradF, A, out initAlpha);
                meritFunction.penaltyWeight = adjustMeritPenalty();
                // this next function is not part of the regular SQP algorithm
                // it's only intended to keep all the points in the positive space.
                initAlpha = preventNegatives(xk, dk, initAlpha);
                //

                alphaStar = lineSearchMethod.findAlphaStar(xk, dk, initAlpha);
                xk = StarMath.add(xk, StarMath.multiply(alphaStar, dk));
                SearchIO.output("iteration=" + k, 3);
                SearchIO.output("--alpha=" + alphaStar, 3);
                k++;
                fk = calc_f(xk);

                SearchIO.output("----f = " + fk, 3);
                SearchIO.output("---#active =" + active.Count, 3);
            }
            while (!convergeMethod.converged(k, fk, xk, gradF));
            fStar = fk;
            xStar = (double[])xk.Clone();
            return fStar;
        }

        private double adjustMeritPenalty()
        {
            double temp = 0.0;
            foreach (double a in lambdas)
                temp += Math.Abs(a);
            return temp;
            if (temp < meritFunction.penaltyWeight)
                return 2 * meritFunction.penaltyWeight;
            else return temp;
        }

        private double preventNegatives(double[] xk, double[] dk, double initAlpha)
        {
            for (int i = 0; i < dk.GetLength(0); i++)
                if (dk[i] < 0)
                    initAlpha = Math.Min(initAlpha, -xk[i] / dk[i]);
            return initAlpha;
        }
        #endregion

        /// <summary>
        /// Formulates the active set and gradients.
        /// </summary>
        /// <param name="xk">The xk.</param>
        /// <returns>the gradient of the active constraints as an m by n matrix.</returns>
        private double[,] formulateActiveSetAndGradients(double[] xk)
        {
            /* this list is ordered from largest to smallest. We only want to introduce ONE new member 
             * into the active set. Which would be the first element (i.e.value) of this sorted list. */

            SortedList<double, constraint> newInfeasibles
                = new SortedList<double, constraint>(new optimizeSort(optimize.maximize));

            /* this foreach loop can remove any number of g's from the active set, and puts
             * any newly violated g's into the sorted list newInfeasbles. */
            foreach (inequality c in g)
            {
                double gVal = c.calculate(xk);
                if (gVal < 0.0)
                {
                    if (active.Contains(c)) active.Remove(c);
                }
                else
                    newInfeasibles.Add(gVal, c);
            }
            /* only want to add one newInfeasible to the list of active. Hence the break statement. */
            foreach (KeyValuePair<double, constraint> kvp in newInfeasibles)
                if (!active.Contains(kvp.Value))
                {
                    active.Add(kvp.Value);
                    break;
                }
            m = active.Count;
            return calc_active_gradient(xk);
        }

        /// <summary>
        /// Calculates the SQP search direction.
        /// </summary>
        /// <param name="xk">The xk.</param>
        /// <param name="gradF">The grad F.</param>
        /// <param name="A">The A.</param>
        /// <param name="initAlpha">The init alpha.</param>
        /// <returns></returns>
        private double[] calculateSQPSearchDirection(double[] xk, double[] gradF, double[,] A,
            out double initAlpha)
        {

            SearchIO.output("xk = [" + xk[0] + ", " + xk[1] + "]", 6);
            activeVals = calc_active_vector(xk);
            double[] dir = null;
            double[,] At = StarMath.transpose(A);
            double[,] invAAt = StarMath.inverse(StarMath.multiply(A, At));
            double[,] AtinvAAt = StarMath.multiply(At, invAAt);
            double[,] J = StarMath.makeIdentity(n);
            double[,] invJ = StarMath.makeIdentity(n);

            // double[,] P = StarMath.subtract(StarMath.makeIdentity(n), AtinvAAtA);
            if (this.searchDirMethod != null)
            {

            }
            double[,] P = StarMath.multiply(invJ,
                StarMath.subtract(StarMath.multiply(AtinvAAt, A), StarMath.makeIdentity(n)));
            //double[,] Q = StarMath.multiply(invJ,
            //        StarMath.multiply(AtinvAAt, StarMath.multiply(A,
            //        StarMath.multiply(J, StarMath.multiply(invAtA, At)))));
            double[,] Q = AtinvAAt;
            double[] dirMin = StarMath.multiply(P, gradF);
            double[] dirCC = StarMath.multiply(Q, activeVals);
            dir = StarMath.subtract(dirMin, dirCC);

            lambdas = StarMath.multiply(invAAt, StarMath.subtract(
                activeVals, StarMath.multiply(A, gradF)));

            if (double.IsNaN(dir[0])) dir = dk;
            initAlpha = StarMath.norm2(dir);
            if (initAlpha > 0) dir = StarMath.multiply((1.0 / initAlpha), dir);
            return dir;
        }
    }
}