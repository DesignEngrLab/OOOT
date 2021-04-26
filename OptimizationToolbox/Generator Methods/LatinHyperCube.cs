// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="LatinHyperCube.cs" company="OptimizationToolbox">
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
    /// Enum VariablesInScope
    /// </summary>
    public enum VariablesInScope
    {
        /// <summary>
        /// The only discrete
        /// </summary>
        OnlyDiscrete,
        /// <summary>
        /// The only real
        /// </summary>
        OnlyReal,
        /// <summary>
        /// The both discrete and real
        /// </summary>
        BothDiscreteAndReal
    } ;

    /// <summary>
    /// Class LatinHyperCube.
    /// Implements the <see cref="OptimizationToolbox.SamplingGenerator" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.SamplingGenerator" />
    public class LatinHyperCube : SamplingGenerator
    {
        /// <summary>
        /// The generate for
        /// </summary>
        private readonly VariablesInScope generateFor;

        /// <summary>
        /// Initializes a new instance of the <see cref="LatinHyperCube"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        /// <param name="GenerateFor">The generate for.</param>
        public LatinHyperCube(DesignSpaceDescription discreteSpaceDescriptor, VariablesInScope GenerateFor)
            : base(discreteSpaceDescriptor)
        {
            generateFor = GenerateFor;
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="numSamples">The number samples.</param>
        /// <returns>List&lt;System.Double[]&gt;.</returns>
        /// <exception cref="Exception">The bounds on the " + j + "(th) variable must not be at infinity for" +
        ///                                             "performing Latin Hypercube sampling.</exception>
        public override List<double[]> GenerateCandidates(double[] candidate, int numSamples = -1)
        {
            if (numSamples == -1) numSamples = (int)MaxVariableSizes.Min();
            var rnd = new Random();
            var data = new List<double>[n];

            // the following is not correct - need to fix
            // also what about the non-discrete variables and LHC?
            for (var j = 0; j < n; j++)
            {
                var varVals = new List<double>();
                if (discreteSpaceDescriptor.DiscreteVarIndices.Contains(j) && generateFor != VariablesInScope.OnlyReal)
                {
                    varVals = new List<double>();
                    for (var i = 0; i < numSamples; i++)
                    {
                        var effectiveIndex = (int)(((double)i * discreteSpaceDescriptor[j].Size) / numSamples);
                        varVals.Add(discreteSpaceDescriptor[j][effectiveIndex]);
                    }
                }
                else if (!discreteSpaceDescriptor.DiscreteVarIndices.Contains(j) &&
                         generateFor != VariablesInScope.OnlyDiscrete)
                {
                    var delta = (discreteSpaceDescriptor[j].UpperBound - discreteSpaceDescriptor[j].LowerBound);
                    if (double.IsInfinity(delta))
                        throw new Exception("The bounds on the " + j + "(th) variable must not be at infinity for" +
                                            "performing Latin Hypercube sampling.");
                    delta /= numSamples;
                    var lb = discreteSpaceDescriptor[j].LowerBound;
                    varVals = new List<double>();
                    for (var i = 0; i < numSamples; i++) varVals.Add(lb + i * delta);
                }
                else for (var i = 0; i < numSamples; i++) varVals.Add(double.NaN);
                varVals = varVals.OrderBy(a => rnd.NextDouble()).ToList();
                data[j] = varVals;
            }
            var candidates = new List<double[]>(numSamples);
            for (var i = 0; i < numSamples; i++)
            {
                var point = new double[n];
                for (var j = 0; j < n; j++)
                    point[j] = data[j][i];
                candidates.Add(point);
            }
            return candidates;
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="numSamples">The number samples.</param>
        public override void GenerateCandidates(ref List<ICandidate> candidates, int numSamples = -1)
        {
            var candVectors = GenerateCandidates(null, numSamples);
            foreach (var candVector in candVectors)
                candidates.Add(new Candidate(double.NaN, candVector));
        }
    }
}