using System.Collections.Generic;
using System.Linq;
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
    public class Elitism : abstractSelector
    {
        public Elitism(optimize direction)
            : base(direction)
        {
        }

        public override void selectCandidates(ref List<Candidate> candidates,
                                              double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int)(candidates.Count * fractionToKeep);
            sort(ref candidates);
            candidates = candidates.Take(numKeep).ToList();
        }


        /// <summary>
        /// Sorts the specified candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        protected void sort(ref List<Candidate> candidates)
        {
            candidates = candidates.OrderBy(a => a.fValues[0], new optimizeSort(direction, true)).ToList();
        }
    }
}