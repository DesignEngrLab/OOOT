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

namespace OptimizationToolbox
{
    public class linearExteriorPenaltyMax : abstractMeritFunction
    {
        #region Constructor

        public linearExteriorPenaltyMax(abstractOptMethod optMethod, double penaltyWeight)
            : base(optMethod, penaltyWeight)
        {
        }

        #endregion

        public override double calcPenalty(double[] point)
        {
            var max = 0.0;
            double temp;

            foreach (IConstraint c in optMethod.h)
            {
                temp = optMethod.calculate(c, point);
                max = Math.Max(max, Math.Abs(temp));
            }
            foreach (IConstraint c in optMethod.g)
            {
                temp = optMethod.calculate(c, point);
                max = Math.Max(max, temp);
            }
            max *= penaltyWeight;
            return max;
        }

        public override double[] calcGradientOfPenalty(double[] point)
        {
            var n = point.GetLength(0);
            var grad = new double[n];
            double temp;
            var max = 0.0;
            var hMaxIndex = -1;
            var gMaxIndex = -1;
            for (var j = 0; j < optMethod.h.Count; j++)
            {
                temp =optMethod.calculate(optMethod.h[j],point);
                if (Math.Abs(temp) > max)
                {
                    max = Math.Abs(temp);
                    hMaxIndex = j;
                }
            }
            for (var j = 0; j < optMethod.g.Count; j++)
            {
                temp = optMethod.calculate(optMethod.g[j], point);
                if (temp > max)
                {
                    max = temp;
                    gMaxIndex = j;
                }
            }
            if (gMaxIndex == -1)
                // this means the max was an h.
                for (var i = 0; i != n; i++)
                    grad[i] = penaltyWeight * optMethod.deriv_wrt_xi(optMethod.h[hMaxIndex], point, i);
            else
                // this means the max was a g.
                for (var i = 0; i != n; i++)
                    grad[i] = penaltyWeight * optMethod.deriv_wrt_xi(optMethod.g[gMaxIndex], point, i);

            return grad;
        }
    }
}