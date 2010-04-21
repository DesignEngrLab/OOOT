using System;

using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class GeneralizedReducedGradientSlack : abstractOptMethod
    {
        List<int> xcIndices;
        List<int> xdIndices;

        double[] xk, xkLast, gradF, dk;
        double fk, fStar, alphaStar;

        double[,] gradH, gradHWRT_xc, gradHWRT_xd;
        double[,] invGradHWRT_xc;

        //inner loop counters and limits
        int xcAttempts;
        double iL_epsilon;

        #region Constructor
        public GeneralizedReducedGradientSlack()
        {
            this.InequalitiesConvertedToEqualities = true;
            this.iL_epsilon = this.epsilon = 0.0000001;
            feasibleOuterLoopMax = 5;
            feasibleInnerLoopMax = 500;
        }
        #endregion

        #region Xd and Xc properties
        public double[] xc
        {
            get
            {
                double[] vars = new double[xcIndices.Count];
                for (int i = 0; i < xcIndices.Count; i++)
                    vars[i] = xk[xcIndices[i]];
                return vars;
            }
            set
            {
                if ((xcIndices != null) && (xcIndices.Count != 0) &&
                    (xcIndices.Count == value.GetLength(0)))
                    for (int i = 0; i != xcIndices.Count; i++)
                        xk[xcIndices[i]] = value[i];
            }
        }

        // these properties will use the x*Indices
        public double[] xd
        {
            get
            {
                double[] vars = new double[xdIndices.Count];
                for (int i = 0; i < xdIndices.Count; i++)
                    vars[i] = xk[xdIndices[i]];
                return vars;
            }
            set
            {
                if ((xdIndices != null) && (xdIndices.Count != 0) &&
                    (xdIndices.Count == value.GetLength(0)))
                    for (int i = 0; i != xdIndices.Count; i++)
                        xk[xdIndices[i]] = value[i];
            }
        }
        #endregion

        #region Main Function, run
        public override double run(double[] x0, out double[] xStar)
        {
            throw new NotImplementedException();
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
            dk = new double[n];
            // k = 0 --> iteration counter
            k = 0;
            #endregion

            /* this is the iteration counter for updating Xc it's compared with feasibleOuterLoopMax. */
            int outerFeasibleK;
            foreach (equality c in h)
                active.Add(c);
            divideXintoDecisionAndDependentVars();

            do
            {
                gradH = calc_h_gradient(xk);
                divideGradH_intoXcAndXdParts();
                invGradHWRT_xc = StarMath.inverseUpper(gradHWRT_xc);
                gradF = objfn.gradient(xk);
                calculateReducedGradientSearchDirection();

                // use line search (arithmetic mean) to find alphaStar
                lineSearchMethod.lastFeasAlpha4G = 0.0;
                lineSearchMethod.findAlphaStar(xk, dk);
                alphaStar = lineSearchMethod.lastFeasAlpha4G;
                //alphaStar = lineSearchMethod.findAlphaStar(xk, dk, g);

                xkLast = xk;
                xk = StarMath.add(xkLast, StarMath.multiply(alphaStar, dk));
                outerFeasibleK = 0;
                while (!updateXc() && (++outerFeasibleK == feasibleOuterLoopMax))
                {
                    alphaStar /= 2;
                    xk = StarMath.add(xkLast, StarMath.multiply(alphaStar, dk));
                }
                k++;
                fk = objfn.calculate(xk);
                SearchIO.output("X = " + xk[0] + ", " + xk[1], 3);// + ", " + xk[2]
                SearchIO.output("F(" + k.ToString() + ") = " + fk.ToString(), 3);
            }
            while (!convergeMethod.converged(k, xk, fk, gradF));
            fStar = fk;
            xStar = xk;
            SearchIO.output("X* = " + xk[0] + ", " + xk[1], 2);
            SearchIO.output("F* = " + fk.ToString(), 2);
            return fStar;
        }
        #endregion

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
                if (i < q) maxCol = n - i - 1;
                else
                {
                    maxForRow = 0.0;
                    maxCol = -1;
                    for (int j = 0; j < n; j++)
                        if (Math.Abs(this.gradH[i, j]) > maxForRow)
                        {
                            maxForRow = Math.Abs(gradH[i, j]);
                            maxCol = j;
                        }
                }
                xcIndices.Add(maxCol);
                // if outerK equal m-1 continue!
                for (int ii = i + 1; ii < m; ii++)
                {
                    // first we find B coefficent to multiply all elements of row outerK with
                    coeff = gradH[ii, maxCol] / gradH[i, maxCol];
                    for (int j = 0; j < n; j++)
                        gradH[ii, j] -= coeff * gradH[i, j];
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

        private void divideGradH_intoXcAndXdParts()
        {
            gradHWRT_xc = new double[m, m];

            for (int i = 0; i < m; i++)
                for (int j = 0; j < m; j++)
                    gradHWRT_xc[j, i] = gradH[j, xcIndices[i]];

            gradHWRT_xd = new double[m, n - m];
            for (int i = 0; i < n - m; i++)
                for (int j = 0; j < m; j++)
                    gradHWRT_xd[j, i] = gradH[j, xdIndices[i]];
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
            StarMath.multiply(invGradHWRT_xc, gradHWRT_xd)));
            double[] dir_Xc =
                StarMath.multiply(-1.0,
                StarMath.multiply(invGradHWRT_xc,
                StarMath.multiply(gradHWRT_xd, dir_Xd)));
            for (int i = 0; i < n - m; i++)
                dk[xdIndices[i]] = dir_Xd[i];
            for (int i = 0; i < m; i++)
                dk[xcIndices[i]] = dir_Xc[i];

            dk = searchDirMethod.find(xk, dk, fk);

        }

        private bool updateXc()
        {
            int innerFeasibleK = 0;
            double[] xcOld;
            double[] dir_Xd = new double[n - m];

            for (int i = 0; i < n - m; i++)
                dir_Xd[i] = dk[xdIndices[i]];

            xcOld = xc;
            xc = StarMath.subtract(xcOld, StarMath.multiply(invGradHWRT_xc, calc_h_vector(xk)));
            while (StarMath.norm1(xc, xcOld) / StarMath.norm1(xc) > iL_epsilon)
            {
                gradHWRT_xc = calc_h_gradient(xk, xcIndices);
                invGradHWRT_xc = StarMath.inverse(gradHWRT_xc);
                if (++innerFeasibleK == feasibleInnerLoopMax) return false;
                xcOld = xc;
                xc = StarMath.subtract(xcOld, StarMath.multiply(invGradHWRT_xc, calc_h_vector(xk)));
                foreach (int i in this.xcIndices)
                    if ((i >= n - q) && (xk[i] < 0))
                        /*then it's a slack variable that's gone negative and should simply be set to eps*/
                        xk[i] = this.epsilon;
            }
            return true;
        }
    }
}
