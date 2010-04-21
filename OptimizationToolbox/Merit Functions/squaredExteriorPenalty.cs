using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
