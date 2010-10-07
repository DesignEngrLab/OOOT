using System;
using System.Collections;
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
        public GACrossoverBitString(DiscreteSpaceDescriptor discreteSpaceDescriptor, double crossoverRate = 1.7)
            : base(discreteSpaceDescriptor)
        {
            this.crossoverRate = crossoverRate;
            limits = GABitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            xRatePerBit = crossoverRate / bitStringLength;
            rnd = new Random();
        }

        public override void generateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int targetPopNumber = -1)
        {
            /* if no population size is provided, then it is assumed that the population should
             * double from the current one. */
            if (targetPopNumber == -1) targetPopNumber = candidates.Count;
            else targetPopNumber -= candidates.Count;
            var numNew = 0;
            while (numNew < targetPopNumber)
            {
                var child1 = (double[])candidates[(numNew++) % candidates.Count].Value.Clone();
                var child2 = (double[])candidates[(numNew++) % candidates.Count].Value.Clone();
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
                        CrossoverBitString(c1BitArray, c2BitArray,
                            i - limits[varIndex].StartIndex, limits[varIndex].MaxValue, out c1Value, out c2Value);
                        child1[varIndex] = c2Value;
                        child2[varIndex] = c1Value;
                    }
                candidates.Add(new KeyValuePair<double, double[]>(double.NaN, child1));
                candidates.Add(new KeyValuePair<double, double[]>(double.NaN, child2));

            }
        }



        private static void CrossoverBitString(BitArray c1BitArray, BitArray c2BitArray, int position,
            long maxValue, out long c1Value, out long c2Value)
        {
            for (int i = position; i < c1BitArray.Count; i++)
            {
                var c1Temp = c1BitArray[i];
                c1BitArray.Set(i, c2BitArray[i]);
                c2BitArray.Set(i, c1Temp);
            }
            c1Value = GABitByteHexFunctions.Decode(c1BitArray, maxValue);
            c2Value = GABitByteHexFunctions.Decode(c2BitArray, maxValue);
        }
    }
}
