// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="Elitism.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
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
using System.Xml;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class Elitism.
    /// Implements the <see cref="OptimizationToolbox.abstractSelector" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractSelector" />
    public class Elitism : abstractSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Elitism"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public Elitism(optimize direction)
            : base(new[] { direction })
        {
        }

        /// <summary>
        /// Selects the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="fractionToKeep">The fraction to keep.</param>
        public override void SelectCandidates(ref List<ICandidate> candidates, double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int)(candidates.Count() * fractionToKeep);
            var orderedCands = candidates.OrderBy(a => a.objectives[0], directionComparer[0]);
            candidates = orderedCands.Take(numKeep).ToList();
        }

    }
}