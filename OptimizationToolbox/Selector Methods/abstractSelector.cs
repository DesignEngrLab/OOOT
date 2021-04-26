// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="abstractSelector.cs" company="OptimizationToolbox">
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
    /// The class that all selector classes must inherit from.
    /// </summary>
    public abstract class abstractSelector
    {
        /// <summary>
        /// the direction of the search foreach object: maximizing or minimizing
        /// </summary>
        protected readonly optimize[] optDirections;
        /// <summary>
        /// The direction comparer
        /// </summary>
        protected readonly optimizeSort[] directionComparer;
        /// <summary>
        /// The number objectives
        /// </summary>
        protected readonly int numObjectives;

        /// <summary>
        /// Initializes a new instance of the <see cref="abstractSelector" /> class.
        /// </summary>
        /// <param name="optimizationDirections">The optimization directions.</param>
        protected abstractSelector(optimize[] optimizationDirections)
        {
            numObjectives = optimizationDirections.GetLength(0);
            if (optDirections == null)
            {
                optDirections = new[] {optimize.minimize};
                directionComparer = new [] { new optimizeSort(optimize.minimize) };
            }
            else
            {
                optDirections = new optimize[numObjectives];
                directionComparer = new optimizeSort[numObjectives];
                for (int i = 0; i < numObjectives; i++)
                {
                    optDirections[i] = optimizationDirections[i];
                    directionComparer[i] = new optimizeSort(optDirections[i]);
                }
            }
        }

        /// <summary>
        /// Selects the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="control">The control.</param>
        public abstract void SelectCandidates(ref List<ICandidate> candidates,
                                              double control = double.NaN);




        /// <summary>
        /// if x betters the than y given the direction of the search
        /// maximizing or minimizing.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Boolean.</returns>
        /// <exception cref="Exception">The default 'BetterThan' function can only function when there is a single objective function.</exception>
        /// <exception cref="System.Exception">The default 'BetterThan' function
        /// can only function when there is a single objective function.</exception>
        protected Boolean BetterThan(double x, double y)
        {
            if (numObjectives>1) 
                throw new Exception("The default 'BetterThan' function can only function when there is a single objective function.");
            return directionComparer[0].BetterThan(x, y);
        }
    }
}