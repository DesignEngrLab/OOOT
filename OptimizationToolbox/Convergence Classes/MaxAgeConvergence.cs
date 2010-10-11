using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class MaxAgeConvergence : abstractConvergence
    {
        #region Fields
        int age;
        IList<double> xlast;
        public double toleranceForSame { get; set; }
        public int maxAge { get; set; }
        #endregion


        public MaxAgeConvergence(){}
        public MaxAgeConvergence(int maxAge, double toleranceForSame)
        {
            this.maxAge = maxAge;
            this.toleranceForSame = toleranceForSame;
        }
        public override bool converged(long YInteger, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var x = YDoubleArray1;
            if (x == null) throw new Exception("MaxAgeConvergence expected a 1-D array of doubles representing the decision vector, x.");
            findAgeOfBest(x);
            xlast = x;
            return (age >= maxAge);
        }

        private void findAgeOfBest(IList<double> x)
        {
            if ((xlast != null) && (StarMath.norm1(x, xlast) <= toleranceForSame)) age++;
            else age = 0;
        }

    }
}