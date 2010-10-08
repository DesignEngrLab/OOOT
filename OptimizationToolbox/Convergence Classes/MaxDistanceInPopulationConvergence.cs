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
            var population = YJaggedDoubleArray;
            if (population == null)
                throw new Exception("MaxDistanceInPopulationConvergence expected an array of arrays of doubles (in the last argument, YJaggedDoubleArray) "
                    + " representing the current simplex of solutions.");
            if (population.Count == 0) return false;
            double maxSideLength = 0;
            for (int i = 0; i < population.Count - 1; i++)
                for (int j = i + 1; j < population.Count; j++)
                {
                    var sideLengthSquared = StarMath.norm2(population[i], population[j], true);
                    if (maxSideLength < sideLengthSquared) maxSideLength = sideLengthSquared;
                }
            SearchIO.output("side length =" + Math.Sqrt(maxSideLength), 9);
            return (Math.Sqrt(maxSideLength) <= MinimumSideLength);
        }
    }
}