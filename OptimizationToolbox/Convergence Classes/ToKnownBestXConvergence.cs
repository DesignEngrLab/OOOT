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
    public class ToKnownBestXConvergence : abstractConvergence
    {
        /// <summary>
        /// Gets or sets the min difference.
        /// </summary>
        /// <value>The min difference.</value>
        public double minDifference { get; set; }
        /// <summary>
        /// Gets or sets the optimal x which the process should stop after reaching.
        /// </summary>
        /// <value>The x at optimal.</value>
        public double[] xAtOptimal { get; set; }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ToKnownBestXConvergence"/> class.
        /// </summary>
        public ToKnownBestXConvergence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToKnownBestXConvergence"/> class.
        /// </summary>
        /// <param name="xBest">The x best.</param>
        /// <param name="minDifference">The min difference.</param>
        public ToKnownBestXConvergence(double[] xBest, double minDifference)
        {
            this.xAtOptimal = (double[])xBest.Clone();
            this.minDifference = minDifference;
        }

        #endregion

        /// <summary>
        /// Given a value for xAtOptimal and a minimum difference, D. The criteria returns
        /// true when the norm of the difference between xBest and xAtOptimal is less than
        /// or equal to D.
        /// </summary>
        /// <param name="iteration">The number of iterations (not used).</param>
        /// <param name="numFnEvals">The number of function evaluations (not used).</param>
        /// <param name="fBest">The best f (not used).</param>
        /// <param name="xBest">The best x.</param>
        /// <param name="population">The population of candidates (not used).</param>
        /// <param name="gradF">The gradient of F (not used).</param>
        /// <returns>
        /// true or false - has the process converged?
        /// </returns>
        public override bool converged(long iteration = -1, long numFnEvals = -1, double fBest = double.NaN, IList<double> xBest = null, IList<double[]> population = null, IList<double> gradF = null)
        {
            if (xBest == null)
                throw new Exception("DeltaXConvergence expected a 1-D array of doubles (in the third argument, YDoubleArray1) "
                                    + " representing the current decision vector, x.");
            return (StarMath.norm1(xBest, xAtOptimal) <= minDifference);
        }
    }
}