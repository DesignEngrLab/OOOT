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
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SequentialQuadraticProgramming : abstractOptMethod
    {
        #region Fields
        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */
        double[] gradF, dk;

        /* fk is the value of f(xk). */
        double fk;

        /* alphaStar is what is returned by the line search (1-D) search method. It is used
         * to update xk. */
        double alphaStar;

        /* A is the gradient of the active constraints as an m by n matrix. */
        double[,] A;
        double[] activeVals;

        /* Lagrange multipliers for active constraints. */
        double[] lambdas;
        #endregion

        #region Constructor
        public SequentialQuadraticProgramming()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = false;
            RequiresMeritFunction = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = true;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = false;

            this.Add(new SQPSimpleHalver(0.25, 100));
            this.lineSearchMethod.SetOptimizationDetails(this);
            this.Add(new linearExteriorPenaltyMax(this, 1.0));
            this.Add(new MultipleANDConvergenceConditions(5, 0.01, 0.01));
        }
        #endregion

        #region Main Function, run
        protected override double run(out double[] xStar)
        {
            //evaluate f(x0)
            fStar = fk = calc_f(x);

            // the search direction is initialized.
            dk = new double[n];

            double initAlpha;
            active.AddRange(h);

            do
            {
                gradF = calc_f_gradient(x);
                A = formulateActiveSetAndGradients(x);
                dk = calculateSQPSearchDirection(x, out initAlpha);
                meritFunction.penaltyWeight = adjustMeritPenalty();
                // this next function is not part of the regular SQP algorithm
                // it's only intended to keep all the points in the positive space.
                initAlpha = preventNegatives(x, dk, initAlpha);
                //

                alphaStar = lineSearchMethod.findAlphaStar(x, dk, initAlpha);
                x = StarMath.add(x, StarMath.multiply(alphaStar, dk));
                SearchIO.output("iteration=" + k, 3);
                SearchIO.output("--alpha=" + alphaStar, 3);
                k++;
                fk = calc_f(x);

                SearchIO.output("----f = " + fk, 3);
                SearchIO.output("---#active =" + active.Count, 3);
            }
            while (notConverged(k, fk, x, gradF, new List<double[]>()));
            fStar = fk;
            xStar = (double[])x.Clone();
            return fStar;
        }

        private double adjustMeritPenalty()
        {
            double weight = StarMath.norm1(lambdas);

            /* what is the point of the next condition? is it a correct step when one is still in the
             * infeasible region? */
            if (weight < meritFunction.penaltyWeight)
                return 2 * meritFunction.penaltyWeight;
             return weight;
        }

        private double preventNegatives(double[] xk, double[] dk, double initAlpha)
        {
            for (int i = 0; i < dk.GetLength(0); i++)
                if (dk[i] < 0)
                    initAlpha = Math.Min(initAlpha, -xk[i] / dk[i]);
            return initAlpha;
        }
        #endregion

        /// <summary>
        /// Formulates the active set and gradients.
        /// </summary>
        /// <param name="xk">The xk.</param>
        /// <returns>the gradient of the active constraints as an m by n matrix.</returns>
        private double[,] formulateActiveSetAndGradients(double[] xk)
        {
            /* this list is ordered from largest to smallest. We only want to introduce ONE new member 
             * into the active set. Which would be the first element (i.e.value) of this sorted list. */

            SortedList<double, constraint> newInfeasibles
                = new SortedList<double, constraint>(new optimizeSort(optimize.maximize));

            /* this foreach loop can remove any number of g's from the active set, and puts
             * any newly violated g's into the sorted list newInfeasbles. */
            foreach (inequality c in g)
            {
                double gVal = c.calculate(xk);
                if (gVal < 0.0)
                {
                    if (active.Contains(c)) active.Remove(c);
                }
                else
                    newInfeasibles.Add(gVal, c);
            }
            /* only want to add one newInfeasible to the list of active. Hence the break statement. */
            foreach (KeyValuePair<double, constraint> kvp in newInfeasibles)
                if (!active.Contains(kvp.Value))
                {
                    active.Add(kvp.Value);
                    break;
                }
            m = active.Count;
            return calc_active_gradient(xk);
        }

        /// <summary>
        /// Calculates the SQP search direction.
        /// </summary>
        /// <param name="xk">The xk.</param>
        /// <param name="gradF">The grad F.</param>
        /// <param name="A">The A.</param>
        /// <param name="initAlpha">The init alpha.</param>
        /// <returns></returns>
        private double[] calculateSQPSearchDirection(double[] xk, out double initAlpha)
        {
            activeVals = calc_active_vector(xk);
            double[] dir;
            double[,] At = StarMath.transpose(A);
            double[,] invAAt = StarMath.inverse(StarMath.multiply(A, At));
            double[,] AtinvAAt = StarMath.multiply(At, invAAt);
            double[,] J = StarMath.makeIdentity(n);
            double[,] invJ = StarMath.makeIdentity(n);

            // double[,] P = StarMath.subtract(StarMath.makeIdentity(n), AtinvAAtA);

            double[,] P = StarMath.multiply(invJ,
                StarMath.subtract(StarMath.multiply(AtinvAAt, A), StarMath.makeIdentity(n)));
            //double[,] Q = StarMath.multiply(invJ,
            //        StarMath.multiply(AtinvAAt, StarMath.multiply(A,
            //        StarMath.multiply(J, StarMath.multiply(invAtA, At)))));
            double[,] Q = AtinvAAt;
            double[] dirMin = StarMath.multiply(P, gradF);
            double[] dirCC = StarMath.multiply(Q, activeVals);
            dir = StarMath.subtract(dirMin, dirCC);

            lambdas = StarMath.multiply(invAAt, StarMath.subtract(
                activeVals, StarMath.multiply(A, gradF)));

            if (double.IsNaN(dir[0])) dir = dk;
            initAlpha = StarMath.norm2(dir);
            if (initAlpha > 0) dir = StarMath.multiply((1.0 / initAlpha), dir);
            return dir;
        }
    }
}