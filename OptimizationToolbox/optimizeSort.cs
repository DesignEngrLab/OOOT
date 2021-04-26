// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="optimizeSort.cs" company="OptimizationToolbox">
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
using System.Collections.Generic;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class optimizeSort.
    /// Implements the <see cref="System.Collections.Generic.IComparer{System.Double}" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IComparer{System.Double}" />
    public class optimizeSort : IComparer<double>
    {
        /* an internal integer equal to the required sort direction. */
        /// <summary>
        /// The allow equal in sort
        /// </summary>
        private readonly Boolean AllowEqualInSort;
        /// <summary>
        /// The direction
        /// </summary>
        private readonly int direction;
        /* if using with SortedList, set AllowEqualInSorting to false, otherwise
         * it will crash when equal values are encountered. If using in Linq's 
         * OrderBy then the equal is need (AllowEqualInSorting = true) otherwise
         * the program will hang. */

        /// <summary>
        /// Initializes a new instance of the <see cref="optimizeSort"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="AllowEqualInSort">The allow equal in sort.</param>
        public optimizeSort(optimize direction, Boolean AllowEqualInSort = false)
        {
            this.direction = (int)direction;
            this.AllowEqualInSort = AllowEqualInSort;
        }

        #region IComparer<double> Members

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.
        /// Value
        /// Meaning
        /// Less than zero
        /// <paramref name="x" /> is less than <paramref name="y" />.
        /// Zero
        /// <paramref name="x" /> equals <paramref name="y" />.
        /// Greater than zero
        /// <paramref name="x" /> is greater than <paramref name="y" />.</returns>
        public int Compare(double x, double y)
        {
            if (AllowEqualInSort && (x == y)) return 0;
            /* in order to avoid the collections from throwing an error, we make sure
             * that only -1 or 1 is returned. If they are equal, we return +1 (when
             * minimizing). This makes newer items to the list appear before older items.
             * It is slightly more efficient than returning -1 and conforms with the 
             * philosophy of always exploring/preferring new concepts. See: SA's Metropolis Criteria. */
            if (direction * x < direction * y) return 1;
            return -1;
        }

        #endregion

        /// <summary>
        /// Betters the than.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Boolean.</returns>
        public Boolean BetterThan(double x, double y)
        {
            return (-1 == Compare(x, y));
        }
    }
}