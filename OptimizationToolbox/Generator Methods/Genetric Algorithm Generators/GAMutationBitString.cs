﻿/*************************************************************************
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
    public class GAMutationBitString : GeneticMutationGenerator
    {
        private readonly int bitStringLength;
        private readonly BitByteHexLimits[] limits;
        private readonly double mRatePerBit;
        private readonly Random rnd;

        public GAMutationBitString(DesignSpaceDescription discreteSpaceDescriptor, double mutationRate = 0.1)
            : base(discreteSpaceDescriptor)
        {
            limits = BitByteHexFunctions.InitializeBitString(discreteSpaceDescriptor);
            bitStringLength = limits[n - 1].EndIndex;
            mRatePerBit = mutationRate / bitStringLength;
            rnd = new Random();
        }

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