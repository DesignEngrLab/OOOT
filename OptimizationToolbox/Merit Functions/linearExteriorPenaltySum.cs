using System;
using System.Collections.Generic;


namespace OptimizationToolbox
{
   public class linearExteriorPenaltySum:abstractMeritFunction
    {
        #region Constructor
       public linearExteriorPenaltySum(abstractOptMethod optMethod, double penaltyWeight)
            : base(optMethod, penaltyWeight) { }
        #endregion

        public override double calcPenalty(double[] point)
        {
            double sum = 0.0;
            double temp;

            foreach (constraint c in optMethod.h)
            {
                temp = c.calculate(point);
                sum += Math.Abs(temp);
            }
            foreach (constraint c in optMethod.g)
            {
                temp = c.calculate(point);
                if (temp > 0) sum += temp;
            }
            sum *= penaltyWeight;
            return sum;
        }
        public override double[] calcGradientOfPenalty(double[] point)
        {
            int n = point.GetLength(0);
            double[] grad = new double[n];
            double temp;
            for (int i = 0; i != n; i++)
            {
                double sum = 0.0;
                foreach (constraint c in optMethod.h)
                {
                    temp = c.calculate(point);
                    if (temp < 0.0)
                        sum -= c.deriv_wrt_xi(point, i);
                    if (temp>0.0)
                    sum += c.deriv_wrt_xi(point, i);
                }
                foreach (constraint c in optMethod.g)
                {
                    temp = c.calculate(point);
                    if (temp > 0.0)
                        sum +=  c.deriv_wrt_xi(point, i);
                }
                grad[i] =  penaltyWeight * sum;
            }
            return grad;
        }
    }
}
