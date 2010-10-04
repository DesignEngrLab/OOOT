using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class MaxIterationsConvergence : abstractConvergence
    {
        readonly int maxIterations;


        public MaxIterationsConvergence(int maxIterations)
        {
            this.maxIterations = maxIterations;
        }
        public override bool converged(int YInteger = int.MinValue, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<IList<double>> YJaggedDoubleArray = null)
        {
            var k = YInteger;
            if (k < 0) throw new Exception("MaxIterationsConvergence expected a positive value for the first argument, YInteger");
            return (k >= maxIterations);
        }
    }
}