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
    public class DeltaGradFConvergence : abstractConvergence
    {
        public DeltaGradFConvergence()
        {
        }

        public DeltaGradFConvergence(double minDifference)
        {
            this.minDifference = minDifference;
        }

        public double minDifference { get; set; }

        public override bool converged(long YInteger, double YDouble = double.NaN, IList<double> YDoubleArray1 = null,
                                       IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var gradf = YDoubleArray2;
            if (gradf == null)
                throw new Exception("DeltaGradFConvergence expected a 1-D array of doubles (in the fourth argument, YDoubleArray2) "
                                    + " representing the last calculated gradient of f.");
            return (StarMath.norm1(gradf) <= minDifference);
        }
    }
}