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
using System.Collections;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class BitStringNeighborGenerator : abstractGenerator
    {
        private readonly BitByteHexLimits[] limits;
        private readonly int bitStringLength;
        private readonly Random rnd;
        public BitStringNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
            limits = BitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            rnd = new Random();
        }
        public override List<double[]> GenerateCandidates(double[] candidate, int numToCreate = -1)
        {
            var newCands = new List<double[]>();
            if (numToCreate == -1) numToCreate = 1;
            while (numToCreate-- > 0)
            {
                var result = (double[])candidate.Clone();
                var j = rnd.Next(bitStringLength);
                int varIndex = BitByteHexFunctions.FindVariableIndex(limits, j);
                long valueIndex = VariableDescriptors[varIndex].PositionOf(result[varIndex]);
                valueIndex = BitByteHexFunctions.FlipBit(valueIndex, limits[varIndex], j);
                result[varIndex] = VariableDescriptors[varIndex][valueIndex];
                newCands.Add(result);
            }
            return newCands;
        }
    }
}