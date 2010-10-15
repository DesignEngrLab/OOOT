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
using StarMathLib;

namespace OptimizationToolbox
{
    public class CyclicCoordinates : abstractSearchDirection
    {
        private int counter;
        private double[] xLast;

        public override double[] find(double[] x, double[] gradf, double f, ref double initAlpha, Boolean reset = false)
        {
            if (counter == 0) xLast = (double[])x.Clone();
            else if (counter == x.GetLength(0))
            {
                counter = 0;
                return StarMath.normalize(StarMath.subtract(x, xLast));
            }

            var d = new double[x.GetLength(0)];
            d[counter] = 1;
            counter++;
            return d;
        }
    }
}