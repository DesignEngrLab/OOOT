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
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class MultipleANDConvergenceConditions : abstractConvergence
    {
        #region Fields

        private int age;
        private double deltaF = double.NaN;
        private double deltaGradF = double.NaN;
        private double deltaX = double.NaN;
        private double flast;
        private int maxAge = int.MaxValue;
        private int maxIterations = int.MaxValue;
        private double toleranceForSame = double.NegativeInfinity;
        private IList<double> xlast;

        public int MaxIterations
        {
            get { return maxIterations; }
            set { maxIterations = value; }
        }

        public double DeltaF
        {
            get { return deltaF; }
            set { deltaF = value; }
        }

        public double DeltaX
        {
            get { return deltaX; }
            set { deltaX = value; }
        }

        public double DeltaGradF
        {
            get { return deltaGradF; }
            set { deltaGradF = value; }
        }

        public int MaxAge
        {
            get { return maxAge; }
            set { maxAge = value; }
        }

        private double ToleranceForSame
        {
            get { return toleranceForSame; }
            set { toleranceForSame = value; }
        }

        #endregion

        #region Constructor

        public MultipleANDConvergenceConditions()
        {
        }

        public MultipleANDConvergenceConditions(int MaxIterations, double DeltaF, double DeltaX = double.NaN,
                                                double DeltaGradF = double.NaN, int MaxAge = int.MinValue,
                                                double ToleranceForSame = double.NegativeInfinity)
        {
            this.MaxIterations = MaxIterations;
            this.DeltaF = DeltaF;
            this.DeltaX = DeltaX;
            this.DeltaGradF = DeltaGradF;
            this.MaxAge = MaxAge;
            this.ToleranceForSame = ToleranceForSame;
        }

        #endregion

        public override bool converged(long YInteger, double YDouble = double.NaN, IList<double> YDoubleArray1 = null,
                                       IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var k = YInteger;
            if (k < 0)
                throw new Exception(
                    "MultipleANDConvergenceConditions expected a positive value for the first argument, YInteger");
            if (k < maxIterations) return false;


            var x = YDoubleArray1;
            if (maxAge > 0)
            {
                if (x == null)
                    throw new Exception(
                        "MultipleANDConvergenceConditions expected a 1-D array of doubles (in the third argument, YDoubleArray1) " +
                        "representing the decision vector, x.");
                findAgeOfBest(x);
                if (age < maxAge)
                {
                    xlast = x;
                    return false;
                }
            }

            if (!double.IsNaN(deltaX))
            {
                if (x == null)
                    throw new Exception(
                        "MultipleANDConvergenceConditions expected a 1-D array of doubles (in the third argument, YDoubleArray1) " +
                        "representing the decision vector, x.");
                if ((xlast == null) || (StarMath.norm1(x, xlast) <= ToleranceForSame) ||
                    (StarMath.norm1(x, xlast) > deltaX))
                {
                    xlast = x;
                    return false;
                }
            }
            var f = YDouble;
            if (double.IsNaN(f))
                throw new Exception("MultipleANDConvergenceConditions expected a double value (in the second argument, YDouble)"
                                    + " representing the last calculated value of f.");
            if ((StarMath.norm1(x, xlast) <= ToleranceForSame) || (Math.Abs(f - flast) > deltaF)) return false;
            flast = f;

            if (!double.IsNaN(deltaGradF))
            {
                var gradf = YDoubleArray2;
                if (gradf == null)
                    throw new Exception("DeltaGradFConvergence expected a 1-D array of doubles (in the fourth argument, YDoubleArray2) "
                                        + " representing the last calculated gradient of f.");
                if (StarMath.norm1(gradf) <= deltaGradF) return false;
            }
            return true;
        }

        private void findAgeOfBest(IList<double> x)
        {
            if ((xlast != null) && (StarMath.norm1(x, xlast) <= ToleranceForSame)) age++;
            else age = 0;
        }
    }
}