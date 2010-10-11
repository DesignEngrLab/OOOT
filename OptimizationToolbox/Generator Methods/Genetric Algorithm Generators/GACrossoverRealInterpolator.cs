using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class GACrossoverRealInterpolator : GeneticCrossoverGenerator
    {
        public GACrossoverRealInterpolator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }

        public override void GenerateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int number = -1)
        {
            throw new NotImplementedException();
        }
    }
}
