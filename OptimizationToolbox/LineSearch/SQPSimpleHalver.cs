// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="SQPSimpleHalver.cs" company="OptimizationToolbox">
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
    /// Class SQPSimpleHalver.
    /// Implements the <see cref="OptimizationToolbox.abstractLineSearch" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractLineSearch" />
    public class SQPSimpleHalver : abstractLineSearch
    {
        /// <summary>
        /// The gamma
        /// </summary>
        private readonly double gamma;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SQPSimpleHalver"/> class.
        /// </summary>
        /// <param name="gamma">The gamma.</param>
        /// <param name="kMax">The k maximum.</param>
        public SQPSimpleHalver(double gamma, int kMax)
            : base(double.NaN, double.NaN, kMax)
        {
            this.gamma = gamma;
        }

        #endregion

        /// <summary>
        /// Finds the alpha star.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="dir">The dir.</param>
        /// <returns>System.Double.</returns>
        public override double findAlphaStar(double[] x, double[] dir)
        {
            var f_at_current = calcF(x, 0, dir);
            double fnew;
            double alpha;
            k = 0;
            do
            {
                alpha = stepSize / (Math.Pow(2, k));
                fnew = calcF(x, alpha, dir);
            } while ((fnew > (f_at_current - gamma * stepSize * alpha)) && (k++ < kMax));

            /* truthfully, this already requires a full step of dk and then backs off on this
             * until a simple condition is satisfied - lookup convergence */
            return alpha;
        }
    }
}