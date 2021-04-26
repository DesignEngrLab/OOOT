// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="abstractLineSearch.cs" company="OptimizationToolbox">
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
    /// Class abstractLineSearch.
    /// </summary>
    public abstract class abstractLineSearch
    {
        /// <summary>
        /// The infeasibles
        /// </summary>
        private readonly List<IConstraint> infeasibles;
        /// <summary>
        /// The track feasibility
        /// </summary>
        private readonly Boolean trackFeasibility;
        /// <summary>
        /// the tolerance value, epsilon is used to distinguish values of alpha. It is part
        /// of the convergence for the line search.
        /// </summary>
        protected double epsilon;
        /// <summary>
        /// the iterations are counted with k
        /// </summary>
        protected int k;
        /// <summary>
        /// Kmax is the maximum iterations to convergence.
        /// </summary>
        protected int  kMax;
        /// <summary>
        /// The last feas alpha
        /// </summary>
        internal double lastFeasAlpha, lastFeasAlpha4G, lastFeasAlpha4H;

        /// <summary>
        /// The opt method
        /// </summary>
        private abstractOptMethod optMethod;
        /// <summary>
        /// stepSize is the discretization step taken between values of alpha
        /// </summary>
        protected double stepSize;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="abstractLineSearch" /> class.
        /// </summary>
        /// <param name="epsilon">The epsilon.</param>
        /// <param name="stepSize">Size of the step.</param>
        /// <param name="kMax">The k max.</param>
        /// <param name="trackFeasibility">if set to <c>true</c> [track feasibility].</param>
        protected abstractLineSearch(double epsilon, double stepSize, int kMax,
                                  Boolean trackFeasibility = false)
        {
            this.epsilon = epsilon;
            this.stepSize = stepSize;
            this.kMax = kMax;
            infeasibles = new List<IConstraint>();
            this.trackFeasibility = trackFeasibility;
        }

        #endregion

        /// <summary>
        /// Finds the alpha star.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="dir">The dir.</param>
        /// <returns>System.Double.</returns>
        public abstract double findAlphaStar(double[] x, double[] dir);

        /// <summary>
        /// Finds the alpha star.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="allowNegAlpha">if set to <c>true</c> [allow neg alpha].</param>
        /// <returns>System.Double.</returns>
        public double findAlphaStar(double[] x, double[] dir, Boolean allowNegAlpha)
        {
            return findAlphaStar(x, dir, allowNegAlpha, stepSize);
        }

        /// <summary>
        /// Finds the alpha star.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="initAlpha">The init alpha.</param>
        /// <returns>System.Double.</returns>
        public double findAlphaStar(double[] x, double[] dir, double initAlpha)
        {
            return findAlphaStar(x, dir, false, initAlpha);
        }

        /* the above 3 overloads all flow into this master one, which simply catches
         * all cases. The user still only needs to write the dervied classes only
         * implement the basic one that takes only x and dir. */

        /// <summary>
        /// Finds the alpha star.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="allowNegAlpha">if set to <c>true</c> [allow neg alpha].</param>
        /// <param name="initAlpha">The init alpha.</param>
        /// <returns>System.Double.</returns>
        public double findAlphaStar(double[] x, double[] dir, Boolean allowNegAlpha, double initAlpha)
        {
            var tempStepSize = stepSize;
            stepSize = initAlpha;

            var alpha1 = findAlphaStar(x, dir);
            if ((alpha1 < epsilon) && allowNegAlpha)
                alpha1 = -findAlphaStar(x, StarMath.multiply(-1.0, dir));

            stepSize = tempStepSize;

            return alpha1;
        }

        /// <summary>
        /// Calcs the objective function value.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="dir">The dir.</param>
        /// <returns>System.Double.</returns>
        protected double calcF(double[] start, double alpha, double[] dir)
        {
            var point = start.add(StarMath.multiply(alpha, dir));
            if (trackFeasibility)
            {
                var temp = optMethod.calc_f(point, true);
                trackLastFeasible(point, alpha);
                return temp;
            }
            return optMethod.calc_f(point, true);
        }

        /// <summary>
        /// Tracks the last feasible.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="alpha">The alpha.</param>
        private void trackLastFeasible(double[] point, double alpha)
        {
            var allConstraints = optMethod.h.Cast<IConstraint>().ToList();
            allConstraints.AddRange(optMethod.g.Cast<IConstraint>());

            foreach (var c in allConstraints)
                if ((optMethod.feasible(c, point)) && (infeasibles.Contains(c))) infeasibles.Remove(c);
                else if ((!optMethod.feasible(c, point)) && (!infeasibles.Contains(c))) infeasibles.Add(c);

            if (infeasibles.Count == 0) lastFeasAlpha = lastFeasAlpha4G = lastFeasAlpha4H = alpha;
            else if (!infeasibles.Any(ic => (ic is IEquality)))
                lastFeasAlpha4H = alpha;
            else if (!infeasibles.Any(ic => (ic is IInequality)))
                lastFeasAlpha4G = alpha;
        }

        /// <summary>
        /// Sets the optimization details.
        /// </summary>
        /// <param name="optmethod">The optmethod.</param>
        internal void SetOptimizationDetails(abstractOptMethod optmethod)
        {
            optMethod = optmethod;
        }
    }
}