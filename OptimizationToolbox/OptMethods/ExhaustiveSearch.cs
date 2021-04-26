// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="ExhaustiveSearch.cs" company="OptimizationToolbox">
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

namespace OptimizationToolbox
{
    /// <summary>
    /// Class ExhaustiveSearch.
    /// Implements the <see cref="OptimizationToolbox.abstractOptMethod" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractOptMethod" />
    public class ExhaustiveSearch : abstractOptMethod
    {
        /// <summary>
        /// The time predition index
        /// </summary>
        public static int timePreditionIndex = 1000;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ExhaustiveSearch"/> class.
        /// </summary>
        /// <param name="SpaceDescription">The space description.</param>
        /// <param name="direction">The direction.</param>
        /// <exception cref="Exception">Exhaustive Search can only be used when Space is all discrete</exception>
        public ExhaustiveSearch(DesignSpaceDescription SpaceDescription, optimize direction)
        {
            spaceDescription = SpaceDescription;
            if (!SpaceDescription.AllDiscrete)
                throw new Exception("Exhaustive Search can only be used when Space is all discrete");
            comparer = new optimizeSort(direction);
            RequiresObjectiveFunction = false;
            ConstraintsSolvedWithPenalties = false;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = false;
            RequiresConvergenceCriteria = false;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = true;
        }

        #endregion

        /// <summary>
        /// The comparer
        /// </summary>
        private readonly optimizeSort comparer;
        /// <summary>
        /// The space description
        /// </summary>
        private readonly DesignSpaceDescription spaceDescription;
        /// <summary>
        /// The current indices
        /// </summary>
        private long[] CurrentIndices;

        /// <summary>
        /// Runs the specified optimization method. This includes the details
        /// of the optimization method.
        /// </summary>
        /// <param name="xStar">The x star.</param>
        /// <returns>System.Double.</returns>
        protected override double run(out double[] xStar)
        {
            var startTime = DateTime.Now;
            fStar = double.PositiveInfinity;
            CurrentIndices = new long[n];
            x = spaceDescription.GetVariableVector(CurrentIndices);
            xStar = x;
            if (feasible(x)) fStar = calc_f(x);
            while (notConverged(k++, numEvals, fStar, xStar) && IncrementIndices())
            {
                if (k == timePreditionIndex) performTimePrediction(startTime);
                x = spaceDescription.GetVariableVector(CurrentIndices);
                var fNew = calc_f(x);
                if ((feasible(x)) && (!comparer.BetterThan(fStar, fNew)))
                {
                    fStar = fNew;
                    xStar = x;
                }
            }
            return fStar;
        }

        /// <summary>
        /// Performs the time prediction.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        private void performTimePrediction(DateTime startTime)
        {
            double span = (DateTime.Now - startTime).Ticks;
            span /= timePreditionIndex;
            span *= spaceDescription.SizeOfSpace;
            var endTime = startTime + new TimeSpan((long)span);
            SearchIO.output(timePreditionIndex+" states evaluated. "+ spaceDescription.SizeOfSpace + 
                " total states ("+100*timePreditionIndex/spaceDescription.SizeOfSpace
                +"% complete)");
            SearchIO.output("Predicted time for the process to end:\n" + endTime);
        }


        /// <summary>
        /// Increments the indices.
        /// </summary>
        /// <param name="IndicesIndex">Index of the indices.</param>
        /// <returns>Boolean.</returns>
        private Boolean IncrementIndices(int IndicesIndex = 0)
        {
            if (IndicesIndex == n) return false;
            CurrentIndices[IndicesIndex]++;
            if (CurrentIndices[IndicesIndex] >= spaceDescription.MaxVariableSizes[IndicesIndex])
            {
                CurrentIndices[IndicesIndex] = 0;
                return IncrementIndices(IndicesIndex + 1);
            }
            return true;
        }
    }
}