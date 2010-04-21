using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class SQPSimpleHalver : abstractLineSearch
    {
        double gamma;
        #region Constructors
        public SQPSimpleHalver(abstractOptMethod optMethod, double gamma, int kMax)
            : base(optMethod, double.NaN, double.NaN, kMax)
        {
            this.gamma = gamma;
        }
        #endregion


        
        public override double findAlphaStar(double[] x, double[] dir)
        {
          double  f_at_current = calcF(x, 0, dir);
          double fnew;
            double alpha;
            int k = 0;
            do
            {
                alpha = stepSize/(Math.Pow(2,k));
                fnew = calcF(x, alpha, dir);
            }
              while ((fnew > (f_at_current -  gamma*stepSize*alpha))&& (k++ < kMax));

            /* truthfully, this already requires a full step of dk and then backs off on this
             * until a simple condition is satisfied - lookup convergence */
            return alpha;
        }
    }
}
