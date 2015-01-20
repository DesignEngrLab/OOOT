/*************************************************************************
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
namespace OptimizationToolbox
{
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
        public static int MaxFunctionDataStore = 1000;
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

        public static VerbosityLevels Verbosity { get; set; }

        public static double ToleranceForSame = 1e-15;
        public static double DefaultFiniteDifferenceStepSize = 0.1;
        public static differentiate DefaultFiniteDifferenceMode = differentiate.Central2;
    } 
}