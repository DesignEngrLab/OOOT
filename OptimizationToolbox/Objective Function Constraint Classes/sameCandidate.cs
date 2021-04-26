// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="sameCandidate.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{

    /// <summary>
    /// Class sameCandidate.
    /// Implements the <see cref="System.Collections.Generic.IEqualityComparer{System.Double[]}" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{System.Double[]}" />
    class sameCandidate : IEqualityComparer<double[]>
    {
        /// <summary>
        /// The tolerance squared
        /// </summary>
        private readonly double toleranceSquared;
        /// <summary>
        /// Initializes a new instance of the <see cref="sameCandidate"/> class.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        public sameCandidate(double tolerance)
        {
            toleranceSquared = tolerance * tolerance;
        }

        #region Implementation of IEqualityComparer<double[]>

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first candidate x array to compare.</param>
        /// <param name="y">The second candidate x array to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(double[] x, double[] y)
        {
            if ((x == null) || (y == null) || (x.GetLength(0) != y.GetLength(0))) return false;
            if (x.Equals(y)) return true;
            //return false;
            return (x.norm2(y, true) <= toleranceSquared);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.</exception>
        public int GetHashCode(double[] obj)
        {
            return -1; // obj.GetHashCode();
        }

        #endregion
    }
}
