using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarMathLib;

namespace OptimizationToolbox
{
    public class NelderMeadConvergence : abstractConvergence
    {
        readonly int maxIterations;
        readonly int maxAge;
        readonly double MinimumSideLength;
        readonly double toleranceForSame;
        double lastBestF = double.NaN;
        int ageBest = 0;

        #region Properties
        #endregion
        public NelderMeadConvergence(int MaxIterations, int MaxAge, double toleranceForSame, double MinimumSideLength)
        {
            this.maxAge = MaxAge;
            maxIterations = MaxIterations;
            this.MinimumSideLength = MinimumSideLength;
            this.toleranceForSame = toleranceForSame;
        }

        public override bool converged(int YInteger = int.MinValue, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<IList<double>> YJaggedDoubleArray = null)
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

            if (k >= maxIterations) return true;
            SearchIO.output("fbest = " + f, 5);
            SearchIO.output("age = " + ageBest, 5);
            if (f == lastBestF) ageBest++;
            else
            {
                lastBestF = f;
                ageBest = 0;
            }
            if (ageBest >= maxAge) return true;

            double maxSideLength = 0;
            for (int i = 0; i < simplex.Count - 1; i++)
                for (int j = i + 1; j < simplex.Count; j++)
                {
                    var sideLengthSquared = StarMath.norm2(simplex[i], simplex[j], true);
                    if (maxSideLength < sideLengthSquared) maxSideLength = sideLengthSquared;
                }
            SearchIO.output("side length =" + Math.Sqrt(maxSideLength), 5);
            return (Math.Sqrt(maxSideLength) <= MinimumSideLength);
        }
    }
}