using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimizationToolbox;

namespace tester
{
    class BragasFruitTruck : IObjectiveFunction
    {
        public double calculate(double[] x)
        {
            if (x.Any(anyX => anyX < 0)) return 99999;
            var C1 = 1007.5;
            var C2 = 1070.5;
            var C3 = 258.18;

            var n = x[0];
            var t = x[1];
            var nNegTwoThirds = Math.Pow(n, -2.0 / 3.0);
            return 21 * n
                   + C1 * nNegTwoThirds
                   + C2 * Math.Pow(n, 1.0 / 3.0) / (t + 1.2)
                   + C3 * nNegTwoThirds * t;

        }
    }
}
