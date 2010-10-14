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
using System.Text;
using StarMathLib;

namespace OptimizationToolbox
{
    public class ExhaustiveNeighborGenerator : abstractGenerator
    {
        readonly Random r;
        readonly int[][] changeVectors;

        public ExhaustiveNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor, int maxNumNeighbors=250)
            : base(discreteSpaceDescriptor)
        {
            r = new Random();
            changeVectors = discreteSpaceDescriptor.CreateNeighborChangeVectors(maxNumNeighbors);
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
