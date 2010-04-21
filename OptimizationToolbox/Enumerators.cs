/*************************************************************************
 *     This file includes definitions for fundamental enumerators and the
 *     Comparer used throughout OOOT. 
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
 *     along with GraphSynth.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on GraphSynth
 *     at http://www.GraphSynth.com.
 *************************************************************************/
using System;
using System.Collections.Generic;


namespace OptimizationToolbox
{

    /* Enumerator for Search functions that have generality 
     * to either minimize or maximize (e.g. PNPPS, stochasticChoose). */
    public enum optimize { minimize = -1, maximize = 1 };

    public class optimizeSort : IComparer<double>
    {
        /* an internal integer equal to the required sort direction. */
        private int direction;
        public optimizeSort(optimize direction)
        {
            this.direction = (int)direction;
        }
        public int Compare(double x, double y)
        {
            /* in order to avoid the collections from throwing an error, we make sure
             * that only -1 or 1 is returned. If they are equal, we return 1. This makes
             * newer items to the list appear before older items. It is slightly more
             * efficient than returning -1 and conforms with the philosophy of always
             * exploring/preferring new concepts. See: SA's Metropolis Criteria. */
            if (x == y) return 1;
            if (x < y) return direction;
            else return -1 * direction;
        }
    }

    /* This enumerator is used primarily by the parameter tuning where suboptimization
     * functions (that derive from the Optimization Toolbox's abstractOptFunction) need
     * to determine how derivatives will be calculated. */
    public enum differentiate
    {
        Analytic,
        Back1,
        Forward1,
        Central2,
        Back2,
        Forward2,
        Central4
    };

}
