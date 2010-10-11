using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class ToKnownBestConvergence : abstractConvergence
    {
        public double fBest { get; set; }
        private optimize _direction;
        public optimize direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                directionComparer = new optimizeSort(direction, true);
            }
        }
        private optimizeSort directionComparer;


        #region Constructor
        public ToKnownBestConvergence() { }
        public ToKnownBestConvergence(double fBest, optimize direction)
        {
            this.fBest = fBest;
            this.direction = direction;
        }

        #endregion
        public override bool converged(long YInteger = -2147483648, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var f = YDouble;
            if (f == null)
                throw new Exception("ToKnownBestConvergence expected a a doubles (in the second argument, YDouble) "
                    + " representing the objective function.");
            return directionComparer.BetterThan(f, fBest);

        }
    }
}