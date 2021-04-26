// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="Parameters.cs" company="OptimizationToolbox">
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
    /// Class Parameters.
    /// </summary>
    public static class Parameters
    {
        /// <summary>
        /// This is the golden ratio. it is equal to (sqrt(5)-1)/2
        /// </summary>
        internal const double Golden62 = 0.61803398874989484820458683436564;
        /// <summary>
        /// This term is simply 1-Golden62
        /// </summary>
        internal const double Golden38 = 1 - Golden62;
        /// <summary>
        /// The maximum number of previous function evaluation to store. Too big means
        /// too much memory and time searching for past evalutions. Too small means
        /// re-evaluating candidates that have been previously visited.
        /// </summary>
        public static int MaxFunctionDataStore = 400;
        /// <summary>
        /// The past function storage is cleaned out when it reaches the limit provided
        /// by MaxFunctionDataStore. This could be one, but that means the clean operation
        /// happens every step (default is 20).
        /// </summary>
        public static int FunctionStoreCleanOutStepDown = 20;
        /// <summary>
        /// Used when defining a discrete space descriptor. If the space is smaller than this
        /// then the values are stored in an explicit vector, else they are calculated on the fly. discrete variable maximum to store implicitly
        /// </summary>
        public static int DiscreteVariableMaxToStoreImplicitly = 5000;

        /// <summary>
        /// Gets or sets the verbosity.
        /// </summary>
        /// <value>The verbosity.</value>
        public static VerbosityLevels Verbosity { get; set; }

        /// <summary>
        /// The tolerance for same
        /// </summary>
        public static double ToleranceForSame = 1e-15;
        /// <summary>
        /// The default finite difference step size
        /// </summary>
        public static double DefaultFiniteDifferenceStepSize = 0.1;
        /// <summary>
        /// The default finite difference mode
        /// </summary>
        public static differentiate DefaultFiniteDifferenceMode = differentiate.Central2;
    } 
}