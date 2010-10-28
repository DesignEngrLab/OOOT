using System;
using OptimizationToolbox;

namespace Example4_Using_Dependent_Analysis
{
    public class massObjective : IObjectiveFunction, IDifferentiable
    {
        private readonly ForceVelocityPositionAnalysis FVPAnalysis;
        private readonly int numberGears;
        private readonly double gearDensity;

        public massObjective(ForceVelocityPositionAnalysis fvpsAnalysis, double gearDensity)
        {
            this.FVPAnalysis = fvpsAnalysis;
            this.numberGears = fvpsAnalysis.numGears;
            this.gearDensity = gearDensity;
        }

        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            var totMass = 0.0;
            for (var i = 0; i < numberGears; i++)
            {
                var faceWidth = x[i * 4 + 2];
                totMass += Math.PI * Math.Pow((FVPAnalysis.diameters[i] / 2), 2) * faceWidth * gearDensity;
            }
            return totMass;
        }
        /* there are four design variables per gear:
         * 0. number of teeth (N)
         * 1. pitch (P) or module (m)...gear tooth size
         * 2. face width (F)
         * 3. location variable, z
         * by setting the NumGearPairs to 3, we are create
         * 3 * (2* 4) = 24 variables. */

        #endregion

        #region Implementation of IDifferentiable

        public double deriv_wrt_xi(double[] x, int i)
        {
            var coeff = Math.PI * gearDensity / 4.0;
            if (i % 4 == 0) //number of teeth
                return coeff * 2 * x[i] * x[i + 2] / (x[i + 1] * x[i + 1]);
            if (i % 4 == 1) // pitch
                return coeff * -2 * x[i - 1] * x[i - 1] * x[i + 1] / (x[i] * x[i] * x[i]);
            if (i % 4 == 2) //face width
                return coeff * x[i - 2] * x[i - 2] / (x[i - 1] * x[i - 1]);
            else return 0;
        }

        #endregion
    }
}