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

namespace OptimizationToolbox
{
    public class ExhaustiveNeighborGenerator : abstractGenerator
    {
        private readonly int[][] changeVectors;
        private readonly Random r;

        public ExhaustiveNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor, int minNumNeighbors = 250)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors(minNumNeighbors);
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
                for (var i = 0; i < n; i++)
                    if (changeVectors[changeVectorIndex][i] != 0)
                    {
                        var valueIndex = discreteSpaceDescriptor[i].PositionOf(neighbor[i]);
                        valueIndex += changeVectors[changeVectorIndex][i];
                        neighbor[i] = discreteSpaceDescriptor[i][valueIndex];
                    }
                candidates.Add(neighbor);
            }
            return candidates;
        }
    }
}