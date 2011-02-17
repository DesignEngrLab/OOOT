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
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationToolbox
{
    public class RandomPairwiseCompare : abstractSelector
    {
        private readonly Random rnd;

        public RandomPairwiseCompare(optimize direction)
            : base(direction)
        {
            rnd = new Random();
        }

        public override void selectCandidates(ref List<Candidate> candidates,
                                              double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int)(candidates.Count * fractionToKeep);
            randomizeList(ref candidates);
            /* maxLoops was created in the off chance that the population stagnates all at the same
             * objective function value. It is unlikely that the process should exist on this account
             * but it has happened. Ergo, we put in this condition so that the process doesn't hang here.*/
            var maxLoops = 3 * candidates.Count;
            while ((candidates.Count > numKeep) && (maxLoops-- > 0))
            {
                var contestantA = candidates[0];
                candidates.RemoveAt(0);
                var contestantB = candidates[0];
                candidates.RemoveAt(0);
                if (betterThan(contestantA.fValues[0], contestantB.fValues[0]))
                    candidates.Add(contestantA);
                else if (betterThan(contestantB.fValues[0], contestantA.fValues[0]))
                    candidates.Add(contestantB);
                else
                {
                    /* if it's a tie, add them both to the list, but in off-chance that this match is played
                     * again, contestantB is added at a random location in the list. */
                    candidates.Add(contestantA);
                    candidates.Insert(rnd.Next(candidates.Count), contestantB);
                }
            }
        }


        /// <summary>
        /// Randomizes the list.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        protected void randomizeList(ref List<Candidate> candidates)
        {
            var r = new Random();
            candidates = candidates.OrderBy(a => r.NextDouble()).ToList();
        }
    }
}