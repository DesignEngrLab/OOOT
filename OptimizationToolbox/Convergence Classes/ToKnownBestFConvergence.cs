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
    public class ToKnownBestFConvergence : abstractConvergence
    {
        public double fBest { get; set; }
        private double posTolerance = double.PositiveInfinity;
        private double negTolerance = double.PositiveInfinity;
        public double positiveTolerance { get { return posTolerance; } set { posTolerance = value; } }
        public double negativeTolerance { get { return negTolerance; } set { negTolerance = value; } }


        #region Constructor
        public ToKnownBestFConvergence() { }
        public ToKnownBestFConvergence(double fBest, double positiveTolerance, double negativeTolerance = double.PositiveInfinity)
        {
            this.fBest = fBest;
            this.positiveTolerance = positiveTolerance;
            this.negativeTolerance = negativeTolerance;
        }

        #endregion
        public override bool converged(long YInteger, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var f = YDouble;
            if (double.IsNaN(f))
                throw new Exception("ToKnownBestConvergence expected a double (in the second argument, YDouble) "
                    + " representing the objective function.");
            return ((f < fBest + Math.Abs(positiveTolerance)) && (f > fBest - Math.Abs(negativeTolerance)));

        }
    }
}