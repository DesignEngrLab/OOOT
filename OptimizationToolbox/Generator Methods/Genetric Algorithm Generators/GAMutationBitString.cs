using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class GAMutationBitString : GeneticMutationGenerator
    {
        private readonly double mutationRate;
        private readonly double mRatePerBit;
        private readonly GABitByteHexLimits[] limits;
        private readonly long bitStringLength;
        private readonly Random rnd;
        public GAMutationBitString(DiscreteSpaceDescriptor discreteSpaceDescriptor, double mutationRate = 0.01)
            : base(discreteSpaceDescriptor)
        {
            this.mutationRate = mutationRate;
            limits = GABitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            mRatePerBit = mutationRate / bitStringLength;
            rnd = new Random();
        }

        public override void generateCandidates(ref List<double[]> candidates, int control = -1)
        {
            foreach (var c in candidates)
                for (int i = 0; i < bitStringLength; i++)
                    if (rnd.NextDouble() < mRatePerBit)
                    {
                        int varIndex = GABitByteHexFunctions.FindVariableIndex(limits, i);
                        long valueIndex = VariableDescriptors[varIndex].PositionOf(c[varIndex]);
                        c[varIndex] = GABitByteHexFunctions.FlipBit(valueIndex, limits[varIndex], i);
                    }
        }

    }
}
