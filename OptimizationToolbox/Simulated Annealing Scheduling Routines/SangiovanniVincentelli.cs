/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class SACoolingSangiovanniVincentelli : abstractSimulatedAnnealingCoolingSchedule
    {
        private const int initialSamples = 100;
        private const double initProbabilityForThreeSigma = 0.8;
        private readonly double lambda;
        private double[] objectiveValues;

        public SACoolingSangiovanniVincentelli(int samplesInGeneration, double lambda = 0.7)
            : base(samplesInGeneration)
        {
            this.lambda = lambda;
        }

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
            var stdev = StarMath.standardDeviation(objectiveValues);

            objectiveValues = new double[samplesInGeneration];
            return -3 * stdev / Math.Log(initProbabilityForThreeSigma);
        }

        internal override double UpdateTemperature(double temperature, List<ICandidate> candidates)
        {
            objectiveValues[samplesThusFar++] = candidates[0].objectives[0];
            if (samplesThusFar < samplesInGeneration) return temperature;
            samplesThusFar = 0;
            var stdev = StarMath.standardDeviation(objectiveValues);
            var newTemp = temperature * Math.Exp(-1 * lambda * temperature / stdev);
            if (newTemp == 0) newTemp = .99 * temperature;
            return newTemp;
        }
    }
}