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
    public class DeltaXConvergence : abstractConvergence
    {
        private IList<double> xlast;

        #region Constructor

        public DeltaXConvergence()
        {
        }

        public DeltaXConvergence(double minDifference)
        {
            this.minDifference = minDifference;
        }

        #endregion

        public double minDifference { get; set; }

        public override bool converged(long YInteger, double YDouble = double.NaN, IList<double> YDoubleArray1 = null,
                                       IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var x = YDoubleArray1;
            if (x == null)
                throw new Exception("DeltaXConvergence expected a 1-D array of doubles (in the third argument, YDoubleArray1) "
                                    + " representing the current decision vector, x.");
            if ((xlast == null) || (StarMath.norm1(x, xlast) > minDifference))
            {
                xlast = x;
                return false;
            }
            return true;
        }
    }
}