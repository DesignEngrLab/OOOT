// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="SangiovanniVincentelli.cs" company="OptimizationToolbox">
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
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class SACoolingSangiovanniVincentelli.
    /// Implements the <see cref="OptimizationToolbox.abstractSimulatedAnnealingCoolingSchedule" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractSimulatedAnnealingCoolingSchedule" />
    public class SACoolingSangiovanniVincentelli : abstractSimulatedAnnealingCoolingSchedule
    {
        /// <summary>
        /// The initial samples
        /// </summary>
        private const int initialSamples = 100;
        /// <summary>
        /// The initialize probability for three sigma
        /// </summary>
        private const double initProbabilityForThreeSigma = 0.8;
        /// <summary>
        /// The lambda
        /// </summary>
        private readonly double lambda;
        /// <summary>
        /// The objective values
        /// </summary>
        private double[] objectiveValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="SACoolingSangiovanniVincentelli"/> class.
        /// </summary>
        /// <param name="samplesInGeneration">The samples in generation.</param>
        /// <param name="lambda">The lambda.</param>
        public SACoolingSangiovanniVincentelli(int samplesInGeneration, double lambda = 0.7)
            : base(samplesInGeneration)
        {
            this.lambda = lambda;
        }

        /// <summary>
        /// Sets the initial temperature.
        /// </summary>
        /// <returns>System.Double.</returns>
        internal override double SetInitialTemperature()
        {
            var randNeighbor = new RandomNeighborGenerator(optMethod.spaceDescriptor);
            objectiveValues = new double[initialSamples];
            var x = (double[])optMethod.xStart.Clone();
            for (var j = 0; j < initialSamples; j++)
            {
                objectiveValues[j] = optMethod.calc_f(x, true);
                x = randNeighbor.GenerateCandidates(x)[0];
            }
            var stdev = objectiveValues.standardDeviation();

            objectiveValues = new double[samplesInGeneration];
            return -3 * stdev / Math.Log(initProbabilityForThreeSigma);
        }

        /// <summary>
        /// Updates the temperature.
        /// </summary>
        /// <param name="temperature">The temperature.</param>
        /// <param name="candidates">The candidates.</param>
        /// <returns>System.Double.</returns>
        internal override double UpdateTemperature(double temperature, List<ICandidate> candidates)
        {
            objectiveValues[samplesThusFar++] = candidates[0].objectives[0];
            if (samplesThusFar < samplesInGeneration) return temperature;
            samplesThusFar = 0;
            var stdev = objectiveValues.standardDeviation();
            var newTemp = temperature * Math.Exp(-1 * lambda * temperature / stdev);
            if (newTemp == 0) newTemp = .99 * temperature;
            return newTemp;
        }
    }
}