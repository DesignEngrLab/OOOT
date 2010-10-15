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
    public enum VariablesInScope
    {
        OnlyDiscrete,
        OnlyReal,
        BothDiscreteAndReal
    } ;

    public class LatinHyperCube : SamplingGenerator
    {
        private readonly VariablesInScope generateFor;

        public LatinHyperCube(DesignSpaceDescription discreteSpaceDescriptor, VariablesInScope GenerateFor)
            : base(discreteSpaceDescriptor)
        {
            generateFor = GenerateFor;
        }

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
                        var effectiveIndex = (int)(Math.Round(((double)i * VariableDescriptors[j].Size) / numSamples));
                        varVals.Add(VariableDescriptors[j][effectiveIndex]);
                    }
                }
                else if (!discreteSpaceDescriptor.DiscreteVarIndices.Contains(j) &&
                         generateFor != VariablesInScope.OnlyDiscrete)
                {
                    var delta = (VariableDescriptors[j].UpperBound - VariableDescriptors[j].LowerBound);
                    if (double.IsInfinity(delta))
                        throw new Exception("The bounds on the " + j + "(th) variable must not be at infinity for" +
                                            "performing Latin Hypercube sampling.");
                    delta /= numSamples;
                    var lb = VariableDescriptors[j].LowerBound;
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

        public override void GenerateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int numSamples = -1)
        {
            var candVectors = GenerateCandidates(null, numSamples);
            foreach (var candVector in candVectors)
                candidates.Add(new KeyValuePair<double, double[]>(double.NaN, candVector));
        }
    }
}