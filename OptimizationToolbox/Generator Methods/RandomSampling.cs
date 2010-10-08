using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationToolbox
{
    public class RandomSampling : SamplingGenerator
    {
        private const double defaultRealBound = 10000;
        public RandomSampling(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }

        public override void generateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int numSamples = -1)
        {
            var rnd = new Random();

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
                candidates.Add(new KeyValuePair<double, double[]>(double.NaN, x));
            }
        }
    }
}
