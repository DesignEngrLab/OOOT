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
using StarMathLib;

namespace OptimizationToolbox
{
    public class DeltaFConvergence : abstractConvergence
    {
        private double flast;
        private IList<double> xlast;

        #region Constructor

        public DeltaFConvergence()
        {
        }

        public DeltaFConvergence(double minDifference, double toleranceForSame = double.NegativeInfinity)
        {
            this.minDifference = minDifference;
            this.toleranceForSame = toleranceForSame;
        }

        #endregion

        /// <summary>
        /// Gets or sets the minimum difference.
        /// </summary>
        /// <value>The min difference.</value>
        public double minDifference { get; set; }
        /// <summary>
        /// Gets or sets the tolerance for same x. If the x is the same as the last the condition is NOT checked (returns false).
        /// If this is not a desirable catch, then leave as the default (negative infinity). In fact any negative value for
        /// toleranceForSame will cause it to be ignored since the distance between any two points can at best be 0.
        /// </summary>
        /// <value>The tolerance for same.</value>
        public double toleranceForSame { get; set; }

        /// <summary>
        /// Given a value D (minimum difference), this criteria will return true, if the distance (absolute value of the difference) 
        /// between fBest and flast is less than or equal to D.
        /// </summary>
        /// <param name="iteration">The number of iterations (not used).</param>
        /// <param name="numFnEvals">The number of function evaluations (not used).</param>
        /// <param name="fBest">The best f.</param>
        /// <param name="xBest">The best x (used to check if candidate is the same).</param>
        /// <param name="population">The population of candidates (not used).</param>
        /// <param name="gradF">The gradient of F (not used).</param>
        /// <returns>
        /// true or false - has the process converged?
        /// </returns>
        public override bool converged(long iteration = -1, long numFnEvals = -1, double fBest = double.NaN,
            IList<double> xBest = null, IList<double[]> population = null, IList<double> gradF = null)
        {
            if (double.IsNaN(fBest))
                throw new Exception("DeltaFConvergence expected a double value (in the second argument, YDouble) "
                                    + " representing the last calculated value of f.");
            if (xBest == null)
                throw new Exception("DeltaFConvergence expected a 1-D array of doubles (in the third argument, YDoubleArray1) "
                                    + " representing the current decision vector, x.");
            if ((xlast == null) || (StarMath.norm1(xBest, xlast) > toleranceForSame))
            {
                var result = (Math.Abs(fBest - flast) <= minDifference);
                xlast = xBest;
                flast = fBest;
                return result;
            }
            return false;
        }
    }
}