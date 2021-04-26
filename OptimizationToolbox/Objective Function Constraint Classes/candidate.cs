// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="candidate.cs" company="OptimizationToolbox">
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
    /// Interface ICandidate
    /// </summary>
    public interface ICandidate
    {
        /// <summary>
        /// Gets or sets the f values.
        /// </summary>
        /// <value>The f values.</value>
        double[] objectives { get; set; }

        /// <summary>
        /// Gets or sets the g values.
        /// </summary>
        /// <value>The g values.</value>
        double[] gValues { get; set; }

        /// <summary>
        /// Gets or sets the h values.
        /// </summary>
        /// <value>The h values.</value>
        double[] hValues { get; set; }

        /// <summary>
        /// Gets or sets the x vector - the vector of design variables.
        /// </summary>
        /// <value>The x.</value>
        double[] x { get; set; }
    }

    /// <summary>
    /// Class Candidate.
    /// Implements the <see cref="OptimizationToolbox.ICandidate" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.ICandidate" />
    public class Candidate : ICandidate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Candidate" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="evaluationMethods">the evaluation methods</param>
        public Candidate(double[] x, abstractOptMethod evaluationMethods = null)
        {
            this.x = (double[])x.Clone();
            if (evaluationMethods != null)
            {
                //                evaluationMethods
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Candidate"/> class.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="x">The x.</param>
        public Candidate(double f, double[] x)
        {
            this.objectives = new[] { f };
            this.x = (double[])x.Clone();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Candidate"/> class.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="x">The x.</param>
        public Candidate(double[] f, double[] x)
        {
            this.objectives = (double[])f.Clone();
            this.x = (double[])x.Clone();
        }

        /// <summary>
        /// Gets or sets the f values.
        /// </summary>
        /// <value>The f values.</value>
        public double[] objectives { get; set; }

        /// <summary>
        /// Gets or sets the g values.
        /// </summary>
        /// <value>The g values.</value>
        public double[] gValues { get; set; }

        /// <summary>
        /// Gets or sets the h values.
        /// </summary>
        /// <value>The h values.</value>
        public double[] hValues { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public double[] x { get; set; }
    }
}
