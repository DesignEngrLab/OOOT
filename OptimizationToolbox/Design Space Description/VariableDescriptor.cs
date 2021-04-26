// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="VariableDescriptor.cs" company="OptimizationToolbox">
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
using System.Linq;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class VariableDescriptor.
    /// </summary>
    public class VariableDescriptor
    {

        /* both real and discrete numbers can have both upper and lower limits.
         * Discrete require values less than infinity, but for reals, it may be infinity. */

        /* the following three are only for discrete numbers. */
        /// <summary>
        /// The delta
        /// </summary>
        private double delta = double.NaN;
        /// <summary>
        /// The lower bound
        /// </summary>
        private double lowerBound = double.NegativeInfinity;

        /// <summary>
        /// The size
        /// </summary>
        private long size = long.MinValue;
        /// <summary>
        /// The upper bound
        /// </summary>
        private double upperBound = double.PositiveInfinity;

        /// <summary>
        /// The values
        /// </summary>
        private double[] values;

        //private double realValue;
        //private long currentIndex;


        /// <summary>
        /// Initializes a new instance of the <see cref="VariableDescriptor"/> class.
        /// </summary>
        public VariableDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableDescriptor"/> class.
        /// </summary>
        /// <param name="LowerBound">The lower bound.</param>
        /// <param name="UpperBound">The upper bound.</param>
        public VariableDescriptor(double LowerBound, double UpperBound)
            : this()
        {
            lowerBound = LowerBound;
            upperBound = UpperBound;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableDescriptor"/> class.
        /// </summary>
        /// <param name="LowerBound">The lower bound.</param>
        /// <param name="UpperBound">The upper bound.</param>
        /// <param name="Delta">The delta.</param>
        public VariableDescriptor(double LowerBound, double UpperBound, double Delta)
            : this(LowerBound, UpperBound)
        {
            defineRemainingDiscreteValues(1 + (long)((UpperBound - LowerBound) / Delta), Delta);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableDescriptor"/> class.
        /// </summary>
        /// <param name="LowerBound">The lower bound.</param>
        /// <param name="UpperBound">The upper bound.</param>
        /// <param name="Size">The size.</param>
        public VariableDescriptor(double LowerBound, double UpperBound, long Size)
            : this(LowerBound, UpperBound)
        {
            defineRemainingDiscreteValues(Size, (upperBound - lowerBound) / Size);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableDescriptor"/> class.
        /// </summary>
        /// <param name="Values">The values.</param>
        public VariableDescriptor(double[] Values)
        {
            values = Values;
            defineBasedOnValues();
        }

        /// <summary>
        /// Gets or sets the lower bound.
        /// </summary>
        /// <value>The lower bound.</value>
        public double LowerBound
        {
            get { return lowerBound; }
            set { lowerBound = value; }
        }

        /// <summary>
        /// Gets or sets the upper bound.
        /// </summary>
        /// <value>The upper bound.</value>
        public double UpperBound
        {
            get { return upperBound; }
            set { upperBound = value; }
        }

        /// <summary>
        /// Gets or sets the delta.
        /// </summary>
        /// <value>The delta.</value>
        public double Delta
        {
            get { return delta; }
            set
            {
                delta = value;
                if (!double.IsNaN(delta))
                    defineRemainingDiscreteValues(1 + (long)((upperBound - lowerBound) / delta), delta);
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size
        {
            get { return size; }
            set
            {
                size = value;
                defineRemainingDiscreteValues(size, (upperBound - lowerBound) / size);
            }
        }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
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
        /// Gets or sets a value indicating whether this <see cref="VariableDescriptor" /> is discrete.
        /// </summary>
        /// <value><c>true</c> if discrete; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public Boolean Discrete { get; private set; }

        /// <summary>
        /// Gets the <see cref="System.Double"/> with the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>System.Double.</returns>
        public double this[long position]
        {
            get
            {
                if (!Discrete) return double.NaN;
                if (Values != null) return Values[position];
                return LowerBound + position * Delta;
            }
        }

        /// <summary>
        /// Defines the remaining discrete values.
        /// </summary>
        /// <param name="newSize">The new size.</param>
        /// <param name="newDelta">The new delta.</param>
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

        /// <summary>
        /// Defines the based on values.
        /// </summary>
        private void defineBasedOnValues()
        {
            size = Values.GetLength(0);
            lowerBound = Values.Min();
            upperBound = Values.Max();
            delta = double.NaN;
            Discrete = true;
        }

        /// <summary>
        /// Positions the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int64.</returns>
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