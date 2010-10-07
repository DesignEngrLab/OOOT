using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationToolbox
{
    public class RandomSampling : abstractGenerator
    {
        private const double defaultRealBound = 10000;
        public RandomSampling(DiscreteSpaceDescriptor discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }

        public override void generateCandidates(ref List<double[]> candidates, int numSamples = -1)
        {
            Random rnd = new Random();

            candidates = new List<double[]>();
            if (numSamples == -1) numSamples = (int)MaxVariableSizes.Min();
            for (int i = 0; i < numSamples; i++)
            {
                var x = new double[n];
                for (int j = 0; j < n; j++)
                    if (discreteSpaceDescriptor.DiscreteVarIndices.Contains(j))
                    {
                        var index = rnd.Next((int)MaxVariableSizes[j]);
                        x[j] = discreteSpaceDescriptor.VariableDescriptors[j][index];
                    }
                    else
                    {
                        var range = VariableDescriptors[j].UpperBound - VariableDescriptors[j].LowerBound;
                        if (double.IsInfinity(range)) range = 2 * defaultRealBound;
                        var lb = VariableDescriptors[j].LowerBound;
                        if (double.IsInfinity(lb)) lb = -defaultRealBound;
                        x[j] = range * rnd.NextDouble() + lb;
                    }
            }
        }
    }
}
