// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="slackSquaredEqualityFromInequality.cs" company="OptimizationToolbox">
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
namespace OptimizationToolbox
{
    /// <summary>
    /// Class slackSquaredEqualityFromInequality.
    /// Implements the <see cref="OptimizationToolbox.IEquality" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IEquality" />
    internal class slackSquaredEqualityFromInequality : IEquality
    {
        /// <summary>
        /// The former ineq
        /// </summary>
        private readonly IInequality formerIneq;
        /// <summary>
        /// The slack index
        /// </summary>
        private readonly int slackIndex;
        /// <summary>
        /// The opt method
        /// </summary>
        private readonly abstractOptMethod optMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="slackSquaredEqualityFromInequality"/> class.
        /// </summary>
        /// <param name="formerIneq">The former ineq.</param>
        /// <param name="slackIndex">Index of the slack.</param>
        /// <param name="optMethod">The opt method.</param>
        public slackSquaredEqualityFromInequality(IInequality formerIneq, int slackIndex, abstractOptMethod optMethod)
        {
            this.formerIneq = formerIneq;
            this.slackIndex = slackIndex;
            this.optMethod = optMethod;
        }

        #region Implementation of IOptFunction

        /// <summary>
        /// Gets or sets the h.
        /// </summary>
        /// <value>The h.</value>
        public double h { get; set; }
        /// <summary>
        /// Gets or sets the find deriv by.
        /// </summary>
        /// <value>The find deriv by.</value>
        public differentiate findDerivBy { get; set; }
        /// <summary>
        /// Gets the number evals.
        /// </summary>
        /// <value>The number evals.</value>
        public int numEvals { get; private set; }
        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public double calculate(double[] x)
        {
            return optMethod.calculate(formerIneq,x) + x[slackIndex] * x[slackIndex];
        }
        #endregion
    }
}