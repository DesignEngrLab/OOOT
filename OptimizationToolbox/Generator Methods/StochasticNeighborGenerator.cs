/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

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

        public StochasticNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor, optimize direction, int maxNumNeighbors = 250)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors(maxNumNeighbors);
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

        private static int findIndex(double p, double[] probabilities)
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
