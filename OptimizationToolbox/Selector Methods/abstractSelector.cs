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
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationToolbox
{
    /// <summary>
    /// The class that all selector classes must inherit from. 
    /// </summary>
    public abstract class abstractSelector
    {
        /// <summary>
        /// the direction of the search: maximizing or minimizing
        /// </summary>
        protected readonly optimize direction;
        private readonly optimizeSort directionComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="abstractSelector"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        protected abstractSelector(optimize direction)
        {
            this.direction = direction;
            directionComparer = new optimizeSort(direction, true);
        }

        /// <summary>
        /// Selects the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="control">The control.</param>
        public abstract void selectCandidates(ref List<KeyValuePair<double, double[]>> candidates,
                                              double control = double.NaN);

        /// <summary>
        /// Makes a random list of integers up to the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        protected static List<int> makeRandomIntList(int size)
        {
            var rnd = new Random();
            var result = Enumerable.Range(0, size).ToList();
            for (var i = 0; i < size; i++)
            {
                var value = result[i];
                result.RemoveAt(i);
                result.Insert(rnd.Next(result.Count), value);
            }
            return result;
        }


        /// <summary>
        /// Sorts the specified candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        protected void sort(ref List<KeyValuePair<double, double[]>> candidates)
        {
            candidates = candidates.OrderBy(a => a.Key, new optimizeSort(direction, true)).ToList();
        }

        /// <summary>
        /// Randomizes the list.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        protected void randomizeList(ref List<KeyValuePair<double, double[]>> candidates)
        {
            var r = new Random();
            candidates = candidates.OrderBy(a => r.NextDouble()).ToList();
        }

        /// <summary>
        /// if x betters the than y given the direction of the search
        /// maximizing or minimizing.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        protected Boolean betterThan(double x, double y)
        {
            return directionComparer.BetterThan(x, y);
        }
    }
}