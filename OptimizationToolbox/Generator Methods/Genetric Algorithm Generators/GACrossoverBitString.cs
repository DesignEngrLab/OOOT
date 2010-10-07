using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class GACrossoverBitString : GeneticCrossoverGenerator
    {
        private readonly double crossoverRate;
        private readonly double xRatePerBit;
        private readonly GABitByteHexLimits[] limits;
        private readonly long bitStringLength;
        private readonly Random rnd;
        public GACrossoverBitString(DiscreteSpaceDescriptor discreteSpaceDescriptor, double crossoverRate = 2)
            : base(discreteSpaceDescriptor)
        {
            this.crossoverRate = crossoverRate;
            limits = GABitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            xRatePerBit = crossoverRate / bitStringLength;
            rnd = new Random();
        }

        public override void generateCandidates(ref List<double[]> candidates, int populationSize = -1)
        {
            /* if no population size is provided, then it is assumed that the population should
             * double from the current one. */
            if (populationSize == -1) populationSize = candidates.Count;
            else populationSize -= candidates.Count;
            var newCandidates = new List<double[]>(populationSize);
            var numNew = 0;
            while (numNew < populationSize)
            {
                var child1 = (double[])candidates[(numNew++) % candidates.Count].Clone();
                var child2 = (double[])candidates[(numNew++) % candidates.Count].Clone();
                for (int i = 0; i < bitStringLength; i++)
                    if (rnd.NextDouble() < xRatePerBit)
                    {
                        int varIndex = GABitByteHexFunctions.FindVariableIndex(limits, i);
                        if (varIndex + 1 < n)
                        {
                            /* switch the remaining double values. No need to encode/decode them.*/
                            var tailLength = n - varIndex - 1;
                            var child1Tail = new double[tailLength];
                            Array.Copy(child1, varIndex + 1, child1Tail, 0, tailLength);
                            Array.Copy(child2, varIndex + 1, child1, varIndex + 1, tailLength);
                            Array.Copy(child1Tail, 0, child1, varIndex + 1, tailLength);
                        }
                        var c1Value = VariableDescriptors[varIndex].PositionOf(child1[varIndex]);
                        var c1BitArray = GABitByteHexFunctions.Encode(c1Value,
                                                                      limits[varIndex].EndIndex -
                                                                      limits[varIndex].StartIndex);
                        var c2Value = VariableDescriptors[varIndex].PositionOf(child2[varIndex]);
                        var c2BitArray = GABitByteHexFunctions.Encode(c2Value,
                                                                      limits[varIndex].EndIndex -
                                                                      limits[varIndex].StartIndex);
                        GABitByteHexFunctions.CrossoverBitString(c1BitArray, c2BitArray,
                            i - limits[varIndex].StartIndex, limits[varIndex].MaxValue, out c1Value, out c2Value);
                        child1[varIndex] = c2Value;
                        child2[varIndex] = c1Value;
                    }

            }
        }
    }
}
