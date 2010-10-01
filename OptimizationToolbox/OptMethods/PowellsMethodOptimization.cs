using System;

using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class PowellsMethodOptimization : abstractOptMethod
    {
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

        double[,] searchDirMatrix;
        
        #region Constructor
        public PowellsMethodOptimization(double penaltyWeight)
        {
            this.ConstraintsSolvedWithPenalties = true;
            this.penaltyWeight = penaltyWeight;
            this.searchDirMethod = new PowellsDirection();
            this.LineSearchMethodNeeded = true;
            this.searchDirMatrix = StarMath.makeIdentity(n);
            this.k = 0;
        }
        #endregion


        #region Main Function, run
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
            dk = new double[n];
            // k = 0 --> iteration counter
            k = 0;
            #endregion
            dk = this.searchDirMethod.find(xk, null, double.NaN, true);

            do
            {
                dk = this.searchDirMethod.find(xk, null, double.NaN, false);
                // use line search (arithmetic mean) to find alphaStar
                alphaStar = lineSearchMethod.findAlphaStar(xk, dk);
                xk = StarMath.add(xk, StarMath.multiply(alphaStar, dk));
                SearchIO.output("iteration=" + k,3);
                k++; 
                fk = calc_f(xk);
                if (fk < fStar)
                {
                    fStar = fk;
                    xStar = (double[])xk.Clone();
                }
                SearchIO.output("f = " + fk, 3);
            }
            while (!convergeMethod.converged(k, xk, fk, gradF));

            return fStar;
        }


        #endregion


    }
}
