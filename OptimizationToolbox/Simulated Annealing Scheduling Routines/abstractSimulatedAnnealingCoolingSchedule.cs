﻿/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System.Collections.Generic;

namespace OptimizationToolbox
{
    /// <summary>
    /// the class that all simulated annealing cooling schedules must inherit from.
    /// </summary>
    public abstract class abstractSimulatedAnnealingCoolingSchedule
    {
        /// <summary>
        /// the number of samples to take in determining the temperature.
        /// </summary>
        protected readonly int samplesInGeneration;
        /// <summary>
        /// the reference back to the entire simulated annealing optimization method.
        /// </summary>
        protected abstractOptMethod optMethod;
        /// <summary>
        /// number of samples taken thus far
        /// </summary>
        protected int samplesThusFar;

        /// <summary>
        /// Initializes a new instance of the <see cref="abstractSimulatedAnnealingCoolingSchedule"/> class.
        /// </summary>
        /// <param name="samplesInGeneration">The samples in generation.</param>
        protected abstractSimulatedAnnealingCoolingSchedule(int samplesInGeneration)
        {
            this.samplesInGeneration = samplesInGeneration;
        }

        /// <summary>
        /// Sets the optimization details.
        /// </summary>
        /// <param name="optMethod">The opt method.</param>
        public void SetOptimizationDetails(abstractOptMethod optMethod)
        {
            this.optMethod = optMethod;
        }

        internal abstract double SetInitialTemperature();
        internal abstract double UpdateTemperature(double temperature, List<Candidate> candidates);
    }
}