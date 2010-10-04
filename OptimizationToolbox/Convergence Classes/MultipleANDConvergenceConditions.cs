using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class MultipleANDConvergenceConditions : abstractConvergence
    {
        #region Fields
        readonly int maxIterations = int.MaxValue;
        readonly int maxAge = int.MaxValue;
        readonly double deltaX;
        readonly double deltaF;
        readonly double deltaGradF;
        readonly double toleranceForSame;

        int age = 0;
        IList<double> xlast;
        double flast;
        #endregion
        #region Constructor
        public MultipleANDConvergenceConditions(int MaxIterations, double DeltaF, double DeltaX = double.NaN,
                    double DeltaGradF = double.NaN, int MaxAge = int.MinValue, double ToleranceForSame = double.NegativeInfinity)
        {
            maxIterations = MaxIterations;
            deltaF = DeltaF;
            deltaX = DeltaX;
            deltaGradF = DeltaGradF;
            maxAge = MaxAge;
            toleranceForSame = ToleranceForSame;
        }
        #endregion
        public override bool converged(int YInteger = int.MinValue, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<IList<double>> YJaggedDoubleArray = null)
        {
            var k = YInteger;
            if (k < 0) throw new Exception("MultipleANDConvergenceConditions expected a positive value for the first argument, YInteger");
            if (k < maxIterations) return false;

            var f = YDouble;
            if (double.IsNaN(f))
                throw new Exception("MultipleANDConvergenceConditions expected a double value (in the second argument, YDouble)"
                    + " representing the last calculated value of f.");
            if (Math.Abs(f - flast) > deltaF) return false;
            flast = f;

            if (maxAge > 0)
            {
                var x = YDoubleArray1;
                if (x == null)
                    throw new Exception("MultipleANDConvergenceConditions expected a 1-D array of doubles (in the third argument, YDoubleArray1) " +
                        "representing the decision vector, x.");
                findAgeOfBest(x);
                if (age < maxAge)
                {
                    xlast = x;
                    return false;
                }
            }

            if (!double.IsNaN(deltaX))
            {
                var x = YDoubleArray1;
                if (x == null)
                    throw new Exception("MultipleANDConvergenceConditions expected a 1-D array of doubles (in the third argument, YDoubleArray1) " +
                        "representing the decision vector, x.");
                if ((xlast == null) || (StarMath.norm1(x, xlast) > deltaX))
                {
                    xlast = x;
                    return false;
                }
            }

            if (!double.IsNaN(deltaGradF))
            {
                var gradf = YDoubleArray2;
                if (gradf == null)
                    throw new Exception("DeltaGradFConvergence expected a 1-D array of doubles (in the fourth argument, YDoubleArray2) "
                        + " representing the last calculated gradient of f.");
                if (StarMath.norm1(gradf) <= deltaGradF) return false;
            }
            return true;
        }

        private void findAgeOfBest(IList<double> x)
        {
            if ((xlast != null) && (StarMath.norm1(x, xlast) <= toleranceForSame)) age++;
            else age = 0;
        }

    }
}