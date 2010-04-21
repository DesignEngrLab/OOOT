using System;
using System.Collections.Generic;


namespace OptimizationToolbox
{
    public class linearExteriorPenaltyMax : abstractMeritFunction
    {
        #region Constructor
        public linearExteriorPenaltyMax(abstractOptMethod optMethod, double penaltyWeight)
            : base(optMethod, penaltyWeight) { }
        #endregion

        public override double calcPenalty(double[] point)
        {
            double max = 0.0;
            double temp;

            foreach (constraint c in optMethod.h)
            {
                temp = c.calculate(point);
                max = Math.Max(max, Math.Abs(temp));
            }
            foreach (constraint c in optMethod.g)
            {
                temp = c.calculate(point);
                max = Math.Max(max, temp);
            }
            max *= penaltyWeight;
            return max;
        }
        public override double[] calcGradientOfPenalty(double[] point)
        {
            int n = point.GetLength(0);
            double[] grad = new double[n];
            double temp;
            double max = 0.0;
            int hMaxIndex = -1;
            int gMaxIndex = -1;
            for (int j = 0; j < optMethod.h.Count; j++)
            {
                temp = optMethod.h[j].calculate(point);
                if (Math.Abs(temp) > max)
                {
                    max = Math.Abs(temp);
                    hMaxIndex = j;
                }
            }
            for (int j = 0; j < optMethod.g.Count; j++)
            {
                temp = optMethod.g[j].calculate(point);
                if (temp > max)
                {
                    max = temp;
                    gMaxIndex = j;
                }
            }
            if (gMaxIndex == -1)
                // this means the max was an h.
                for (int i = 0; i != n; i++)
                    grad[i] = penaltyWeight * optMethod.h[hMaxIndex].deriv_wrt_xi(point, i);
            else
                // this means the max was a g.
                for (int i = 0; i != n; i++)
                    grad[i] = penaltyWeight * optMethod.g[gMaxIndex].deriv_wrt_xi(point, i);

            return grad;
        }
    }
}
