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

namespace OptimizationToolbox
{
    public class RandomNeighborGenerator : abstractGenerator
    {
        private readonly int[][] changeVectors;
        private readonly Random r;
        private int changeVectorIndex;

        public RandomNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor, int maxNumNeighbors = 250)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors(maxNumNeighbors);
        }

        public override List<double[]> GenerateCandidates(double[] candidate, int control = -1)
        {
            var neighbor = (double[])candidate.Clone();
            var changes = discreteSpaceDescriptor.FindValidChanges(neighbor, changeVectors);
            var z = r.Next(changes.Count);
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
    }
}