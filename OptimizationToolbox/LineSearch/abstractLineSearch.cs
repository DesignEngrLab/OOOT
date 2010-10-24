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
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// </summary>
    public abstract class abstractLineSearch
    {
        private readonly List<IConstraint> infeasibles;
        private readonly Boolean trackFeasibility;
        /// <summary>
        /// the tolerance value, epsilon is used to distinguish values of alpha. It is part
        /// of the convergence for the line search.
        /// </summary>
        protected double epsilon;
        protected int k, kMax;
        public double lastFeasAlpha, lastFeasAlpha4G, lastFeasAlpha4H;

        protected abstractOptMethod optMethod;
        protected double stepSize;

        #region Constructors

        protected abstractLineSearch(double epsilon, double stepSize, int kMax,
                                  Boolean trackFeasibility = false)
        {
            this.epsilon = epsilon;
            this.stepSize = stepSize;
            this.kMax = kMax;
            infeasibles = new List<IConstraint>();
            this.trackFeasibility = trackFeasibility;
        }

        #endregion

        public abstract double findAlphaStar(double[] x, double[] dir);

        public double findAlphaStar(double[] x, double[] dir, Boolean allowNegAlpha)
        {
            return findAlphaStar(x, dir, allowNegAlpha, stepSize);
        }

        public double findAlphaStar(double[] x, double[] dir, double initAlpha)
        {
            return findAlphaStar(x, dir, false, initAlpha);
        }

        /* the above 3 overloads all flow into this master one, which simply catches
         * all cases. The user still only needs to write the dervied classes only
         * implement the basic one that takes only x and dir. */

        public double findAlphaStar(double[] x, double[] dir, Boolean allowNegAlpha, double initAlpha)
        {
            var tempStepSize = stepSize;
            stepSize = initAlpha;

            var alpha1 = findAlphaStar(x, dir);
            if ((alpha1 < epsilon) && allowNegAlpha)
                alpha1 = -findAlphaStar(x, StarMath.multiply(-1.0, dir));

            stepSize = tempStepSize;

            return alpha1;
        }

        protected double calcF(double[] start, double alpha, double[] dir)
        {
            var point = StarMath.add(start, StarMath.multiply(alpha, dir));
            if (trackFeasibility)
            {
                var temp = optMethod.calc_f(point, true);
                trackLastFeasible(point, alpha);
                return temp;
            }
            return optMethod.calc_f(point, true);
        }

        private void trackLastFeasible(double[] point, double alpha)
        {
            var allConstraints = optMethod.h.Cast<IConstraint>().ToList();
            allConstraints.AddRange(optMethod.g.Cast<IConstraint>());

            foreach (var c in allConstraints)
                if ((optMethod.feasible(c,point)) && (infeasibles.Contains(c))) infeasibles.Remove(c);
                else if ((!optMethod.feasible(c, point)) && (!infeasibles.Contains(c))) infeasibles.Add(c);

            if (infeasibles.Count == 0) lastFeasAlpha = lastFeasAlpha4G = lastFeasAlpha4H = alpha;
            else if (!infeasibles.Exists(ic => (typeof(IEquality).IsInstanceOfType(ic))))
                lastFeasAlpha4H = alpha;
            else if (!infeasibles.Exists(ic => (typeof(IInequality).IsInstanceOfType(ic))))
                lastFeasAlpha4G = alpha;
        }

        internal void SetOptimizationDetails(abstractOptMethod optmethod)
        {
            this.optMethod = optmethod;
        }
    }
}