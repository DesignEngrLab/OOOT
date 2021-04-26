// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="OneDimensionalSearch.cs" company="OptimizationToolbox">
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
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class OneDimensionalSearch.
    /// Implements the <see cref="OptimizationToolbox.abstractOptMethod" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractOptMethod" />
    public class OneDimensionalSearch : abstractOptMethod
    {
        /// <summary>
        /// The step
        /// </summary>
        private readonly double step;
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OneDimensionalSearch" /> class.
        /// </summary>
        /// <param name="step">The step.</param>
        public OneDimensionalSearch(double step = double.NaN)
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = true;
            RequiresLineSearchMethod = true;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = false;
            this.step = step;
            if (double.IsNaN(step)) this.step = 1.0;
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
            var dk = new[] { 1.0 };
            // use line search to find alphaStar
            var alphaStar = lineSearchMethod.findAlphaStar(x, dk, true, step);
            x = xStar = x.add(new[] { alphaStar });
            return calc_f(x);
        }
        #endregion
    }
}