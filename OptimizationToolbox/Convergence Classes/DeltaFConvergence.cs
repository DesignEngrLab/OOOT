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
    public class DeltaFConvergence : abstractConvergence
    {
        public double minDifference { get; set; }
        public double toleranceForSame { get; set; }
        IList<double> xlast;
        double flast;


        #region Constructor
        public DeltaFConvergence(){}
        public DeltaFConvergence(double minDifference, double toleranceForSame = double.NegativeInfinity)
        {
            this.minDifference = minDifference;
            this.toleranceForSame = toleranceForSame;
        }
        #endregion
        public override bool converged(long YInteger, double YDouble = double.NaN, IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var f = YDouble;
            if (double.IsNaN(f))
                throw new Exception("DeltaFConvergence expected a double value (in the second argument, YDouble) "
                    + " representing the last calculated value of f.");
            var x = YDoubleArray1;
            if (x == null)
                throw new Exception("DeltaFConvergence expected a 1-D array of doubles (in the third argument, YDoubleArray1) "
                    + " representing the current decision vector, x.");
            if ((xlast == null) || (StarMath.norm1(x, xlast) > toleranceForSame))
            {
                Boolean result =(Math.Abs(f - flast) <= minDifference);
                xlast = x;
                flast = f;
                return result;
            }
            return false;
        }
    }
}