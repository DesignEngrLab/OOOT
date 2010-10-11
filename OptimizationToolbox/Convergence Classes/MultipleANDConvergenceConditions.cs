using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class MultipleANDConvergenceConditions : abstractConvergence
    {
        #region Fields
        int maxIterations = int.MaxValue;
        int maxAge = int.MaxValue;
        double deltaX = double.NaN;
        double deltaF = double.NaN;
        double deltaGradF = double.NaN;
        double toleranceForSame = double.NegativeInfinity;
        public int MaxIterations
        {
            get { return maxIterations; }
            set { maxIterations = value; }
        }
        public double DeltaF
        {
            get { return deltaF; }
            set { deltaF = value; }
        }
        public double DeltaX
        {
            get { return deltaX; }
            set { deltaX = value; }
        }
        public double DeltaGradF
        {
            get { return deltaGradF; }
            set { deltaGradF = value; }
        }
        public int MaxAge
        {
            get { return maxAge; }
            set { maxAge = value; }
        }
        private double ToleranceForSame
        {
            get { return toleranceForSame; }
            set { toleranceForSame = value; }
        }

        int age = 0;
        IList<double> xlast;
        double flast;
        #endregion
        #region Constructor
        public MultipleANDConvergenceConditions() { }
        public MultipleANDConvergenceConditions(int MaxIterations, double DeltaF, double DeltaX = double.NaN,
                    double DeltaGradF = double.NaN, int MaxAge = int.MinValue, double ToleranceForSame = double.NegativeInfinity)
        {
            this.MaxIterations = MaxIterations;
            this.DeltaF = DeltaF;
            this.DeltaX = DeltaX;
            this.DeltaGradF = DeltaGradF;
            this.MaxAge = MaxAge;
            this.ToleranceForSame = ToleranceForSame;
        }
        #endregion
        public override bool converged(long YInteger, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
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