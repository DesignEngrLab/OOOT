using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarMathLib;

namespace OptimizationToolbox
{
    public class StochasticNeighborGenerator : abstractGenerator
    {
        private const double minToMaxRatio = 0.05;
        readonly Random r;
        readonly int[][] changeVectors;
        double[] performance;
        int[] population;
        int changeVectorIndex;
        readonly optimize direction;
        public int changesStored { get; private set; }

        public StochasticNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor, optimize direction)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors();
            this.direction = direction;
            ResetStats();
        }
        public override List<double[]> GenerateCandidates(double[] candidate, int control = -1)
        {
            var neighbor = (double[])candidate.Clone();
            var changes = discreteSpaceDescriptor.FindValidChanges(neighbor, changeVectors);
            double[] probabilities = makeProbabilites(changes);
            int z = findIndex(r.NextDouble(), probabilities);
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

        private int findIndex(double p, double[] probabilities)
        {
            int i = 0;
            while (p > 0) p -= probabilities[i++];
            return i;
        }

        private double[] makeProbabilites(List<int> changes)
        {
            var result = new double[changes.Count];
            if (changesStored == 0)
                for (int i = 0; i < result.GetLength(0); i++)
                    result[i] = 1.0;
            else
            {
                for (int i = 0; i < result.GetLength(0); i++)
                    if (performance[changes[i]] != 0)
                        result[i] = performance[changes[i]] / population[changes[i]];
                    else result[i] = 0;
                var minP = minToMaxRatio * result.Max();
                for (int i = 0; i < result.GetLength(0); i++)
                    if (result[i] < minP) result[i] = minP;
            }
            var sum = result.Sum();
            for (int i = 0; i < result.GetLength(0); i++)
                result[i] /= sum;

            return result;
        }
        public void ResetStats()
        {
            performance = new double[changeVectors.GetLength(0)];
            population = new int[changeVectors.GetLength(0)];
            changesStored = 0;
        }

        public void RecordEffect(double delta)
        {
            if (direction == optimize.neither) delta = Math.Abs(delta);
            else if (direction == optimize.minimize) delta = -1 * Math.Min(delta, 0);
            else delta = Math.Max(delta, 0);

            performance[changeVectorIndex] += delta;
            population[changeVectorIndex]++;
            changesStored++;
        }
    }
}
