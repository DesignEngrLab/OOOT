/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System;
using OptimizationToolbox;

namespace Example4_Using_Dependent_Analysis
{
    /* the best gear train in this problem is the one with the lightest
     * combination of gears. The objective function is a farely easy thing
     * to calculate. It is monotonic with 3/4 of the variables and the remaining
     * quarter is not related. We are fortunate in that we can inherit from
     * IDifferentiable as well. This drastically cuts down on the number of function
     * calls for gradient based methods. */
    public class massObjective : IObjectiveFunction, IDifferentiable
    {
        private readonly ForceVelocityPositionAnalysis FVPAnalysis;
        private readonly int numberGears;
        private readonly double gearDensity;

        public massObjective(ForceVelocityPositionAnalysis fvpsAnalysis, double gearDensity)
        {
            FVPAnalysis = fvpsAnalysis;
            numberGears = fvpsAnalysis.numGears;
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
            return 0;
        }

        #endregion
    }
}