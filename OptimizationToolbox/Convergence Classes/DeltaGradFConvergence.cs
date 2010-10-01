using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class DeltaGradFConvergence : abstractConvergence
    {
        readonly double minDifference;
        public DeltaGradFConvergence(double minDifference)
        {
            this.minDifference = minDifference;
        }
        public override bool converged(int YInteger = int.MinValue, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<IList<double>> YJaggedDoubleArray = null)
        {
            var gradf = YDoubleArray2;
            if (gradf == null) 
                throw new Exception("DeltaGradFConvergence expected a 1-D array of doubles (in the fourth argument, YDoubleArray2) "
                    +" representing the last calculated gradient of f.");
            return (StarMath.norm1(gradf) <= minDifference);
        }
    }
}