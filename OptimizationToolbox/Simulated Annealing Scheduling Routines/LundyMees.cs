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
            var stdev = StarMath.standardDeviation(objectiveValues);

            objectiveValues = new double[samplesInGeneration];
            return -3 * stdev / Math.Log(initProbabilityForThreeSigma);
        }

        internal override double UpdateTemperature(double temperature, List<Candidate> candidates)
        {
            throw new NotImplementedException();
            objectiveValues[samplesThusFar++] = candidates[0].fValues[0];
            if (samplesThusFar < samplesInGeneration)
                return temperature;
            samplesThusFar = 0;
            var stdev = StarMath.standardDeviation(objectiveValues);
            return temperature * Math.Exp(-1 * beta * temperature / stdev);
        }
    }
}