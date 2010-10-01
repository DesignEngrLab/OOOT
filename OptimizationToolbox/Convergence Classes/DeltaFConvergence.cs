using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class DeltaFConvergence : abstractConvergence
    {
        readonly double minDifference;
        readonly double toleranceForSame;
        IList<double> xlast;
        double flast;


        #region Constructor
        public DeltaFConvergence(double minDifference, double toleranceForSame = double.NegativeInfinity)
        {
            this.minDifference = minDifference;
            this.toleranceForSame = toleranceForSame;
        }
        #endregion
        public override bool converged(int YInteger = int.MinValue, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<IList<double>> YJaggedDoubleArray = null)
        {
            var f = YDouble;
            if (double.IsNaN(f))
                throw new Exception("DeltaFConvergence expected a double value (in the second argument, YDouble) "
                    + " representing the last calculated value of f.");
            var x = YDoubleArray1;
            if (x == null)
                throw new Exception("DeltaFConvergence expected a 1-D array of doubles (in the third argument, YDoubleArray1) "
                    + " representing the current decision vector, x.");
            if ((xlast == null) || (StarMath.norm1(x, xlast) > toleranceForSame))
            {
                Boolean result =(Math.Abs(f - flast) <= minDifference);
                xlast = x;
                flast = f;
                return result;
            }
            return false;
        }
    }
}