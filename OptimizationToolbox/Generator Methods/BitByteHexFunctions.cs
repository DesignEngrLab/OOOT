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

namespace OptimizationToolbox
{
    internal struct BitByteHexLimits
    {
        public int EndIndex;
        public long MaxValue;
        public int StartIndex;
    }

    internal static class BitByteHexFunctions
    {
        internal static BitByteHexLimits[] InitializeBitString(DesignSpaceDescription discreteSpaceDescriptor)
        {
            var result = new BitByteHexLimits[discreteSpaceDescriptor.n];
            var currentIndex = 0;
            for (var i = 0; i < discreteSpaceDescriptor.n; i++)
            {
                if (discreteSpaceDescriptor[i].Discrete)
                {
                    var maxValue = discreteSpaceDescriptor.MaxVariableSizes[i];
                    var numberBits = (int)(Math.Log(maxValue, 2)) + 1;
                    var endIndex = currentIndex + numberBits;
                    result[i] = new BitByteHexLimits
                                    {
                                        StartIndex = currentIndex,
                                        EndIndex = endIndex,
                                        MaxValue = maxValue
                                    };
                    currentIndex = endIndex;
                }
                else result[i] = new BitByteHexLimits();
            }
            return result;
        }

        internal static int FindVariableIndex(BitByteHexLimits[] limits, int i)
        {
            return Array.FindIndex(limits, a => a.EndIndex > i);
        }

        internal static BitArray Encode(long value, int length)
        {
            var result = new BitArray(length);
            var denominator = (long)Math.Pow(2, length - 1);
            for (var i = length - 1; i >= 0; i--)
            {
                if (value >= denominator)
                {
                    result.Set(i, true);
                    value -= denominator;
                }
                else result.Set(i, false);
                denominator /= 2;
            }
            return result;
        }

        internal static long Decode(BitArray b, long maxValue)
        {
            long result = 0;
            long factor = 1;
            for (var i = 0; i < b.Count; i++)
            {
                if (b[i]) result += factor;
                factor *= 2;
            }
            if (result >= maxValue) return result - maxValue;
            return result;
        }


        internal static long FlipBit(long initValue, BitByteHexLimits limits, int bitIndex)
        {
            bitIndex -= limits.StartIndex;
            var b = Encode(initValue, limits.EndIndex - limits.StartIndex);
            b.Set(bitIndex, !b[bitIndex]);
            return Decode(b, limits.MaxValue);
        }


        internal static void CrossoverBitString(BitArray c1BitArray, BitArray c2BitArray, int position,
                                                long maxValue, out long c1Value, out long c2Value)
        {
            for (var i = position; i < c1BitArray.Count; i++)
            {
                var c1Temp = c1BitArray[i];
                c1BitArray.Set(i, c2BitArray[i]);
                c2BitArray.Set(i, c1Temp);
            }
            c1Value = Decode(c1BitArray, maxValue);
            c2Value = Decode(c2BitArray, maxValue);
        }
    }
}