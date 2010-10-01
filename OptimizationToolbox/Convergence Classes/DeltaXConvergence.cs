using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class DeltaXConvergence : abstractConvergence
    {
        readonly double minDifference;
        IList<double> xlast;


        #region Constructor
        public DeltaXConvergence(double minDifference)
        {
            this.minDifference = minDifference;
        }
        #endregion

        public override bool converged(int YInteger = int.MinValue, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<IList<double>> YJaggedDoubleArray = null)
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