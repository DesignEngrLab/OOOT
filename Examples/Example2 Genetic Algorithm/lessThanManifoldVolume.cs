using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OptimizationToolbox;

namespace Example2_Genetic_Algorithm
{
    class lessThanManifoldVolume : IInequality
    {

        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            return -1 * x[0] * x[1] * x[2];
        }

        #endregion
    }
}
