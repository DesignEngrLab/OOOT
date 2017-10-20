/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT X11 License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT X11 License for more details.
 *  


 *     
 *     Please find further details and contact information on OOOT
 *     at http://designengrlab.github.io/OOOT/.
 *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationToolbox
{
    public class StochasticNeighborGenerator : abstractGenerator
    {
        public static double MinToMaxRatio = 0.05;
        private readonly int[][] changeVectors;
        private readonly optimize direction;
        private readonly Random r;
        private int changeVectorIndex;
        private double[] performance;
        private int[] population;

        public StochasticNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor, optimize direction,
                                           int maxNumNeighbors = 250)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors(maxNumNeighbors);
            this.direction = direction;
            ResetStats();
        }

        public int changesStored { get; private set; }

        public override List<double[]> GenerateCandidates(double[] candidate, int control = -1)
        {
            var neighbor = (double[])candidate.Clone();
            var changes = discreteSpaceDescriptor.FindValidChanges(neighbor, changeVectors);
            var probabilities = makeProbabilites(changes);
            var z = findIndex(r.NextDouble(), probabilities);
            changeVectorIndex = changes[z];
            for (var i = 0; i < n; i++)
                if (changeVectors[changeVectorIndex][i] != 0)
                {
                    var valueIndex = discreteSpaceDescriptor[i].PositionOf(neighbor[i]);
                    valueIndex += changeVectors[changeVectorIndex][i];
                    neighbor[i] = discreteSpaceDescriptor[i][valueIndex];
                }
            return new List<double[]> { neighbor };
        }

        private static int findIndex(double p, IList<double> probabilities)
        {
            var i = 0;
            while (p > 0) p -= probabilities[i++];
            return i;
        }

        private double[] makeProbabilites(IList<int> changes)
        {
            var result = new double[changes.Count];
            if (changesStored == 0)
                for (var i = 0; i < result.GetLength(0); i++)
                    result[i] = 1.0;
            else
            {
                for (var i = 0; i < result.GetLength(0); i++)
                    if (performance[changes[i]] != 0)
                        result[i] = performance[changes[i]] / population[changes[i]];
                    else result[i] = 0;
                var minP = MinToMaxRatio * result.Max();
                for (var i = 0; i < result.GetLength(0); i++)
                    if (result[i] < minP) result[i] = minP;
            }
            var sum = result.Sum();
            for (var i = 0; i < result.GetLength(0); i++)
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
            switch (direction)
            {
                case optimize.neither:
                    delta = Math.Abs(delta);
                    break;
                case optimize.minimize:
                    delta = -1 * Math.Min(delta, 0);
                    break;
                default:
                    delta = Math.Max(delta, 0);
                    break;
            }

            performance[changeVectorIndex] += delta;
            population[changeVectorIndex]++;
            changesStored++;
        }
    }
}