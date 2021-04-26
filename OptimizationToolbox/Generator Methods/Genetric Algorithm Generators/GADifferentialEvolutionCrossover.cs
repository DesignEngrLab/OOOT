// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="GADifferentialEvolutionCrossover.cs" company="OptimizationToolbox">
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
    /// Class GADifferentialEvolutionCrossover.
    /// Implements the <see cref="OptimizationToolbox.GeneticCrossoverGenerator" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.GeneticCrossoverGenerator" />
    public class GADifferentialEvolutionCrossover : GeneticCrossoverGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GADifferentialEvolutionCrossover"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        public GADifferentialEvolutionCrossover(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="number">The number.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void GenerateCandidates(ref List<ICandidate> candidates, int number = -1)
        {
            throw new NotImplementedException();
        }
    }
}