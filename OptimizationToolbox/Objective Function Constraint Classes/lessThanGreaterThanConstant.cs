// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="lessThanGreaterThanConstant.cs" company="OptimizationToolbox">
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
    /// Class inequalityWithConstant.
    /// Implements the <see cref="OptimizationToolbox.IDifferentiable" />
    /// Implements the <see cref="OptimizationToolbox.IInequality" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IDifferentiable" />
    /// <seealso cref="OptimizationToolbox.IInequality" />
    public abstract class inequalityWithConstant : IDifferentiable, IInequality
    {
        /// <summary>
        /// Gets or sets the constant.
        /// </summary>
        /// <value>The constant.</value>
        public double constant { get; set; }
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int index { get; set; }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="inequalityWithConstant"/> class.
        /// </summary>
        protected inequalityWithConstant()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="inequalityWithConstant"/> class.
        /// </summary>
        /// <param name="constant">The constant.</param>
        /// <param name="index">The index.</param>
        protected inequalityWithConstant(double constant, int index)
        {
            this.constant = constant;
            this.index = index;
        }

        #endregion

        #region Implementation of IDifferentiable

        /// <summary>
        /// Derivs the WRT xi.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Double.</returns>
        public abstract double deriv_wrt_xi(double[] x, int i);

        #endregion

        #region Implementation of IOptFunction
        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public abstract double calculate(double[] x);

        #endregion
    }

    /// <summary>
    /// Class lessThanConstant.
    /// Implements the <see cref="OptimizationToolbox.inequalityWithConstant" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.inequalityWithConstant" />
    public class lessThanConstant : inequalityWithConstant
    {

        /// <summary>
        /// Derivs the WRT xi.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Double.</returns>
        public override double deriv_wrt_xi(double[] x, int i)
        {
            if (i == index) return 1.0;
            return 0.0;
        }

        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public override double calculate(double[] x)
        {
            return x[index] - constant;
        }
    }

    /// <summary>
    /// Class greaterThanConstant.
    /// Implements the <see cref="OptimizationToolbox.inequalityWithConstant" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.inequalityWithConstant" />
    public class greaterThanConstant : inequalityWithConstant
    {
        /// <summary>
        /// Derivs the WRT xi.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Double.</returns>
        public override double deriv_wrt_xi(double[] x, int i)
        {
            if (i == index) return -1.0;
            return 0.0;
        }

        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public override double calculate(double[] x)
        {
            return constant - x[index];
        }
    }
}