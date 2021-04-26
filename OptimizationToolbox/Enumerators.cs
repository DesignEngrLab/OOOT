// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="Enumerators.cs" company="OptimizationToolbox">
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
    /* Enumerator for Search functions that have generality 
     * to either minimize or maximize (e.g. PNPPS, stochasticChoose). */

    /// <summary>
    /// the direction of the optimization
    /// </summary>
    public enum optimize
    {
        /// <summary>
        /// The minimize
        /// </summary>
        minimize = -1,
        /// <summary>
        /// The neither
        /// </summary>
        neither = 0,
        /// <summary>
        /// The maximize
        /// </summary>
        maximize = 1
    } ;

    /* This enumerator is used primarily by the parameter tuning where suboptimization
     * functions (that derive from the Optimization Toolbox's abstractOptFunction) need
     * to determine how derivatives will be calculated. */

    /// <summary>
    /// The types of numerical differention (finite difference) supported by OOOT
    /// </summary>
    public enum differentiate
    {
        /// <summary>
        /// The analytic
        /// </summary>
        Analytic,
        /// <summary>
        /// The back1
        /// </summary>
        Back1,
        /// <summary>
        /// The forward1
        /// </summary>
        Forward1,
        /// <summary>
        /// The central2
        /// </summary>
        Central2,
        /// <summary>
        /// The back2
        /// </summary>
        Back2,
        /// <summary>
        /// The forward2
        /// </summary>
        Forward2,
        /// <summary>
        /// The central4
        /// </summary>
        Central4
    } ;

    /// <summary>
    /// Setting the Verbosity to one of these values changes the amount of output
    /// send to the Debug Listener. Lower values may speed up search
    /// </summary>
    public enum VerbosityLevels
    {
        /// <summary>
        /// The only critical
        /// </summary>
        OnlyCritical = 0,
        /// <summary>
        /// The low
        /// </summary>
        Low = 1,
        /// <summary>
        /// The below normal
        /// </summary>
        BelowNormal = 2,
        /// <summary>
        /// The normal
        /// </summary>
        Normal = 3,
        /// <summary>
        /// The above normal
        /// </summary>
        AboveNormal = 4,
        /// <summary>
        /// The everything
        /// </summary>
        Everything = int.MaxValue
    };
}