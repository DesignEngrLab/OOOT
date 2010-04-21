using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class abstractMeritFunction
    {
       protected abstractOptMethod optMethod;
        public double penaltyWeight {get; set; }
        public abstractMeritFunction(abstractOptMethod optMethod, double penaltyWeight)
        {
            this.optMethod = optMethod;
            this.penaltyWeight = penaltyWeight;
        }

        public abstract double[] calcGradientOfPenalty(double[] point);
        public abstract double calcPenalty(double[] point);
    }
}