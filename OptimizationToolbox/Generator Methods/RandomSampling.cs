using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationToolbox
{
    public class RandomSampling : SamplingGenerator
    {
        private const double defaultRealBound = 10000;
        private readonly Random rnd;
        public RandomSampling(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
            rnd = new Random();
        }

        public override void GenerateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int numSamples = -1)
        {
            if (numSamples == -1) numSamples = (int)MaxVariableSizes.Min();
            for (int i = 0; i < numSamples; i++)
                candidates.Add(new KeyValuePair<double, double[]>(double.NaN, makeOneRandomCandidate()));
        }
        public override List<double[]> GenerateCandidates(double[] candidate, int numSamples = -1)
        {
           var candidates = new List<double[]>();
             if (numSamples == -1) numSamples = (int)MaxVariableSizes.Min();
            for (int i = 0; i < numSamples; i++)
                candidates.Add(makeOneRandomCandidate());
            return candidates;
        }

        private double[] makeOneRandomCandidate()
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
            return x;
        }
    }
}
