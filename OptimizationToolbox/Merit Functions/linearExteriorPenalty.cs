// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="linearExteriorPenalty.cs" company="OptimizationToolbox">
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
    /// Class linearExteriorPenalty.
    /// Implements the <see cref="OptimizationToolbox.abstractMeritFunction" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractMeritFunction" />
    public class linearExteriorPenalty : abstractMeritFunction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="linearExteriorPenalty"/> class.
        /// </summary>
        /// <param name="optMethod">The opt method.</param>
        /// <param name="penaltyWeight">The penalty weight.</param>
        public linearExteriorPenalty(abstractOptMethod optMethod, double penaltyWeight)
            : base(optMethod, penaltyWeight)
        {
        }

        #endregion

        /// <summary>
        /// Calcs the penalty.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>System.Double.</returns>
        public override double calcPenalty(double[] point)
        {
            var sum = 0.0;
            double temp;

            foreach (var c in optMethod.h)
            {
                temp = optMethod.calculate(c, point);
                sum += Math.Abs(temp);
            }
            foreach (var c in optMethod.g)
            {
                temp = optMethod.calculate(c, point);
                if (temp > 0) sum += temp;
            }
            sum *= penaltyWeight;
            return sum;
        }

        /// <summary>
        /// Calculates the gradient of penalty.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>System.Double[].</returns>
        public override double[] calcGradientOfPenalty(double[] point)
        {
            var n = point.GetLength(0);
            var grad = new double[n];
            for (var i = 0; i != n; i++)
            {
                var sum = 0.0;
                double temp;
                foreach (var c in optMethod.h)
                {
                    temp = optMethod.calculate(c, point);
                    if (temp < 0.0)
                        sum -= optMethod.deriv_wrt_xi(c,point, i);
                    if (temp > 0.0)
                        sum += optMethod.deriv_wrt_xi(c,point, i);
                }
                foreach (var c in optMethod.g)
                {
                    temp = optMethod.calculate(c, point);
                    if (temp > 0.0)
                        sum += optMethod.deriv_wrt_xi(c,point, i);
                }
                grad[i] = penaltyWeight * sum;
            }
            return grad;
        }
    }
}