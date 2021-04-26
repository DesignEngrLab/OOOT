// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="FixedOrGoldenSection.cs" company="OptimizationToolbox">
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
    /// Class FixedOrGoldenSection.
    /// Implements the <see cref="OptimizationToolbox.abstractLineSearch" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractLineSearch" />
    public class FixedOrGoldenSection : abstractLineSearch
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GoldenSection" /> class.
        /// Unlike other line search methods, there is no need to provide a kmax.
        /// This is determined directly from the epsilon in the body of the code
        /// </summary>
        /// <param name="epsilon">The epsilon.</param>
        /// <param name="upperBound">The upper bound.</param>
        public FixedOrGoldenSection(double epsilon, double upperBound)
            : base(epsilon, upperBound, -1)
        {
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
            var alphaLow = 0.0;
            var alphaHigh = stepSize;
            var alpha1 = Parameters.Golden38 * alphaHigh;
            var alpha2 = Parameters.Golden62 * alphaHigh;

            var fLow = calcF(x, alphaLow, dir);
            var fHigh = calcF(x, alphaHigh, dir);
            var f2 = calcF(x, alpha2, dir);
            var f1 = double.NaN;

            #region Setting up Bracket

            /* not sure this has been published before, but borrowing the concept from DSC, what the
             * following while-loop is doing is to check if the three points are decreasing, if so
             * we define a new alphaHigh - in the GS-proportion so that the old alpahHigh can become
             * the new alpha2, the old alpha2 can become a new alpha1. In this way, can define a 
             * much lower initial bracket, so that it is similar to the other two line search methods. */
            if ((fHigh < f2) && (fHigh < fLow))
                return alphaHigh;
            
            f1 = calcF(x, alpha1, dir);

            #endregion

            /* the number -2.078086921235 in the following formula is the 1/ln(golden62).
             * Since each new iteration (or function evaluations) will effectively reduce the
             * space by 62%, we can a priori determine the number of iterations that need to be
             * called. In this way, GS is the most robust, but perhaps not the most efficent. */
            kMax = (int)Math.Ceiling(-2.078086921235 * (Math.Log(epsilon, Math.E) -
                                                       Math.Log(alphaHigh, Math.E)));

            for (k = 0; k <= kMax; k++)
            {
                if (f1 < f2)
                {
                    alphaHigh = alpha2;
                    alpha2 = alpha1;
                    f2 = f1;
                    alpha1 = Parameters.Golden38 * (alphaHigh - alphaLow) + alphaLow;
                    f1 = calcF(x, alpha1, dir);
                    // reduce the upper bounds, alphaLow does not
                    // change in this case.
                }
                else
                {
                    alphaLow = alpha1;
                    alpha1 = alpha2;
                    f1 = f2;
                    alpha2 = Parameters.Golden62 * (alphaHigh - alphaLow) + alphaLow;
                    f2 = calcF(x, alpha2, dir);
                }
            }
            if (f1 < f2) return alpha1;
            return alpha2;
        }
    }
}