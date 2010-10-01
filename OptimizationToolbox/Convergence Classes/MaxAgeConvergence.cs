using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class MaxAgeConvergence : abstractConvergence
    {
        #region Fields
        int age = 0;
        IList<double> xlast;
        readonly double toleranceForSame;
        readonly int maxAge;
        #endregion


        public MaxAgeConvergence(int maxAge, double toleranceForSame)
        {
            this.maxAge = maxAge;
            this.toleranceForSame = toleranceForSame;
        }
        public override bool converged(int YInteger = int.MinValue, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, 
            IList<IList<double>> YJaggedDoubleArray = null)
        {
            var x = YDoubleArray1;
            if (x == null) throw new Exception("MaxAgeConvergence expected a 1-D array of doubles representing the decision vector, x.");
            findAgeOfBest(x);
            var result = (age >= maxAge);
            xlast = x;
            return result;
        }

        private void findAgeOfBest(IList<double> x)
        {
            if ((xlast != null) && (StarMath.norm1(x, xlast) <= toleranceForSame)) age++;
            else age = 0;
        }

    }
}