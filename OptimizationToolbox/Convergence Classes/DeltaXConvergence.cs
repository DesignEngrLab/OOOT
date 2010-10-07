using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class DeltaXConvergence : abstractConvergence
    {
        public double minDifference { get; set; }
        IList<double> xlast;


        #region Constructor
        public DeltaXConvergence(){}
        public DeltaXConvergence(double minDifference)
        {
            this.minDifference = minDifference;
        }
        #endregion

        public override bool converged(int YInteger = -2147483648, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var x = YDoubleArray1;
            if (x == null)
                throw new Exception("DeltaXConvergence expected a 1-D array of doubles (in the third argument, YDoubleArray1) "
                    + " representing the current decision vector, x.");
            if ((xlast == null) || (StarMath.norm1(x, xlast) > minDifference))
            {
                xlast = x;
                return false;
            }
            return true;
        }
    }
}