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

namespace OptimizationToolbox
{
    public class GACrossoverBitString : GeneticCrossoverGenerator
    {
        private readonly int bitStringLength;
        private readonly BitByteHexLimits[] limits;
        private readonly Random rnd;
        private readonly double xRatePerBit;

        public GACrossoverBitString(DesignSpaceDescription discreteSpaceDescriptor, double crossoverRate = 1.7)
            : base(discreteSpaceDescriptor)
        {
            limits = BitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            xRatePerBit = crossoverRate / bitStringLength;
            rnd = new Random();
        }

        public override void GenerateCandidates(ref List<KeyValuePair<double, double[]>> candidates,
                                                int targetPopNumber = -1)
        {
            /* if no population size is provided, then it is assumed that the population should
             * double from the current one. */
            if (targetPopNumber == -1) targetPopNumber = candidates.Count;
            else targetPopNumber -= candidates.Count;
            var numNew = 0;
            while (numNew < targetPopNumber)
            {
                var parent1 = candidates[(numNew) % candidates.Count].Value;
                var parent2 = candidates[(numNew + 1) % candidates.Count].Value;
                var child1 = (double[])parent1.Clone();
                var child2 = (double[])parent2.Clone();
                var ChangeMade = false;
                for (var i = 1; i < bitStringLength; i++)
                {
                    if (rnd.NextDouble() < xRatePerBit)
                    {
                        ChangeMade = true;
                        var varIndex = BitByteHexFunctions.FindVariableIndex(limits, i);
                        if (varIndex + 1 < n)
                        {
                            /* switch the remaining double values. No need to encode/decode them.*/
                            var tailLength = n - varIndex - 1;
                            var child1Tail = new double[tailLength];
                            Array.Copy(child1, varIndex + 1, child1Tail, 0, tailLength);
                            Array.Copy(child2, varIndex + 1, child1, varIndex + 1, tailLength);
                            Array.Copy(child1Tail, 0, child2, varIndex + 1, tailLength);
                        }
                        var c1Value = discreteSpaceDescriptor[varIndex].PositionOf(child1[varIndex]);
                        var c1BitArray = BitByteHexFunctions.Encode(c1Value,
                                                                    limits[varIndex].EndIndex -
                                                                    limits[varIndex].StartIndex);
                        var c2Value = discreteSpaceDescriptor[varIndex].PositionOf(child2[varIndex]);
                        var c2BitArray = BitByteHexFunctions.Encode(c2Value,
                                                                    limits[varIndex].EndIndex -
                                                                    limits[varIndex].StartIndex);
                        BitByteHexFunctions.CrossoverBitString(c1BitArray, c2BitArray,
                                                               i - limits[varIndex].StartIndex,
                                                               limits[varIndex].MaxValue, out c1Value,
                                                               out c2Value);
                        child1[varIndex] = discreteSpaceDescriptor[varIndex][c1Value];
                        child2[varIndex] = discreteSpaceDescriptor[varIndex][c2Value];
                    }
                }
                if (ChangeMade)
                {
                    numNew += 2;
                    candidates.Add(new KeyValuePair<double, double[]>(double.NaN, child1));
                    candidates.Add(new KeyValuePair<double, double[]>(double.NaN, child2));
                }
            }
        }
    }
}