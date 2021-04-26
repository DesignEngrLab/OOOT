// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="ArithmeticMean.cs" company="OptimizationToolbox">
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
    /// Arithmetic Mean 1-D search as described by ? Rao(?)
    /// </summary>
    public class ArithmeticMean : abstractLineSearch
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ArithmeticMean" /> class.
        /// </summary>
        /// <param name="epsilon">The epsilon.</param>
        /// <param name="stepSize">Size of the step.</param>
        /// <param name="kMax">The k max.</param>
        public ArithmeticMean(double epsilon, double stepSize, int kMax)
            : base(epsilon, stepSize, kMax)
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
            var fLow = calcF(x, alphaLow, dir);
            var alphaHigh = stepSize;
            var fHigh = calcF(x, alphaHigh, dir);
            double alphaMid, fMid;
            var step = stepSize;

            k = 1; //so that this local k corresponds to # of f'n evals (starting at 0)

            #region Setting up Brackets Phase

            if (fHigh <= fLow)
            {
                // these two lines are just to set up alphaMid and fMid so that the while loop will correctly
                alphaMid = alphaLow;
                fMid = fLow;
                do
                {
                    alphaLow = alphaMid;
                    fLow = fMid;
                    alphaMid = alphaHigh;
                    fMid = fHigh;
                    step *= 2;
                    alphaHigh = alphaMid + step;
                    fHigh = calcF(x, alphaHigh, dir);
                    k++;
                } while ((fHigh < fMid) && (k < kMax));
            }
            else
            {
                // these two lines are just to set up alphaMid and fMid so that the while loop will correctly
                alphaMid = alphaHigh;
                fMid = fHigh;
                do
                {
                    alphaHigh = alphaMid;
                    fHigh = fMid;
                    /* this may be different from the published DSC approach. In that method, 
                     * the mid point is in the middle of low and high. The problem is that when
                     * the arithmetic mean is taken, you arrive at the same point. So, borrowing
                     * from the four-point bracketing in golden-section and the 5/9 method, the 
                     * mid is placed at 2/3 the distance. In this the 3 points are positioned 
                     * similar to how they are in the expanding case. */
                    alphaMid = 2 * (alphaHigh - alphaLow) / 3;
                    fMid = calcF(x, alphaMid, dir);
                    k++;
                } while ((fLow < fMid) && (k < kMax));
            }

            #endregion

            #region Arithmetic Mean to reduce bracket

            var alphaNew = (alphaLow + alphaMid + alphaHigh) / 3;
            var fNew = calcF(x, alphaNew, dir);
            k++;
            do
            {
                var fMax = Math.Max(fLow, Math.Max(fNew, Math.Max(fMid, fHigh)));
                // find the largest of the four values
                if ((fLow == fMax) ||
                    ((fNew == fMax) && (alphaNew < alphaMid)) ||
                    ((fMid == fMax) && (alphaMid < alphaNew))) //if lowest two alphas are max, then remove alphaLow
                {
                    // then remove alphaLow 
                    if (alphaNew < alphaMid)
                    {
                        //alphaLow moves up to alphaNew, alphaMid, alphaHigh stay the same
                        alphaLow = alphaNew;
                        fLow = fNew;
                    }
                    else
                    {
                        //alphaLow moves up to alphaMid,alphaNew changes to alphaMid, alphaHigh stays
                        alphaLow = alphaMid;
                        fLow = fMid;
                        alphaMid = alphaNew;
                        fMid = fNew;
                    }
                }
                else //else highest two alpha create fMax, so remove alpahHigh
                {
                    if (alphaNew > alphaMid)
                    {
                        //alphaHigh moves down to alphaNew, alphaMid, alphaLow stay the same
                        alphaHigh = alphaNew;
                        fHigh = fNew;
                    }
                    else
                    {
                        //alphaHigh moves down to alphaMid,alphaNew changes to alphaMid, alphaLow stays
                        alphaHigh = alphaMid;
                        fHigh = fMid;
                        alphaMid = alphaNew;
                        fMid = fNew;
                    }
                }
                alphaNew = (alphaLow + alphaMid + alphaHigh) / 3;
                fNew = calcF(x, alphaNew, dir);
                k++;
            } while ((Math.Abs(alphaNew - alphaMid) > epsilon) && (k < kMax));

            #endregion

            if (fMid < fNew) return alphaMid;
            return alphaNew;
        }
    }
}