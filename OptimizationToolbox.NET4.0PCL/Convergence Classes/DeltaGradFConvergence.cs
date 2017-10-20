/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT X11 License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT X11 License for more details.
 *  


 *     
 *     Please find further details and contact information on OOOT
 *     at http://designengrlab.github.io/OOOT/.
 *************************************************************************/
using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class DeltaGradFConvergence : abstractConvergence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaGradFConvergence"/> class.
        /// </summary>
        public DeltaGradFConvergence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaGradFConvergence"/> class.
        /// </summary>
        /// <param name="minDifference">The min difference.</param>
        public DeltaGradFConvergence(double minDifference)
        {
            this.minDifference = minDifference;
        }

        /// <summary>
        /// Gets or sets the min difference.
        /// </summary>
        /// <value>The min difference.</value>
        public double minDifference { get; set; }

        /// <summary>
        /// Given a value D (minimum difference), this criteria will return true, if the norm 
        /// (1-norm sum of absolute values) of the gradient less than or equal to D.
        /// </summary>
        /// <param name="iteration">The number of iterations (not used).</param>
        /// <param name="numFnEvals">The number of function evaluations (not used).</param>
        /// <param name="fBest">The best f (not used).</param>
        /// <param name="xBest">The best x (not used).</param>
        /// <param name="population">The population of candidates (not used).</param>
        /// <param name="gradF">The gradient of F.</param>
        /// <returns>
        /// true or false - has the process converged?
        /// </returns>
        public override bool converged(long iteration = -1, long numFnEvals = -1, double fBest = double.NaN,
            IList<double> xBest = null, IList<double[]> population = null, IList<double> gradF = null)
        {
            if (gradF == null)
                throw new Exception("DeltaGradFConvergence expected a 1-D array of doubles (in the fourth argument, YDoubleArray2) "
                                    + " representing the last calculated gradient of f.");
            return (gradF.norm1() <= minDifference);
        }
    }
}