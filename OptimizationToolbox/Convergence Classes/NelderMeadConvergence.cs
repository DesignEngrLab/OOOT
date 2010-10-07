using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarMathLib;

namespace OptimizationToolbox
{
    public class NelderMeadConvergence : abstractConvergence
    {
        double lastBestF = double.NaN;
        int ageBest = 0;

        #region Properties
        public int MaxIterations { get; set; }
        public int MaxAge { get; set; }
        public double MinimumSideLength { get; set; }
        public double toleranceForSame { get; set; }
        #endregion
        public NelderMeadConvergence(){}
        public NelderMeadConvergence(int MaxIterations, int MaxAge, double toleranceForSame, double MinimumSideLength)
        {
            this.MaxAge = MaxAge;
            this.MaxIterations = MaxIterations;
            this.MinimumSideLength = MinimumSideLength;
            this.toleranceForSame = toleranceForSame;
        }

        public override bool converged(int YInteger = -2147483648, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var k = YInteger;
            if (k < 0) throw new Exception("NelderMeadConvergence expected a positive value for the first argument, YInteger");
            var f = YDouble;
            if (double.IsNaN(f))
                throw new Exception("NelderMeadConvergence expected a double value (in the second argument, YDouble) "
                    + " representing the last calculated value of f.");
            var simplex = YJaggedDoubleArray;
            if (simplex == null)
                throw new Exception("NelderMeadConvergence expected an array of arrays of doubles (in the last argument, YJaggedDoubleArray) "
                    + " representing the current simplex of solutions.");

            if (k >= MaxIterations) return true;
            SearchIO.output("fbest = " + f, 9);
            SearchIO.output("age = " + ageBest, 9);
            if (f == lastBestF) ageBest++;
            else
            {
                lastBestF = f;
                ageBest = 0;
            }
            if (ageBest >= MaxAge) return true;

            double maxSideLength = 0;
            for (int i = 0; i < simplex.Count - 1; i++)
                for (int j = i + 1; j < simplex.Count; j++)
                {
                    var sideLengthSquared = StarMath.norm2(simplex[i], simplex[j], true);
                    if (maxSideLength < sideLengthSquared) maxSideLength = sideLengthSquared;
                }
            SearchIO.output("side length =" + Math.Sqrt(maxSideLength), 9);
            return (Math.Sqrt(maxSideLength) <= MinimumSideLength);
        }
    }
}