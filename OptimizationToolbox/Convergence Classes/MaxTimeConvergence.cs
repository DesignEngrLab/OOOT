using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class MaxTimeConvergence : abstractConvergence
    {
        public TimeSpan maxTime { get; set; }
        readonly DateTime startTime;

        #region Constructor
        public MaxTimeConvergence(){}
        public MaxTimeConvergence(TimeSpan maxTime)
        {
            this.maxTime = maxTime;
            startTime = DateTime.Now;
        }
        public MaxTimeConvergence(DateTime timeToStop)
        {
            startTime = DateTime.Now;
            maxTime = timeToStop - startTime;
        }
        #endregion
        public override bool converged(int YInteger = -2147483648, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            return ((DateTime.Now - startTime) >= maxTime);
        }
    }
}