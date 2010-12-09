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
using OptimizationToolbox;
using StarMathLib;

namespace Example4_Using_Dependent_Analysis
{
    /* Here is an example of a dependent analysis. Other than the constructor
     * it has but one function - calculate. Unlike objective functions and 
     * constraints, however; this returns nothing. All that it does is set all
     * of its public properties for perusal by the other functions. */
    public class ForceVelocityPositionAnalysis : IDependentAnalysis
    {
        private readonly double[,] inputPosition;
        private readonly double inputSpeed;
        public readonly int numGears;
        private readonly double outputTorque;

        public double[][,] positions { get; private set; }
        public double[] diameters { get; private set; }
        public double[] forces { get; private set; }
        public double[] torques { get; private set; }
        public double[] speeds { get; private set; }

        public ForceVelocityPositionAnalysis(int numGears, double outputTorque, double inputSpeed,
                                             double[,] inputPosition)
        {
            this.numGears = numGears;
            this.outputTorque = outputTorque;
            this.inputSpeed = inputSpeed;
            this.inputPosition = inputPosition;
            diameters = new double[numGears];
            forces = new double[numGears];
            torques = new double[numGears];
            speeds = new double[numGears];
            positions = new double[numGears][,];
        }

        #region Implementation of IDependentAnalysis

        /* there are four design variables per gear:
         * 0. number of teeth (N)
         * 1. pitch (P) or module (m)...gear tooth size
         * 2. face width (F)
         * 3. location variable, z
         * by setting the NumGearPairs to 3, we are create
         * 3 * (2* 4) = 24 variables. */

        public void calculate(double[] x)
        {
            speeds[0] = inputSpeed;
            positions[0] = inputPosition;
            for (var i = 0; i < numGears; i++)
            {
                var N = x[i * 4];
                var P = x[i * 4 + 1];
                var F = x[i * 4 + 2];
                var Z = x[i * 4 + 3];
                diameters[i] = N / P;
                if (i == 0)
                {
                    if (i % 2 == 0)
                    /* If it is even-numbered gear, then it shares
                 * a shaft with the previous gear. Speeds are the
                 * same and position is just translated along the shaft */
                    {
                        speeds[i] = speeds[i - 1];
                        positions[i] = StarMath.multiply(positions[i - 1], StarMath.Translate(0.0, 0.0, Z));
                    }
                    else
                    /* else the gear is odd-numbered and mates with the previous
          * gear through mating teeth */
                    {
                        speeds[i] = (x[(i - 1) * 4] / N) * speeds[i - 1];
                        positions[i] = StarMath.multiply(positions[i - 1], StarMath.RotationZ(Z));
                        positions[i] = StarMath.multiply(positions[i],
                                                         StarMath.Translate((diameters[i - 1] + diameters[i]) / 2, 0.0,
                                                                            0.0));
                    }
                }
                /* finally we move the coordinates from the front side of the gear to the back. This is the case
                 * for the first gear as well. */
                positions[i] = StarMath.multiply(positions[i - 1], StarMath.Translate(0.0, 0.0, F));
            }
            torques[numGears - 1] = outputTorque;
            forces[numGears - 1] = 2 * outputTorque / diameters[numGears - 1];
            for (var i = numGears - 2; i >= 0; i--)
            {
                var N = x[i * 4];
                var NNext = x[(i + 1) * 4];
                if (i % 2 == 0) /*even numbered gears are the drivers in this simple model */
                {
                    forces[i] = forces[i + 1];
                    torques[i] = (N / NNext) * torques[i + 1];
                }
                else /* odd gears share shaft with the next neighbor */
                {
                    torques[i] = torques[i + 1];
                    forces[i] = 2 * torques[i] / diameters[i];
                }
            }
        }

        #endregion

    }
}