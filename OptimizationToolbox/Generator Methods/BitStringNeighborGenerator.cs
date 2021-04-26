// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="BitStringNeighborGenerator.cs" company="OptimizationToolbox">
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
    /// Class BitStringNeighborGenerator.
    /// Implements the <see cref="OptimizationToolbox.abstractGenerator" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractGenerator" />
    public class BitStringNeighborGenerator : abstractGenerator
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
        /// The random
        /// </summary>
        private readonly Random rnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitStringNeighborGenerator"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        public BitStringNeighborGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
            limits = BitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            rnd = new Random();
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="numToCreate">The number to create.</param>
        /// <returns>List&lt;System.Double[]&gt;.</returns>
        public override List<double[]> GenerateCandidates(double[] candidate, int numToCreate = -1)
        {
            var newCands = new List<double[]>();
            if (numToCreate == -1) numToCreate = 1;
            while (numToCreate-- > 0)
            {
                var result = (double[])candidate.Clone();
                var j = rnd.Next(bitStringLength);
                var varIndex = BitByteHexFunctions.FindVariableIndex(limits, j);
                var valueIndex = discreteSpaceDescriptor[varIndex].PositionOf(result[varIndex]);
                valueIndex = BitByteHexFunctions.FlipBit(valueIndex, limits[varIndex], j);
                result[varIndex] = discreteSpaceDescriptor[varIndex][valueIndex];
                newCands.Add(result);
            }
            return newCands;
        }
    }
}