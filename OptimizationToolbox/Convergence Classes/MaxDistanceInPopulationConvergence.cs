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
    public class MaxDistanceInPopulationConvergence : abstractConvergence
    {
        #region Properties

        public double MinimumSideLength { get; set; }

        #endregion

        public MaxDistanceInPopulationConvergence()
        {
        }

        public MaxDistanceInPopulationConvergence(double MinimumSideLength)
        {
            this.MinimumSideLength = MinimumSideLength;
        }

        public override bool converged(long YInteger, double YDouble = double.NaN, IList<double> YDoubleArray1 = null,
                                       IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            var population = YJaggedDoubleArray;
            if (population == null)
                throw new Exception("MaxDistanceInPopulationConvergence expected an array of arrays of doubles (in the last argument, YJaggedDoubleArray) "
                                    + " representing the current simplex of solutions.");
            if (population.Count == 0) return false;
            double maxSideLength = 0;
            for (var i = 0; i < population.Count - 1; i++)
                for (var j = i + 1; j < population.Count; j++)
                {
                    var sideLengthSquared = StarMath.norm2(population[i], population[j], true);
                    if (maxSideLength < sideLengthSquared) maxSideLength = sideLengthSquared;
                }
            SearchIO.output("side length =" + Math.Sqrt(maxSideLength), 6);
            return (Math.Sqrt(maxSideLength) <= MinimumSideLength);
        }
    }
}