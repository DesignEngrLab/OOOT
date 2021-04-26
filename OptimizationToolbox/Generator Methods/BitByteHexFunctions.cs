// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="BitByteHexFunctions.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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
using System.Collections;
using System.Linq;

namespace OptimizationToolbox
{
    /// <summary>
    /// Struct BitByteHexLimits
    /// </summary>
    internal struct BitByteHexLimits
    {
        /// <summary>
        /// The end index
        /// </summary>
        public int EndIndex;
        /// <summary>
        /// The maximum value
        /// </summary>
        public long MaxValue;
        /// <summary>
        /// The start index
        /// </summary>
        public int StartIndex;
    }

    /// <summary>
    /// Class BitByteHexFunctions.
    /// </summary>
    internal static class BitByteHexFunctions
    {
        /// <summary>
        /// Initializes the bit string.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        /// <returns>BitByteHexLimits[].</returns>
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

        /// <summary>
        /// Finds the index of the variable.
        /// </summary>
        /// <param name="limits">The limits.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Int32.</returns>
        internal static int FindVariableIndex(BitByteHexLimits[] limits, int i)
        {
            for (int index = 0; index < limits.GetLength(0); index++)
                if (limits[index].EndIndex > i)
                    return index;
            return -1;
        }

        /// <summary>
        /// Encodes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <returns>BitArray.</returns>
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

        /// <summary>
        /// Decodes the specified b.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>System.Int64.</returns>
        internal static long Decode(BitArray b, long maxValue)
        {
            long result = 0;
            long factor = 1;
            for (var i = 0; i < b.Length; i++)
            {
                if (b[i]) result += factor;
                factor *= 2;
            }
            if (result >= maxValue) return result - maxValue;
            return result;
        }


        /// <summary>
        /// Flips the bit.
        /// </summary>
        /// <param name="initValue">The initialize value.</param>
        /// <param name="limits">The limits.</param>
        /// <param name="bitIndex">Index of the bit.</param>
        /// <returns>System.Int64.</returns>
        internal static long FlipBit(long initValue, BitByteHexLimits limits, int bitIndex)
        {
            bitIndex -= limits.StartIndex;
            var b = Encode(initValue, limits.EndIndex - limits.StartIndex);
            b.Set(bitIndex, !b[bitIndex]);
            return Decode(b, limits.MaxValue);
        }


        /// <summary>
        /// Crossovers the bit string.
        /// </summary>
        /// <param name="c1BitArray">The c1 bit array.</param>
        /// <param name="c2BitArray">The c2 bit array.</param>
        /// <param name="position">The position.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="c1Value">The c1 value.</param>
        /// <param name="c2Value">The c2 value.</param>
        internal static void CrossoverBitString(BitArray c1BitArray, BitArray c2BitArray, int position,
                                                long maxValue, out long c1Value, out long c2Value)
        {
            for (var i = position; i < c1BitArray.Length; i++)
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