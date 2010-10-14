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

namespace OptimizationToolbox
{
    public class squaredExteriorPenalty : abstractMeritFunction
    {
        #region Constructor
        public squaredExteriorPenalty(abstractOptMethod optMethod, double penaltyWeight)
            : base(optMethod, penaltyWeight) { }
        #endregion

        public override double calcPenalty(double[] point)
        {
            double sum = 0.0;
            double temp;

            foreach (constraint c in optMethod.h)
            {
                temp = c.calculate(point);
                sum += temp * temp;
            }
            foreach (constraint c in optMethod.g)
            {
                temp = c.calculate(point);
                if (temp > 0) sum += temp * temp;
            }
            sum *= penaltyWeight;
            return sum;
        }
        public override double[] calcGradientOfPenalty(double[] point)
        {
            int n = point.GetLength(0);
            double[] grad = new double[n];
            double[] hvals = new double[optMethod.h.Count];
            double[] gvals = new double[optMethod.g.Count];
            for (int i = 0; i < optMethod.h.Count; i++)
                hvals[i] = optMethod.h[i].calculate(point);
            for (int i = 0; i < optMethod.g.Count; i++)
                gvals[i] = optMethod.g[i].calculate(point);

            for (int j = 0; j != n; j++)
            {
                double sum = 0.0;
                for (int i = 0; i < optMethod.h.Count; i++)
                    if (hvals[i] != 0.0)
                        sum += hvals[i] * optMethod.h[i].deriv_wrt_xi(point, j);

                for (int i = 0; i < optMethod.g.Count; i++)
                    if (gvals[i] > 0.0)
                        sum += gvals[i] * optMethod.g[i].deriv_wrt_xi(point, j);
                grad[j] = 2 * penaltyWeight * sum;
            }
            return grad;
        }
    }
}
