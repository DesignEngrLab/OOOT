using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class MaxIterationsConvergence : abstractConvergence
    {
        public int maxIterations { get; set; }


        public MaxIterationsConvergence() { }

        public MaxIterationsConvergence(int maxIterations)
        {
            this.maxIterations = maxIterations;
        }
        public override bool converged(int YInteger = -2147483648, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var k = YInteger;
            if (k < 0) throw new Exception("MaxIterationsConvergence expected a positive value for the first argument, YInteger");
            return (k >= maxIterations);
        }
    }
}