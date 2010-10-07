using System;
using System.Collections;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class GAMutationBitString : GeneticMutationGenerator
    {
        private readonly double mRatePerBit;
        private readonly GABitByteHexLimits[] limits;
        private readonly long bitStringLength;
        private readonly Random rnd;
        public GAMutationBitString(DiscreteSpaceDescriptor discreteSpaceDescriptor, double mutationRate = 0.01)
            : base(discreteSpaceDescriptor)
        {
            limits = GABitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            mRatePerBit = mutationRate / bitStringLength;
            rnd = new Random();
        }

        public override void generateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int control = -1)
        {
            for (int i = candidates.Count; i >= 0; i--)
            {
                var candidate = candidates[i].Value;
                var ChangeMade = false;
                for (int j = 0; j < bitStringLength; j++)
                    if (rnd.NextDouble() < mRatePerBit)
                    {
                        ChangeMade = true;
                        int varIndex = GABitByteHexFunctions.FindVariableIndex(limits, j);
                        long valueIndex = VariableDescriptors[varIndex].PositionOf(candidate[varIndex]);
                        candidate[varIndex] = FlipBit(valueIndex, limits[varIndex], j);
                    }
                if (ChangeMade)
                {
                    candidates.RemoveAt(i);
                    candidates.Add(new KeyValuePair<double, double[]>(double.NaN, candidate));
                }
            }
        }


        private static long FlipBit(long initValue, GABitByteHexLimits limits, int bitIndex)
        {
            bitIndex -= limits.StartIndex;
            BitArray b = GABitByteHexFunctions.Encode(initValue, limits.EndIndex - limits.StartIndex);
            b.Set(bitIndex, !b[bitIndex]);
            return GABitByteHexFunctions.Decode(b, limits.MaxValue);
        }

    }
}
