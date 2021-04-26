// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="DirectSearchStepTooSmallConvergence.cs" company="OptimizationToolbox">
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
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class DirectSearchStepTooSmallConvergence.
    /// Implements the <see cref="OptimizationToolbox.abstractConvergence" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractConvergence" />
    internal class DirectSearchStepTooSmallConvergence : abstractConvergence
    {
        /// <summary>
        /// Gets or sets the has converged.
        /// </summary>
        /// <value>The has converged.</value>
        internal Boolean hasConverged { get; set; }

        /// <summary>
        /// Convergeds the specified iteration.
        /// </summary>
        /// <param name="iteration">The iteration.</param>
        /// <param name="numFnEvals">The number function evals.</param>
        /// <param name="fBest">The f best.</param>
        /// <param name="xBest">The x best.</param>
        /// <param name="population">The population.</param>
        /// <param name="gradF">The grad f.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool converged(long iteration = -1, long numFnEvals = -1, double fBest = double.NaN,
            IList<double> xBest = null, IList<double[]> population = null, IList<double> gradF = null)
        {
            return hasConverged;
        }
    }
}