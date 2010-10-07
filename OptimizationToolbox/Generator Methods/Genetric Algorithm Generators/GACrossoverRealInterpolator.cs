using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class GACrossoverRealInterpolator : GeneticCrossoverGenerator
    {
        public GACrossoverRealInterpolator(DiscreteSpaceDescriptor discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }

        public override void generateCandidates(ref List<double[]> candidates, int number = -1)
        {
            throw new NotImplementedException();
        }
    }
}
