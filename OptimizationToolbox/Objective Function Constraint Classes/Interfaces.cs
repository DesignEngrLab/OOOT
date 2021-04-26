// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="Interfaces.cs" company="OptimizationToolbox">
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
    /// Interface IDifferentiable
    /// </summary>
    public interface IDifferentiable
    {
        /// <summary>
        /// Derivs the WRT xi.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Double.</returns>
        double deriv_wrt_xi(double[] x, int i);
    }

    /// <summary>
    /// Interface ITwiceDifferentiable
    /// </summary>
    public interface ITwiceDifferentiable
    {
        /// <summary>
        /// Seconds the deriv WRT ij.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="i">The i.</param>
        /// <param name="j">The j.</param>
        /// <returns>System.Double.</returns>
        double second_deriv_wrt_ij(double[] x, int i, int j);
    }

    /// <summary>
    /// Interface IOptFunction
    /// </summary>
    public interface IOptFunction
    {
        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        double calculate(double[] x);
    }
    /// <summary>
    /// Interface IObjectiveFunction
    /// Implements the <see cref="OptimizationToolbox.IOptFunction" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IOptFunction" />
    public interface IObjectiveFunction : IOptFunction { }
    /// <summary>
    /// Interface IConstraint
    /// Implements the <see cref="OptimizationToolbox.IOptFunction" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IOptFunction" />
    public interface IConstraint : IOptFunction{}
    /// <summary>
    /// Interface IEquality
    /// Implements the <see cref="OptimizationToolbox.IConstraint" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IConstraint" />
    public interface IEquality : IConstraint { }
    /// <summary>
    /// Interface IInequality
    /// Implements the <see cref="OptimizationToolbox.IConstraint" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IConstraint" />
    public interface IInequality : IConstraint { }

    /// <summary>
    /// Interface IDependentAnalysis
    /// </summary>
    public interface IDependentAnalysis
    {
        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        void calculate(double[] x);
    }
}
