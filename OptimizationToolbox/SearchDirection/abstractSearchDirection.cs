// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="abstractSearchDirection.cs" company="OptimizationToolbox">
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

namespace OptimizationToolbox
{
    /// <summary>
    /// The class that all search direction methods must inherit from. By search direction, we mean
    /// the vector, d that the numerical method must search in. The simplest example being SteepestDescent -
    /// in the opposite direction of the gradient.
    /// </summary>
    public abstract class abstractSearchDirection
    {
        /// <summary>
        /// Finds the direction for the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="gradf">The gradf.</param>
        /// <param name="f">The f.</param>
        /// <param name="initAlpha">The init alpha.</param>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
        /// <returns>System.Double[].</returns>
        public abstract double[] find(double[] x, double[] gradf, double f, ref double initAlpha, Boolean reset = false);

        /// <summary>
        /// Finds the direction for the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="gradf">The gradf.</param>
        /// <param name="f">The f.</param>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
        /// <returns>System.Double[].</returns>
        public virtual double[] find(double[] x, double[] gradf, double f, Boolean reset = false)
        {
            var dummy = double.NaN;
            return find(x, gradf, f, ref dummy, reset);
        }
    }
}