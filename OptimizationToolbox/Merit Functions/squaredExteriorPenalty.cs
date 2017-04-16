/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT X11 License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT X11 License for more details.
 *  


 *     
 *     Please find further details and contact information on OOOT
 *     at http://designengrlab.github.io/OOOT/.
 *************************************************************************/

namespace OptimizationToolbox
{
    public class squaredExteriorPenalty : abstractMeritFunction
    {
        #region Constructor

        public squaredExteriorPenalty(abstractOptMethod optMethod, double penaltyWeight)
            : base(optMethod, penaltyWeight)
        {
        }

        #endregion

        public override double calcPenalty(double[] point)
        {
            var sum = 0.0;
            double temp;

            foreach (var c in optMethod.h)
            {
                temp = optMethod.calculate(c, point);
                sum += temp * temp;
            }
            foreach (var c in optMethod.g)
            {
                temp = optMethod.calculate(c, point);
                if (temp > 0) sum += temp * temp;
            }
            sum *= penaltyWeight;
            return sum;
        }

        public override double[] calcGradientOfPenalty(double[] point)
        {
            var n = point.GetLength(0);
            var grad = new double[n];
            var hvals = new double[optMethod.h.Count];
            var gvals = new double[optMethod.g.Count];
            for (var i = 0; i < optMethod.h.Count; i++)
                hvals[i] = optMethod.calculate(optMethod.h[i], point);
            for (var i = 0; i < optMethod.g.Count; i++)
                gvals[i] = optMethod.calculate(optMethod.g[i], point);

            for (var j = 0; j != n; j++)
            {
                var sum = 0.0;
                for (var i = 0; i < optMethod.h.Count; i++)
                    if (hvals[i] != 0.0)
                        sum += hvals[i] * optMethod.deriv_wrt_xi(optMethod.h[i], point, j);

                for (var i = 0; i < optMethod.g.Count; i++)
                    if (gvals[i] > 0.0)
                        sum += gvals[i] * optMethod.deriv_wrt_xi(optMethod.g[i], point, j);
                grad[j] = 2 * penaltyWeight * sum;
            }
            return grad;
        }
    }
}