// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="MetropolisCriteria.cs" company="OptimizationToolbox">
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

namespace OptimizationToolbox
{
    /// <summary>
    /// Class MetropolisCriteria.
    /// Implements the <see cref="OptimizationToolbox.abstractSelector" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractSelector" />
    public class MetropolisCriteria : abstractSelector
    {
        /// <summary>
        /// The random
        /// </summary>
        private readonly Random rnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetropolisCriteria"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public MetropolisCriteria(optimize direction)
            : base(new[] { direction })
        {
            rnd = new Random();
        }

        /// <summary>
        /// Selects the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="temperature">The temperature.</param>
        public override void SelectCandidates(ref List<ICandidate> candidates, double temperature = double.NaN)
        {
            var fOld = candidates[0].objectives[0];
            var fNew = candidates[1].objectives[0];
            if ((fNew == fOld) || (BetterThan(fNew, fOld)))
                /* throw away the old and keep the new */
                candidates.RemoveAt(0);
            else
            {
                var probability = Math.Exp(((int)optDirections[0]) * (fNew - fOld) / temperature);
                SearchIO.output("fnew = " + fNew + "; fold = " + fOld + "; prob = " + probability, 5);
                if (rnd.NextDouble() <= probability)
                {
                    /* throw away the old and keep the new */
                    candidates.RemoveAt(0);
                    SearchIO.output("keep new", 5);
                }
                /* otherwise stay with the old */
                else
                {
                    candidates.RemoveAt(1);
                    SearchIO.output("keep old", 5);
                }
            }
        }
    }
}