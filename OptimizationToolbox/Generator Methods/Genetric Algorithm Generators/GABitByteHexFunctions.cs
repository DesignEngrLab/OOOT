using System;
using System.Collections;

namespace OptimizationToolbox
{

    internal struct GABitByteHexLimits
    {
        public int StartIndex;
        public int EndIndex;
        public long MaxValue;

    }

    internal static class GABitByteHexFunctions
    {
        internal static GABitByteHexLimits[] InitializeBitString(DesignSpaceDescription discreteSpaceDescriptor)
        {
            var result = new GABitByteHexLimits[discreteSpaceDescriptor.n];
            var currentIndex = 0;
            for (int i = 0; i < discreteSpaceDescriptor.n; i++)
            {
                if (discreteSpaceDescriptor.VariableDescriptors[i].Discrete)
                {
                    var maxValue = discreteSpaceDescriptor.MaxVariableSizes[i];
                    var numberBits = (int)(Math.Log(maxValue, 2)) + 1;
                    var endIndex = currentIndex + numberBits;
                    result[i] = new GABitByteHexLimits()
                                    {
                                        StartIndex = currentIndex,
                                        EndIndex = endIndex,
                                        MaxValue = maxValue
                                    };
                    currentIndex = endIndex;
                }
                else result[i] = new GABitByteHexLimits();
            }
            return result;
        }

        internal static int FindVariableIndex(GABitByteHexLimits[] limits, int i)
        {
            return Array.FindIndex(limits, a => a.EndIndex > i);
        }

        internal static BitArray Encode(long value, int length)
        {
            var result = new BitArray(length);
            var denominator = (long)Math.Pow(2, length - 1);
            for (int i = length - 1; i >= 0; i--)
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
            for (int i = 0; i < b.Count; i++)
            {
                if (b[i]) result += factor;
                factor *= 2;
            }
            if (result > maxValue) return result - maxValue;
            return result;
        }

    }
}