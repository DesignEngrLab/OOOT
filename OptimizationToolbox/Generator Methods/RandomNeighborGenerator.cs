// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="RandomNeighborGenerator.cs" company="OptimizationToolbox">
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
    /// Class RandomNeighborGenerator.
    /// Implements the <see cref="OptimizationToolbox.abstractGenerator" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractGenerator" />
    public class RandomNeighborGenerator : abstractGenerator
    {
        /// <summary>
        /// The change vectors
        /// </summary>
        private readonly int[][] changeVectors;
        /// <summary>
        /// The r
        /// </summary>
        private readonly Random r;
        /// <summary>
        /// The change vector index
        /// </summary>
        private int changeVectorIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomNeighborGenerator"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        /// <param name="minNumNeighbors">The minimum number neighbors.</param>
        public RandomNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor, int minNumNeighbors = 50)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors(minNumNeighbors);
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="control">The control.</param>
        /// <returns>List&lt;System.Double[]&gt;.</returns>
        public override List<double[]> GenerateCandidates(double[] candidate, int control = -1)
        {
            var neighbor = (double[])candidate.Clone();
            var changes = discreteSpaceDescriptor.FindValidChanges(neighbor, changeVectors);
            var z = r.Next(changes.Count);
            changeVectorIndex = changes[z];
            for (var i = 0; i < n; i++)
                if (changeVectors[changeVectorIndex][i] != 0)
                {
                    var valueIndex = discreteSpaceDescriptor[i].PositionOf(neighbor[i]);
                    valueIndex += changeVectors[changeVectorIndex][i];
                    neighbor[i] = discreteSpaceDescriptor[i][valueIndex];
                }
            return new List<double[]> { neighbor };
        }
    }
}