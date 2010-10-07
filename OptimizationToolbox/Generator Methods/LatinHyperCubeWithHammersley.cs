using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimizationToolbox
{
    public class LatinHyperCubeWithHammersley : SamplingGenerator
    {
        public LatinHyperCubeWithHammersley(DiscreteSpaceDescriptor discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }

        public override void generateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int numSamples = -1)
        {
            //if (numSamples == -1) numSamples = MaxVariableSizes.Min();
            //Random rnd = new Random();
            //CandidatesIndices = new int[numSamples][];
            //CandidateOutputs = new double[numSamples];

            //// the following is not correct - need to fix
            //// also what about the non-discrete variables and LHC?
            //for (int j = 0; j < n; j++)
            //{
            //    sampleIndices[j] = new List<int>();
            //    for (int i = 0; i < numSamples; i++)
            //        sampleIndices[j].Insert(rnd.Next(sampleIndices[j].Count), i);
            //}
            //long start = DateTime.Now.Ticks;
            //double[] f = new double[numSamples];
            //for (int i = 0; i < numSamples; i++)
            //    f[i] = optMethod.calc_f(GetVariableVector(CandidatesIndices[i]), true);

            //return DateTime.Now.Ticks - start;
           // candidates = new List<double[]>();
        }
    }
}
