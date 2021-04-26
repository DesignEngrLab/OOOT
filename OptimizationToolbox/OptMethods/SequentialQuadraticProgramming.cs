// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="SequentialQuadraticProgramming.cs" company="OptimizationToolbox">
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
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class SequentialQuadraticProgramming. This class cannot be inherited.
    /// Implements the <see cref="OptimizationToolbox.abstractOptMethod" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractOptMethod" />
    public sealed class SequentialQuadraticProgramming : abstractOptMethod
    {
        #region Fields

        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */

        /* A is the gradient of the active constraints as an m by n matrix. */
        /// <summary>
        /// a
        /// </summary>
        private double[,] A;
        /// <summary>
        /// The active vals
        /// </summary>
        private double[] activeVals;
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

        /* Lagrange multipliers for active constraints. */
        /// <summary>
        /// The lambdas
        /// </summary>
        private double[] lambdas;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialQuadraticProgramming"/> class.
        /// </summary>
        public SequentialQuadraticProgramming()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = false;
            RequiresMeritFunction = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = true;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = false;

            Add(new SQPSimpleHalver(0.25, 100));
            lineSearchMethod.SetOptimizationDetails(this);
            Add(new linearExteriorPenaltyMax(this, 1.0));
            Add(new DeltaFConvergence(0.001, 0.0000001));
        }

        #endregion

        #region Main Function, run

        /// <summary>
        /// Runs the specified optimization method. This includes the details
        /// of the optimization method.
        /// </summary>
        /// <param name="xStar">The x star.</param>
        /// <returns>System.Double.</returns>
        protected override double run(out double[] xStar)
        {
            //evaluate f(x0)
            fStar = fk = calc_f(x);

            // the search direction is initialized.
            dk = new double[n];

            active.AddRange(h.Cast<IConstraint>());

            do
            {
                gradF = calc_f_gradient(x);
                A = formulateActiveSetAndGradients(x);
                double initAlpha;
                dk = calculateSQPSearchDirection(x, out initAlpha);
                meritFunction.penaltyWeight = adjustMeritPenalty();
                // this next function is not part of the regular SQP algorithm
                // it's only intended to keep all the points in the positive space.
                initAlpha = preventNegatives(x, dk, initAlpha);
                //

                alphaStar = lineSearchMethod.findAlphaStar(x, dk, initAlpha);
                x = x.add(StarMath.multiply(alphaStar, dk));
                SearchIO.output("iteration=" + k, 3);
                SearchIO.output("--alpha=" + alphaStar, 3);
                k++;
                fk = calc_f(x);

                SearchIO.output("----f = " + fk, 3);
                SearchIO.output("---#active =" + active.Count, 3);
            } while (notConverged(k, numEvals, fk, x, null, gradF));
            fStar = fk;
            xStar = (double[])x.Clone();
            return fStar;
        }

        /// <summary>
        /// Adjusts the merit penalty.
        /// </summary>
        /// <returns>System.Double.</returns>
        private double adjustMeritPenalty()
        {
            var weight = lambdas.norm1();

            /* what is the point of the next condition? is it a correct step when one is still in the
             * infeasible region? */
            if (weight < meritFunction.penaltyWeight)
                return 2 * meritFunction.penaltyWeight;
            return weight;
        }

        /// <summary>
        /// Prevents the negatives.
        /// </summary>
        /// <param name="xk">The xk.</param>
        /// <param name="dk">The dk.</param>
        /// <param name="initAlpha">The initialize alpha.</param>
        /// <returns>System.Double.</returns>
        private static double preventNegatives(IList<double> xk, double[] dk, double initAlpha)
        {
            for (var i = 0; i < dk.GetLength(0); i++)
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

            var newInfeasibles
                = new SortedList<double, IConstraint>(new optimizeSort(optimize.maximize));

            /* this foreach loop can remove any number of g's from the active set, and puts
             * any newly violated g's into the sorted list newInfeasbles. */
            foreach (IInequality c in g)
            {
                var gVal = calculate(c, xk);
                if (gVal < 0.0)
                {
                    if (active.Contains(c)) active.Remove(c);
                }
                else
                    newInfeasibles.Add(gVal, c);
            }
            /* only want to add one newInfeasible to the list of active. Hence the break statement. */
            foreach (var kvp in newInfeasibles.Where(kvp => !active.Contains(kvp.Value)))
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
        /// <param name="initAlpha">The init alpha.</param>
        /// <returns>System.Double[].</returns>
        private double[] calculateSQPSearchDirection(double[] xk, out double initAlpha)
        {
            activeVals = calc_active_vector(xk);
            var At = A.transpose();
            var invAAt = A.multiply(At).inverse();
            var AtinvAAt = At.multiply(invAAt);
            //double[,] J = StarMath.makeIdentity(n);
            var invJ = StarMath.makeIdentity(n);

            // double[,] P = StarMath.subtract(StarMath.makeIdentity(n), AtinvAAtA);

            var P = invJ.multiply(AtinvAAt.multiply(A).subtract(StarMath.makeIdentity(n)));
            //double[,] Q = StarMath.multiply(invJ,
            //        StarMath.multiply(AtinvAAt, StarMath.multiply(A,
            //        StarMath.multiply(J, StarMath.multiply(invAtA, At)))));
            var Q = AtinvAAt;
            var dirMin = P.multiply(gradF);
            var dirCC = Q.multiply(activeVals);
            var dir = dirMin.subtract(dirCC);

            lambdas = invAAt.multiply(activeVals.subtract(A.multiply(gradF)));

            if (double.IsNaN(dir[0])) dir = dk;
            initAlpha = dir.norm2();
            if (initAlpha > 0) dir = StarMath.multiply((1.0 / initAlpha), dir);
            return dir;
        }
    }
}