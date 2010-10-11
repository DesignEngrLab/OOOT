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
        public GAMutationBitString(DesignSpaceDescription discreteSpaceDescriptor, double mutationRate = 0.1)
            : base(discreteSpaceDescriptor)
        {
            limits = GABitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            mRatePerBit = mutationRate / bitStringLength;
            rnd = new Random();
        }

        public override void GenerateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int control = -1)
        {
            for (int i = candidates.Count-1; i >= 0; i--)
            {
                var candidate = candidates[i].Value;
                var ChangeMade = false;
                for (int j = 0; j < bitStringLength; j++)
                    if (rnd.NextDouble() < mRatePerBit)
                    {
                        ChangeMade = true;
                        int varIndex = GABitByteHexFunctions.FindVariableIndex(limits, j);
                        long valueIndex = VariableDescriptors[varIndex].PositionOf(candidate[varIndex]);
                        valueIndex = FlipBit(valueIndex, limits[varIndex], j);
                        candidate[varIndex] = VariableDescriptors[varIndex][valueIndex];
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
