using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarMathLib;

namespace OptimizationToolbox
{
    public class MaxDistanceInPopulationConvergence : abstractConvergence
    {
        #region Properties
        public double MinimumSideLength { get; set; }
        #endregion
        public MaxDistanceInPopulationConvergence(){}
        public MaxDistanceInPopulationConvergence(double MinimumSideLength)
        {
            this.MinimumSideLength = MinimumSideLength;
        }

        public override bool converged(int YInteger = -2147483648, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var simplex = YJaggedDoubleArray;
            if (simplex == null)
                throw new Exception("NelderMeadConvergence expected an array of arrays of doubles (in the last argument, YJaggedDoubleArray) "
                    + " representing the current simplex of solutions.");

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