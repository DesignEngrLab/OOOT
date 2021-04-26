// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="squaredExteriorPenalty.cs" company="OptimizationToolbox">
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

namespace OptimizationToolbox
{
    /// <summary>
    /// Class squaredExteriorPenalty.
    /// Implements the <see cref="OptimizationToolbox.abstractMeritFunction" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractMeritFunction" />
    public class squaredExteriorPenalty : abstractMeritFunction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="squaredExteriorPenalty"/> class.
        /// </summary>
        /// <param name="optMethod">The opt method.</param>
        /// <param name="penaltyWeight">The penalty weight.</param>
        public squaredExteriorPenalty(abstractOptMethod optMethod, double penaltyWeight)
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
                sum += temp * temp;
            }
            foreach (var c in optMethod.g)
            {
                temp = optMethod.calculate(c, point);
                if (temp > 0) sum += temp * temp;
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
            var hvals = new double[optMethod.h.Count];
            var gvals = new double[optMethod.g.Count];
            for (var i = 0; i < optMethod.h.Count; i++)
                hvals[i] = optMethod.calculate(optMethod.h[i], point);
            for (var i = 0; i < optMethod.g.Count; i++)
                gvals[i] = optMethod.calculate(optMethod.g[i], point);

            for (var j = 0; j != n; j++)
            {
                var sum = 0.0;
                for (var i = 0; i < optMethod.h.Count; i++)
                    if (hvals[i] != 0.0)
                        sum += hvals[i] * optMethod.deriv_wrt_xi(optMethod.h[i], point, j);

                for (var i = 0; i < optMethod.g.Count; i++)
                    if (gvals[i] > 0.0)
                        sum += gvals[i] * optMethod.deriv_wrt_xi(optMethod.g[i], point, j);
                grad[j] = 2 * penaltyWeight * sum;
            }
            return grad;
        }
    }
}