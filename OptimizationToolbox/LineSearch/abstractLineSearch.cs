using System;

using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class abstractLineSearch
    {
        protected double epsilon;
        protected double stepSize;

        readonly Boolean trackFeasibility;
        List<constraint> infeasibles;
        public double lastFeasAlpha, lastFeasAlpha4G, lastFeasAlpha4H;
 
        protected abstractOptMethod optMethod;
        protected int k, kMax;


        #region Constructors
        public abstractLineSearch(double epsilon, double stepSize, int kMax, 
            Boolean trackFeasibility = false)
        {
            this.epsilon = epsilon;
            this.stepSize = stepSize;
            this.kMax = kMax;
            infeasibles = new List<constraint>();
            this.trackFeasibility = trackFeasibility;
        }
        #endregion

        public abstract double findAlphaStar(double[] x, double[] dir);
        public double findAlphaStar(double[] x, double[] dir, Boolean allowNegAlpha)
        { return findAlphaStar(x, dir, allowNegAlpha, stepSize); }
        public double findAlphaStar(double[] x, double[] dir, double initAlpha)
        { return findAlphaStar(x, dir, false, initAlpha); }
        /* the above 3 overloads all flow into this master one, which simply catches
         * all cases. The user still only needs to write the dervied classes only
         * implement the basic one that takes only x and dir. */
        public double findAlphaStar(double[] x, double[] dir, Boolean allowNegAlpha, double initAlpha)
        {
            double tempStepSize = stepSize;
            stepSize = initAlpha;

            double alpha1 = findAlphaStar(x, dir);
            if ((alpha1 < epsilon) && allowNegAlpha)
                alpha1 = findAlphaStar(x, StarMath.multiply(-1.0, dir));

            this.stepSize = tempStepSize;

            return alpha1;
        }

        protected double calcF(double[] start, double alpha, double[] dir)
        {
            double[] point = StarMath.add(start, StarMath.multiply(alpha, dir));
            if (trackFeasibility)
            {
                double temp = optMethod.calc_f(point, true);
                trackLastFeasible(point, alpha);
                return temp;
            }
            return optMethod.calc_f(point, true);
        }

        private void trackLastFeasible(double[] point, double alpha)
        {
            List<constraint> allConstraints = new List<constraint>(optMethod.h);
            allConstraints.AddRange(optMethod.g);

            foreach (constraint c in allConstraints)
                if ((c.feasible(point)) && (infeasibles.Contains(c))) infeasibles.Remove(c);
                else if ((!c.feasible(point)) && (!infeasibles.Contains(c))) infeasibles.Add(c);

            if (infeasibles.Count == 0) lastFeasAlpha = lastFeasAlpha4G = lastFeasAlpha4H = alpha;
            else if (!infeasibles.Exists(delegate(constraint ic)
            {
                return (ic.GetType() == typeof(equality));
            }))
                lastFeasAlpha4H = alpha;
            else if (!infeasibles.Exists(delegate(constraint ic)
            {
                return (ic.GetType() == typeof(inequality));
            }))
                lastFeasAlpha4G = alpha;
        }

        internal void SetOptimizationDetails(abstractOptMethod optMethod)
        {
            this.optMethod = optMethod;
        }
    }
}
