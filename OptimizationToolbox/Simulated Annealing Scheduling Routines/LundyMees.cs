﻿/*************************************************************************
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
    public class SACoolingLundyMees : abstractSimulatedAnnealingCoolingSchedule
    {
        private const int initialSamples = 100;
        private const double initProbabilityForThreeSigma = 0.99;
        private readonly double beta;
        private double[] objectiveValues;

        public SACoolingLundyMees(int samplesInGeneration, double beta = 0.7)
            : base(samplesInGeneration)
        {
            this.beta = beta;
        }

        internal override double SetInitialTemperature()
        {
            var LHC = new LatinHyperCube(optMethod.spaceDescriptor, VariablesInScope.OnlyDiscrete);
            var initCandidates = LHC.GenerateCandidates(null, initialSamples);
            objectiveValues = new double[initialSamples];
            for (var j = 0; j < initialSamples; j++)
            {
                for (var i = 0; i < optMethod.n; i++)
                    if (!optMethod.spaceDescriptor.DiscreteVarIndices.Contains(i))
                        initCandidates[j][i] = optMethod.xStart[i];
                objectiveValues[j] = optMethod.calc_f(initCandidates[j], true);
            }
            var stdev = objectiveValues.standardDeviation();

            objectiveValues = new double[samplesInGeneration];
            return -3 * stdev / Math.Log(initProbabilityForThreeSigma);
        }

        internal override double UpdateTemperature(double temperature, List<ICandidate> candidates)
        {
            throw new NotImplementedException();
            objectiveValues[samplesThusFar++] = candidates[0].objectives[0];
            if (samplesThusFar < samplesInGeneration)
                return temperature;
            samplesThusFar = 0;
            var stdev = objectiveValues.standardDeviation();
            return temperature * Math.Exp(-1 * beta * temperature / stdev);
        }
    }
}