using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class MaxIterationsConvergence : abstractConvergence
    {
        public long maxIterations { get; set; }


        public MaxIterationsConvergence() { }

        public MaxIterationsConvergence(long maxIterations)
        {
            this.maxIterations = maxIterations;
        }
        public override bool converged(long k = -1, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            if (k < 0) throw new Exception("MaxIterationsConvergence expected a positive value for the first argument, YInteger");
            return (k >= maxIterations);
        }
    }
}