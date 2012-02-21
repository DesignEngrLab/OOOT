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
    public class MaxSpanInPopulationConvergence : abstractConvergence
    {
        #region Properties

        /// <summary>
        /// Gets or sets the minimum span of the set of solutions found in the population.
        /// The span is the farthest euclidian distance between any two solutions.
        /// </summary>
        /// <value>The minimum span.</value>
        public double MinimumSpan { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxSpanInPopulationConvergence"/> class.
        /// </summary>
        public MaxSpanInPopulationConvergence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxSpanInPopulationConvergence"/> class.
        /// </summary>
        /// <param name="MinimumSpan">The minimum span.</param>
        public MaxSpanInPopulationConvergence(double MinimumSpan)
        {
            this.MinimumSpan = MinimumSpan;
        }

        /// <summary>
        /// Given a minimum span, S, this criteria returns true when the span is equal to 
        /// or less than S. This is probably the slowest criteria (p*log(p)) given that it must
        /// check the distance between every pair of solutions in the population. But, probably 
        /// not an significant increase  for p less than 1000.
        /// </summary>
        /// <param name="iteration">The number of iterations (not used).</param>
        /// <param name="numFnEvals">The number of function evaluations (not used).</param>
        /// <param name="fBest">The best f (not used).</param>
        /// <param name="xBest">The best x (not used).</param>
        /// <param name="population">The population of candidates.</param>
        /// <param name="gradF">The gradient of F (not used).</param>
        /// <returns>
        /// true or false - has the process converged?
        /// </returns>
        public override bool converged(long iteration = -1, long numFnEvals = -1, double fBest = double.NaN, IList<double> xBest = null, IList<double[]> population = null, IList<double> gradF = null)
        {
            if (population == null)
                throw new Exception("MaxSpanInPopulationConvergence expected an array of arrays of doubles (in the last argument, YJaggedDoubleArray) "
                                    + " representing the current simplex of solutions.");
            if (population.Count == 0) return false;
            double maxSideLength = 0;
            for (var i = 0; i < population.Count - 1; i++)
                for (var j = i + 1; j < population.Count; j++)
                {
                    var sideLengthSquared = StarMath.norm2(population[i], population[j], true);
                    if (maxSideLength < sideLengthSquared) maxSideLength = sideLengthSquared;
                }
            SearchIO.output("side length =" + Math.Sqrt(maxSideLength), 6);
            return (Math.Sqrt(maxSideLength) <= MinimumSpan);
        }
    }
}