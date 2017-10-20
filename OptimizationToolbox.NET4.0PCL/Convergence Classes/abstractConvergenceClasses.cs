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
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    /// <summary>
    /// The abstract class that all convergence criteria must inherit from. There is one Boolean function
    /// that is returned from the class, called "converged", which takes up to five arguments.
    /// </summary>
    [XmlInclude(typeof(DeltaFConvergence)), XmlInclude(typeof(DeltaGradFConvergence)),
     XmlInclude(typeof(DeltaXConvergence)), XmlInclude(typeof(MaxAgeConvergence)),
    XmlInclude(typeof(MaxFnEvalsConvergence)), XmlInclude(typeof(MaxIterationsConvergence)),
    XmlInclude(typeof(MaxSpanInPopulationConvergence)), XmlInclude(typeof(MaxTimeConvergence)),
     XmlInclude(typeof(ToKnownBestFConvergence)), XmlInclude(typeof(ToKnownBestXConvergence))]
    public abstract class abstractConvergence
    {
        /// <summary>
        /// Has the optimization algorithm converged? Each criteria that overrides this is OR'ed together
        /// that means only one critieria needs to return true.
        /// </summary>
        /// <param name="iteration">The number of iterations.</param>
        /// <param name="numFnEvals">The number of function evaluations.</param>
        /// <param name="fBest">The best f.</param>
        /// <param name="xBest">The best x.</param>
        /// <param name="population">The population of candidates.</param>
        /// <param name="gradF">The gradient of F.</param>
        /// <returns>
        /// true or false - has the process converged?
        /// </returns>
        public abstract Boolean converged(long iteration = -1, long numFnEvals = -1, double fBest = double.NaN,
                                          IList<double> xBest = null, IList<double[]> population = null, IList<double> gradF = null);
    }
}