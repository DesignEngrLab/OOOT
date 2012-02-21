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

using System.Collections.Generic;

namespace OptimizationToolbox
{
    /// <summary>
    /// The class that all selector classes must inherit from. 
    /// </summary>
    public abstract class abstractMOSelector
    {
        /// <summary>
        /// the direction of the search: maximizing or minimizing
        /// </summary>
        protected readonly optimize[] optDirections;


        protected readonly int numObjectives;

        /// <summary>
        /// Initializes a new instance of the <see cref="abstractSelector"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        protected abstractMOSelector(int numObjectives, optimize[] optDirections)
        {
            this.numObjectives = numObjectives;
            if (optDirections == null)
            {
                optDirections = new optimize[numObjectives];
                for (int i = 0; i < numObjectives; i++)
                    optDirections[i] = optimize.minimize;
            }
            else optDirections = (optimize[])optDirections.Clone();
        }

        /// <summary>
        /// Selects the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="control">The control.</param>
        public abstract void selectCandidates(ref List<Candidate> candidates,
                                              double control = double.NaN);


    }
}