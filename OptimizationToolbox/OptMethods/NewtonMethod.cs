// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="NewtonMethod.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT X11 License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT X11 License for more details.
 *  


 *     
 *     Please find further details and contact information on OOOT
 *     at http://designengrlab.github.io/OOOT/.
 *************************************************************************/

using System;
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class NewtonMethod.
    /// Implements the <see cref="OptimizationToolbox.abstractOptMethod" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractOptMethod" />
    public class NewtonMethod : abstractOptMethod
    {
        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */
        /// <summary>
        /// The alpha star
        /// </summary>
        private double alphaStar;
        /// <summary>
        /// The dk
        /// </summary>
        private double[] dk;

        /* fk is the value of f(xk). */
        /// <summary>
        /// The fk
        /// </summary>
        private double fk;
        /// <summary>
        /// The grad f
        /// </summary>
        private double[] gradF;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonMethod"/> class.
        /// </summary>
        public NewtonMethod()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = false;
        }

        #endregion

        #region Main Function, run

        /// <summary>
        /// Runs the specified x star.
        /// </summary>
        /// <param name="xStar">The x star.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="Exception">Newton's method requires that the objective function be twice differentiable"
        ///                 + "\n(Must inherit from ITwiceDifferentiable).</exception>
        protected override double run(out double[] xStar)
        {
            if (!(f[0] is ITwiceDifferentiable))
                throw new Exception("Newton's method requires that the objective function be twice differentiable"
                + "\n(Must inherit from ITwiceDifferentiable).");
            /* this is just to overcome a small issue with the compiler. It thinks that xStar will
             * not have a value since it only appears in a conditional statement below. This initi-
             * alization is to "ensure" that it does and that the code compiles. */
            xStar = (double[])x.Clone();
            //evaluate f(x0)
            fStar = fk = calc_f(x);
            do
            {
                gradF = calc_f_gradient(x);
                var Hessian = new double[n, n];
                for (int i = 0; i < n; i++)
                    for (int j = i; j < n; j++)
                        Hessian[i, j] = Hessian[j, i] = ((ITwiceDifferentiable)f[0]).second_deriv_wrt_ij(x, i, j);

                dk = StarMath.multiply(-1, Hessian.inverse().multiply(gradF));
                if (double.IsNaN(dk.SumAllElements()))
                    dk = StarMath.multiply(-1, gradF);
                var step = dk.norm2(); 
                if (step == 0) continue;
                dk = dk.divide(step);
                // use line search (arithmetic mean) to find alphaStar
                alphaStar = lineSearchMethod.findAlphaStar(x, dk, step);
                x = x.add(StarMath.multiply(alphaStar, dk));
                SearchIO.output("iteration=" + k, 3);
                k++;
                fk = calc_f(x);
                if (fk < fStar)
                {
                    fStar = fk;
                    xStar = (double[])x.Clone();
                }
                SearchIO.output("f = " + fk, 3);
            } while (notConverged(k, numEvals, fk, x, null, gradF));

            return fStar;
        }

        #endregion

        /* alphaStar is what is returned by the line search (1-D) search method. It is used
         * to update xk. */
    }
}