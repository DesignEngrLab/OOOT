// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="RandomSampling.cs" company="OptimizationToolbox">
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
    /// Class RandomSampling.
    /// Implements the <see cref="OptimizationToolbox.SamplingGenerator" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.SamplingGenerator" />
    public class RandomSampling : SamplingGenerator
    {
        /// <summary>
        /// The default real bound
        /// </summary>
        public static double DefaultRealBound = 10000;
        /// <summary>
        /// The random
        /// </summary>
        private readonly Random rnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSampling"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        public RandomSampling(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
            rnd = new Random();
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="numSamples">The number samples.</param>
        public override void GenerateCandidates(ref List<ICandidate> candidates, int numSamples = -1)
        {
            if (numSamples == -1) numSamples = (int)MaxVariableSizes.Min();
            for (var i = 0; i < numSamples; i++)
                candidates.Add(new Candidate(double.NaN, makeOneRandomCandidate()));
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="numSamples">The number samples.</param>
        /// <returns>List&lt;System.Double[]&gt;.</returns>
        public override List<double[]> GenerateCandidates(double[] candidate, int numSamples = -1)
        {
            var candidates = new List<double[]>();
            if (numSamples == -1) numSamples = (int)MaxVariableSizes.Min();
            for (var i = 0; i < numSamples; i++)
                candidates.Add(makeOneRandomCandidate());
            return candidates;
        }

        /// <summary>
        /// Makes the one random candidate.
        /// </summary>
        /// <returns>System.Double[].</returns>
        private double[] makeOneRandomCandidate()
        {
            var x = new double[n];
            for (var j = 0; j < n; j++)
                if (discreteSpaceDescriptor.DiscreteVarIndices.Contains(j))
                {
                    var index = rnd.Next((int)MaxVariableSizes[j]);
                    x[j] = discreteSpaceDescriptor[j][index];
                }
                else
                {
                    var range = discreteSpaceDescriptor[j].UpperBound - discreteSpaceDescriptor[j].LowerBound;
                    if (double.IsInfinity(range)) range = 2 * DefaultRealBound;
                    var lb = discreteSpaceDescriptor[j].LowerBound;
                    if (double.IsInfinity(lb)) lb = -DefaultRealBound;
                    x[j] = range * rnd.NextDouble() + lb;
                }
            return x;
        }
    }
}