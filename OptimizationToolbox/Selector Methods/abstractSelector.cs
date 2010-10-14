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
    public abstract class abstractSelector
    {
        protected readonly optimize direction;
        protected readonly optimizeSort directionComparer;
        protected abstractSelector(optimize direction)
        {
            this.direction = direction;
            directionComparer = new optimizeSort(direction, true);
        }
        public abstract void selectCandidates(ref List<KeyValuePair<double, double[]>> candidates, double control = double.NaN);

        protected static List<int> makeRandomIntList(int size)
        {
            Random rnd = new Random();
            var result = new List<int>(size);
            for (int i = 0; i < size; i++)
                result.Insert(rnd.Next(result.Count), i);
            return result;
        }


        protected void sort(ref List<KeyValuePair<double, double[]>> candidates)
        {
            candidates = candidates.OrderBy(a => a.Key, new optimizeSort(direction, true)).ToList();
        }
        protected void randomizeList(ref List<KeyValuePair<double, double[]>> candidates)
        {
            var r = new Random();
            candidates = candidates.OrderBy(a => r.NextDouble()).ToList();
        }
        protected Boolean betterThan(double x, double y)
        { return directionComparer.BetterThan(x, y); }

    }
}
