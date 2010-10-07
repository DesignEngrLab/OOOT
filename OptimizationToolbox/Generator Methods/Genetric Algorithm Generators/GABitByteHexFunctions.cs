using System;
using System.Collections;

namespace OptimizationToolbox
{

    internal struct GABitByteHexLimits
    {
        public long StartIndex;
        public long EndIndex;
        public long MaxValue;

    }

    internal static class GABitByteHexFunctions
    {
        internal static GABitByteHexLimits[] InitializeBitString(DiscreteSpaceDescriptor discreteSpaceDescriptor)
        {
            var result = new GABitByteHexLimits[discreteSpaceDescriptor.n];
            long currentIndex = 0;
            for (int i = 0; i < discreteSpaceDescriptor.n; i++)
            {
                if (discreteSpaceDescriptor.VariableDescriptors[i].Discrete)
                {
                    var maxValue = discreteSpaceDescriptor.MaxVariableSizes[i];
                    var numberBits = (long)Math.Log(maxValue, 2);
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

        internal static int FindVariableIndex(GABitByteHexLimits[] limits, long i)
        {
            int result = 0;
            while (limits[result].StartIndex > i)
            {
                result++;
            }
            return result;
        }

        internal static long FlipBit(long initValue, GABitByteHexLimits limits, long bitIndex)
        {
            bitIndex -= limits.StartIndex;
            BitArray b = Encode(initValue, limits.EndIndex - limits.StartIndex);
            b.Set((int)bitIndex, !b[(int)bitIndex]);
            return Decode(b, limits.MaxValue);
        }
        internal static BitArray Encode(long value, long length)
        {
            if (length > int.MaxValue)
                throw new Exception("Currently BitArray can only have up to " + int.MaxValue +
                                    "members (GABitByteHexFunctions in Encode function).");
            var result = new BitArray((int)length);
            var denominator = (long)Math.Pow(2, length - 1);
            for (int i = (int)length - 1; i >= 0; i--)
            {
                if (value >= denominator)
                {
                    result[i] = true;
                    value -= denominator;
                }
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


        internal static void CrossoverBitString(BitArray c1BitArray, BitArray c2BitArray, long position,
            long maxValue, out long c1Value, out long c2Value)
        {
            for (int i = (int)position; i < (int)c1BitArray.Count; i++)
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