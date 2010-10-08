using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class ToKnownBestConvergence : abstractConvergence
    {
        public double fBest { get; set; }
        protected readonly optimize direction;
        protected readonly optimizeSort directionComparer;
     

        #region Constructor
        public ToKnownBestConvergence() { }
        public ToKnownBestConvergence(double fBest,optimize direction)
            {
                this.fBest = fBest;
                this.direction = direction;
                directionComparer = new optimizeSort(direction, true);
            }

            #endregion
        public override bool converged(int YInteger = -2147483648, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var f = YDouble;
            if (f == null)
                throw new Exception("DeltaFConvergence expected a 1-D array of doubles (in the third argument, YDoubleArray1) "
                    + " representing the current decision vector, x.");
            return (1!=directionComparer.Compare(f, fBest));

        }
    }
}