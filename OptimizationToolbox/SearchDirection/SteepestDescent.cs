// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="SteepestDescent.cs" company="OptimizationToolbox">
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
    /// Class SteepestDescent.
    /// Implements the <see cref="OptimizationToolbox.abstractSearchDirection" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractSearchDirection" />
    public class SteepestDescent : abstractSearchDirection
    {
        /// <summary>
        /// Finds the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="gradf">The gradf.</param>
        /// <param name="f">The f.</param>
        /// <param name="initAlpha">The initialize alpha.</param>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
        /// <returns>System.Double[].</returns>
        public override double[] find(double[] x, double[] gradf, double f, ref double initAlpha, bool reset = false)
        {
            /* calc the magnitude of the new gradient, magGradF. This is used several
             * times so in order to minimize time, calc it once and save it. */
            var magGradF = gradf.norm2();
            if (magGradF == 0) return gradf;
            /* if the gradient of f is all zeros, then simply return it. */

            return (StarMath.multiply((-1.0 / magGradF), gradf));
        }
    }
}