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
    public class MaxAgeConvergence : abstractConvergence
    {
        #region Fields

        private int age;
        private IList<double> xlast;
        /// <summary>
        /// Gets or sets the tolerance for same candidate (used to increment the internal age of best).
        /// </summary>
        /// <value>The tolerance for same.</value>
        public double toleranceForSame { get; set; }
        /// <summary>
        /// Gets or sets the max age.
        /// </summary>
        /// <value>The max age.</value>
        public int maxAge { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxAgeConvergence"/> class.
        /// </summary>
        public MaxAgeConvergence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxAgeConvergence"/> class.
        /// </summary>
        /// <param name="maxAge">The max age.</param>
        /// <param name="toleranceForSame">The tolerance for same.</param>
        public MaxAgeConvergence(int maxAge, double toleranceForSame)
        {
            this.maxAge = maxAge;
            this.toleranceForSame = toleranceForSame;
        }

        /// <summary>
        /// Internally keeps track of the age of the best candidate. If it exceeds the given MaxAge value, 
        /// the criteria will return true. This is to say, if no better candidate is found in MaxAge 
        /// iterations, return true.
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
        public override bool converged(long iteration = -1, long numFnEvals = -1, double fBest = double.NaN,
            IList<double> xBest = null, IList<double[]> population = null, IList<double> gradF = null)
        {
            if (xBest == null)
                throw new Exception(
                    "MaxAgeConvergence expected a 1-D array of doubles representing the decision vector, x.");
            findAgeOfBest(xBest);
            xlast = xBest;
            return (age >= maxAge);
        }

        private void findAgeOfBest(IList<double> x)
        {
            if ((xlast != null) && (x.norm1(xlast) <= toleranceForSame)) age++;
            else age = 0;
        }
    }
}