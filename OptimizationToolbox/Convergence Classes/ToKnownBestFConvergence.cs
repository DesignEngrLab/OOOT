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

namespace OptimizationToolbox
{
    public class ToKnownBestFConvergence : abstractConvergence
    {
        private double negTolerance = double.PositiveInfinity;
        private double posTolerance = double.PositiveInfinity;
        /// <summary>
        /// Gets or sets the optimal f which the process should stop after reaching.
        /// </summary>
        /// <value>The f at optimal.</value>
        public double fAtOptimal { get; set; }

        /// <summary>
        /// Gets or sets the tolerance negative on the positive side of fAtOptimal.
        /// </summary>
        /// <value>The positive tolerance.</value>
        public double positiveTolerance
        {
            get { return posTolerance; }
            set { posTolerance = value; }
        }

        /// <summary>
        /// Gets or sets the tolerance negative on the negative side of fAtOptimal.
        /// It should be stored as a positive number, but either way the absolute value
        /// is taken to make the range about fAtOptimal.
        /// </summary>
        /// <value>The negative tolerance.</value>
        public double negativeTolerance
        {
            get { return negTolerance; }
            set { negTolerance = value; }
        }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ToKnownBestFConvergence"/> class.
        /// </summary>
        public ToKnownBestFConvergence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToKnownBestFConvergence"/> class.
        /// </summary>
        /// <param name="fAtOptimal">The f at optimal.</param>
        /// <param name="positiveTolerance">The positive tolerance.</param>
        /// <param name="negativeTolerance">The negative tolerance.</param>
        public ToKnownBestFConvergence(double fAtOptimal, double positiveTolerance,
                                       double negativeTolerance = double.PositiveInfinity)
        {
            this.fAtOptimal = fAtOptimal;
            this.positiveTolerance = positiveTolerance;
            this.negativeTolerance = negativeTolerance;
        }

        #endregion

        /// <summary>
        /// Given a value for fAtOptimal and a tolerance on either side. The criteria returns
        /// true when the value of fBest is within the range of fAtOptimal +/- tolerance. One
        /// is welcome to set the tolerance to 0. The default of infinity is fine if it is on
        /// the opposite side of the optimization direction.
        /// </summary>
        /// <param name="iteration">The number of iterations (not used).</param>
        /// <param name="numFnEvals">The number of function evaluations (not used).</param>
        /// <param name="fBest">The best f.</param>
        /// <param name="xBest">The best x (not used).</param>
        /// <param name="population">The population of candidates (not used).</param>
        /// <param name="gradF">The gradient of F (not used).</param>
        /// <returns>
        /// true or false - has the process converged?
        /// </returns>
        public override bool converged(long iteration = -1, long numFnEvals = -1, double fBest = double.NaN, IList<double> xBest = null, IList<double[]> population = null, IList<double> gradF = null)
        {
            if (double.IsNaN(fBest))
                throw new Exception("ToKnownBestConvergence expected a double (in the second argument, YDouble) "
                                    + " representing the objective function.");
            return ((fBest <= fAtOptimal + Math.Abs(positiveTolerance)) 
                && (fBest >= fAtOptimal - Math.Abs(negativeTolerance)));
        }
    }
}