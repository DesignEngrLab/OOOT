using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarMathLib;

namespace OptimizationToolbox
{
    public class ExhaustiveNeighborGenerator : abstractGenerator
    {
        readonly Random r;
        readonly int[][] changeVectors;

        public ExhaustiveNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors();
        }


        public override List<double[]> GenerateCandidates(double[] current, int maxNumber = -1)
        {
            var candidates = new List<double[]>();
            var changeVectorIndices = discreteSpaceDescriptor.FindValidChanges(current, changeVectors);
            if (maxNumber > 0)
            {
                while (changeVectorIndices.Count > maxNumber)
                    changeVectorIndices.RemoveAt(r.Next(changeVectorIndices.Count));
            }
            foreach (var changeVectorIndex in changeVectorIndices)
            {
                var neighbor = (double[])current.Clone();
                for (int i = 0; i < n; i++)
                    if (changeVectors[changeVectorIndex][i] != 0)
                    {
                        var valueIndex = VariableDescriptors[i].PositionOf(neighbor[i]);
                        valueIndex += changeVectors[changeVectorIndex][i];
                        neighbor[i] = VariableDescriptors[i][valueIndex];
                    }
                candidates.Add(neighbor);
            }
            return candidates;
        }
    }
}
