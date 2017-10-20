﻿/*************************************************************************
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
using System.Linq;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    public class VariableDescriptor
    {

        /* both real and discrete numbers can have both upper and lower limits.
         * Discrete require values less than infinity, but for reals, it may be infinity. */

        /* the following three are only for discrete numbers. */
        private double delta = double.NaN;
        private double lowerBound = double.NegativeInfinity;

        private long size = long.MinValue;
        private double upperBound = double.PositiveInfinity;

        private double[] values;

        //private double realValue;
        //private long currentIndex;


        public VariableDescriptor()
        {
        }

        public VariableDescriptor(double LowerBound, double UpperBound)
            : this()
        {
            lowerBound = LowerBound;
            upperBound = UpperBound;
        }

        public VariableDescriptor(double LowerBound, double UpperBound, double Delta)
            : this(LowerBound, UpperBound)
        {
            defineRemainingDiscreteValues(1 + (long)((UpperBound - LowerBound) / Delta), Delta);
        }

        public VariableDescriptor(double LowerBound, double UpperBound, long Size)
            : this(LowerBound, UpperBound)
        {
            defineRemainingDiscreteValues(Size, (upperBound - lowerBound) / Size);
        }

        public VariableDescriptor(double[] Values)
        {
            values = Values;
            defineBasedOnValues();
        }

        public double LowerBound
        {
            get { return lowerBound; }
            set { lowerBound = value; }
        }

        public double UpperBound
        {
            get { return upperBound; }
            set { upperBound = value; }
        }

        public double Delta
        {
            get { return delta; }
            set
            {
                delta = value;
                defineRemainingDiscreteValues(1 + (long)((upperBound - lowerBound) / delta), delta);
            }
        }

        public long Size
        {
            get { return size; }
            set
            {
                size = value;
                defineRemainingDiscreteValues(size, (upperBound - lowerBound) / size);
            }
        }

        public double[] Values
        {
            get { return values; }
            set
            {
                values = value;
                defineBasedOnValues();
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref = "VariableDescriptor" /> is discrete.
        /// </summary>
        /// <value><c>true</c> if discrete; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public Boolean Discrete { get; private set; }

        public double this[long position]
        {
            get
            {
                if (!Discrete) return double.NaN;
                if (Values != null) return Values[position];
                return LowerBound + position * Delta;
            }
        }

        private void defineRemainingDiscreteValues(long newSize, double newDelta)
        {
            size = newSize;
            delta = newDelta;
            if (newSize < Parameters.DiscreteVariableMaxToStoreImplicitly)
            {
                values = new double[newSize];
                values[0] = lowerBound;
                for (var i = 1; i < Size; i++)
                    values[i] = values[i - 1] + newDelta;
                delta = double.NaN;
            }
            //realValue = double.NaN;
            Discrete = true;
        }

        private void defineBasedOnValues()
        {
            size = Values.GetLength(0);
            lowerBound = Values.Min();
            upperBound = Values.Max();
            delta = double.NaN;
            Discrete = true;
        }

        public long PositionOf(double value)
        {
            if (!Discrete) return -1;
            if (Values != null)
            {
                var result = Array.IndexOf(Values, value);
                if (result != -1) return result;
                var minDifference = Values.Min(x => Math.Abs(x - value));
                for (int j = 0; j < size; j++)
                    if (Math.Abs(Values[j] - value) == minDifference)
                        return j;
            }
            var i = (value - LowerBound) / Delta;
            if (i - Math.Floor(i) / Delta < Parameters.ToleranceForSame) return (long)i;
            return -1;
        }
    }
}