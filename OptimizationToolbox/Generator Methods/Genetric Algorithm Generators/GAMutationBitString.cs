// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="GAMutationBitString.cs" company="OptimizationToolbox">
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
    /// Class GAMutationBitString.
    /// Implements the <see cref="OptimizationToolbox.GeneticMutationGenerator" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.GeneticMutationGenerator" />
    public class GAMutationBitString : GeneticMutationGenerator
    {
        /// <summary>
        /// The bit string length
        /// </summary>
        private readonly int bitStringLength;
        /// <summary>
        /// The limits
        /// </summary>
        private readonly BitByteHexLimits[] limits;
        /// <summary>
        /// The m rate per bit
        /// </summary>
        private readonly double mRatePerBit;
        /// <summary>
        /// The random
        /// </summary>
        private readonly Random rnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="GAMutationBitString"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        /// <param name="mutationRate">The mutation rate.</param>
        public GAMutationBitString(DesignSpaceDescription discreteSpaceDescriptor, double mutationRate = 0.1)
            : base(discreteSpaceDescriptor)
        {
            limits = BitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            mRatePerBit = mutationRate / bitStringLength;
            rnd = new Random();
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="control">The control.</param>
        public override void GenerateCandidates(ref List<ICandidate> candidates, int control = -1)
        {
            for (var i = candidates.Count - 1; i >= 0; i--)
            {
                var candidate = candidates[i].x;
                var ChangeMade = false;
                for (var j = 0; j < bitStringLength; j++)
                    if (rnd.NextDouble() < mRatePerBit)
                    {
                        ChangeMade = true;
                        var varIndex = BitByteHexFunctions.FindVariableIndex(limits, j);
                        var valueIndex = discreteSpaceDescriptor[varIndex].PositionOf(candidate[varIndex]);
                        valueIndex = BitByteHexFunctions.FlipBit(valueIndex, limits[varIndex], j);
                        candidate[varIndex] = discreteSpaceDescriptor[varIndex][valueIndex];
                    }
                if (ChangeMade)
                {
                    candidates.RemoveAt(i);
                    candidates.Add(new Candidate(double.NaN, candidate));
                }
            }
        }
    }
}