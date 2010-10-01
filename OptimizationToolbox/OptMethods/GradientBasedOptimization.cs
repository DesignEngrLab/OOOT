﻿using System;

using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class GradientBasedOptimization : abstractOptMethod
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

        #region Constructor
        public GradientBasedOptimization()
        {
            this.ConstraintsSolvedWithPenalties = true;
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

            do
            {
                gradF = calc_f_gradient(xk);
                dk = this.searchDirMethod.find(xk, gradF, fk);

                // use line search (arithmetic mean) to find alphaStar
                alphaStar = lineSearchMethod.findAlphaStar(xk, dk);
                xk = StarMath.add(xk, StarMath.multiply(alphaStar, dk));
                SearchIO.output("iteration=" + k, 3);
                k++;
                fk = calc_f(xk);
                if (fk < fStar)
                {
                    fStar = fk;
                    xStar = (double[])xk.Clone();
                }
                SearchIO.output("f = " + fk, 3);
            }
            while (!convergeMethod.converged(k, fk, xk, gradF));

            return fStar;
        }


        #endregion


    }
}