using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    public abstract class abstractConvergence
    {
        public abstract Boolean converged(int YInteger = int.MinValue, double YDouble = double.NaN,
            IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<IList<double>> YJaggedDoubleArray = null);
    }

}