// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="StochasticNeighborGenerator.cs" company="OptimizationToolbox">
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
using System.Linq;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class StochasticNeighborGenerator.
    /// Implements the <see cref="OptimizationToolbox.abstractGenerator" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractGenerator" />
    public class StochasticNeighborGenerator : abstractGenerator
    {
        /// <summary>
        /// The minimum to maximum ratio
        /// </summary>
        public static double MinToMaxRatio = 0.05;
        /// <summary>
        /// The change vectors
        /// </summary>
        private readonly int[][] changeVectors;
        /// <summary>
        /// The direction
        /// </summary>
        private readonly optimize direction;
        /// <summary>
        /// The r
        /// </summary>
        private readonly Random r;
        /// <summary>
        /// The change vector index
        /// </summary>
        private int changeVectorIndex;
        /// <summary>
        /// The performance
        /// </summary>
        private double[] performance;
        /// <summary>
        /// The population
        /// </summary>
        private int[] population;

        /// <summary>
        /// Initializes a new instance of the <see cref="StochasticNeighborGenerator"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="maxNumNeighbors">The maximum number neighbors.</param>
        public StochasticNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor, optimize direction,
                                           int maxNumNeighbors = 250)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors(maxNumNeighbors);
            this.direction = direction;
            ResetStats();
        }

        /// <summary>
        /// Gets the changes stored.
        /// </summary>
        /// <value>The changes stored.</value>
        public int changesStored { get; private set; }

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
            var probabilities = makeProbabilites(changes);
            var z = findIndex(r.NextDouble(), probabilities);
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

        /// <summary>
        /// Finds the index.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="probabilities">The probabilities.</param>
        /// <returns>System.Int32.</returns>
        private static int findIndex(double p, IList<double> probabilities)
        {
            var i = 0;
            while (p > 0) p -= probabilities[i++];
            return i;
        }

        /// <summary>
        /// Makes the probabilites.
        /// </summary>
        /// <param name="changes">The changes.</param>
        /// <returns>System.Double[].</returns>
        private double[] makeProbabilites(IList<int> changes)
        {
            var result = new double[changes.Count];
            if (changesStored == 0)
                for (var i = 0; i < result.GetLength(0); i++)
                    result[i] = 1.0;
            else
            {
                for (var i = 0; i < result.GetLength(0); i++)
                    if (performance[changes[i]] != 0)
                        result[i] = performance[changes[i]] / population[changes[i]];
                    else result[i] = 0;
                var minP = MinToMaxRatio * result.Max();
                for (var i = 0; i < result.GetLength(0); i++)
                    if (result[i] < minP) result[i] = minP;
            }
            var sum = result.Sum();
            for (var i = 0; i < result.GetLength(0); i++)
                result[i] /= sum;

            return result;
        }

        /// <summary>
        /// Resets the stats.
        /// </summary>
        public void ResetStats()
        {
            performance = new double[changeVectors.GetLength(0)];
            population = new int[changeVectors.GetLength(0)];
            changesStored = 0;
        }

        /// <summary>
        /// Records the effect.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public void RecordEffect(double delta)
        {
            switch (direction)
            {
                case optimize.neither:
                    delta = Math.Abs(delta);
                    break;
                case optimize.minimize:
                    delta = -1 * Math.Min(delta, 0);
                    break;
                default:
                    delta = Math.Max(delta, 0);
                    break;
            }

            performance[changeVectorIndex] += delta;
            population[changeVectorIndex]++;
            changesStored++;
        }
    }
}