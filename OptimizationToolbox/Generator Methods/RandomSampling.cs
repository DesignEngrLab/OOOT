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
using System.Linq;

namespace OptimizationToolbox
{
    public class RandomSampling : SamplingGenerator
    {
        private const double defaultRealBound = 10000;
        private readonly Random rnd;

        public RandomSampling(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
            rnd = new Random();
        }

        public override void GenerateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int numSamples = -1)
        {
            if (numSamples == -1) numSamples = (int)MaxVariableSizes.Min();
            for (var i = 0; i < numSamples; i++)
                candidates.Add(new KeyValuePair<double, double[]>(double.NaN, makeOneRandomCandidate()));
        }

        public override List<double[]> GenerateCandidates(double[] candidate, int numSamples = -1)
        {
            var candidates = new List<double[]>();
            if (numSamples == -1) numSamples = (int)MaxVariableSizes.Min();
            for (var i = 0; i < numSamples; i++)
                candidates.Add(makeOneRandomCandidate());
            return candidates;
        }

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
                    if (double.IsInfinity(range)) range = 2 * defaultRealBound;
                    var lb = discreteSpaceDescriptor[j].LowerBound;
                    if (double.IsInfinity(lb)) lb = -defaultRealBound;
                    x[j] = range * rnd.NextDouble() + lb;
                }
            return x;
        }
    }
}