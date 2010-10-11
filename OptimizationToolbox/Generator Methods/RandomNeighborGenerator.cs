using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarMathLib;

namespace OptimizationToolbox
{
    public class RandomNeighborGenerator : abstractGenerator
    {
        readonly Random r;
        readonly int[][] changeVectors;
        int changeVectorIndex;

        public RandomNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors();
        }

        public override List<double[]> GenerateCandidates(double[] candidate, int control = -1)
        {
            var neighbor = (double[])candidate.Clone();
            var changes = discreteSpaceDescriptor.FindValidChanges(neighbor, changeVectors);
            var z = r.Next(changes.Count);
            changeVectorIndex = changes[z];
            for (int i = 0; i < n; i++)
                if (changeVectors[changeVectorIndex][i] != 0)
                {
                    var valueIndex = VariableDescriptors[i].PositionOf(neighbor[i]);
                    valueIndex += changeVectors[changeVectorIndex][i];
                    neighbor[i] = VariableDescriptors[i][valueIndex];
                }
            return new List<double[]>() { neighbor };
        }
    }
}
