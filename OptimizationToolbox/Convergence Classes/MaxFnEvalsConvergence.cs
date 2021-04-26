// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="MaxFnEvalsConvergence.cs" company="OptimizationToolbox">
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
    /// Given a value Kmax, this criteria will return true if the process reaches this many iterations.
    /// </summary>
    public class MaxFnEvalsConvergence : abstractConvergence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxIterationsConvergence" /> class.
        /// </summary>
        public MaxFnEvalsConvergence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxIterationsConvergence" /> class.
        /// </summary>
        /// <param name="maxFunctionEvaluations">The max function evaluations.</param>
        public MaxFnEvalsConvergence(long maxFunctionEvaluations)
        {
            this.maxFunctionEvaluations = maxFunctionEvaluations;
        }

        /// <summary>
        /// Gets or sets the maximum number of function evaluations.
        /// </summary>
        /// <value>The max function evaluations.</value>
        public long maxFunctionEvaluations { get; set; }

        /// <summary>
        /// Given a value Kmax, this criteria will return true if the process reaches this many function evaluations.
        /// </summary>
        /// <param name="iteration">The number of iterations (not used).</param>
        /// <param name="numFnEvals">The number of function evaluations</param>
        /// <param name="fBest">The best f (not used).</param>
        /// <param name="xBest">The best x (not used).</param>
        /// <param name="population">The population of candidates (not used).</param>
        /// <param name="gradF">The gradient of F (not used).</param>
        /// <returns>true or false - has the process converged?</returns>
        /// <exception cref="Exception">MaxIterationsConvergence expected a positive value for the first argument, YInteger</exception>
        public override bool converged(long iteration = -1, long numFnEvals = -1, double fBest = double.NaN, IList<double> xBest = null, IList<double[]> population = null, IList<double> gradF = null)
        {
            if (numFnEvals < 0)
                throw new Exception(
                    "MaxIterationsConvergence expected a positive value for the first argument, YInteger");
            return (numFnEvals >= maxFunctionEvaluations);
        }
    }
}