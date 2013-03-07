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
    public class PowellsOptimization : abstractOptMethod
    {
        private const double minAlphaStepRatio = 1e-6;
        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */
        private double alphaStar;
        private double[] dk;

        /* fk is the value of f(xk). */
        private double fk;
        private double[] gradF;

        #region Constructor

        public PowellsOptimization()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = true;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = false;
        }

        #endregion

        #region Main Function, run

        protected override double run(out double[] xStar)
        {
            xStar = null;
            var conjugateDirections = new List<double[]>();
            for (int i = 0; i < n; i++)
            {
                var direction = new double[n];
                direction[i] = 1;
                conjugateDirections.Add(direction);
            }
            fStar = fk = calc_f(x);
            do
            {
                var fBegin = fk;
                var xinner = (double[])x.Clone();
                var maxImprovement = 0.0;
                var indexOfMaxImprovement = -1;
                var maxAlpha = 0.0;
                var indexOfMaxAlpha = -1;
                var minAlpha = double.PositiveInfinity;
                var indexOfMinAlpha = -1;
                for (int i = 0; i < n; i++)
                {
                    var dk = conjugateDirections[i];
                    alphaStar = lineSearchMethod.findAlphaStar(xinner, dk, true);
                    xinner = StarMath.add(xinner, StarMath.multiply(alphaStar, dk));
                    var fNew = calc_f(xinner);
                    if (Math.Abs(alphaStar) > Math.Abs(maxAlpha))
                    {
                        maxAlpha = alphaStar;
                        indexOfMaxAlpha = i;
                    }
                    if (Math.Abs(alphaStar) < Math.Abs(minAlpha))
                    {
                        minAlpha = alphaStar;
                        indexOfMinAlpha = i;
                    }
                    if (fk - fNew > maxImprovement)
                    {
                        maxImprovement = fk - fNew;
                        indexOfMaxImprovement = i;
                    }
                    fk = fNew;
                    k++;
                }
                if (Math.Abs(maxAlpha) < minAlphaStepRatio || Math.Abs(minAlpha / maxAlpha) < minAlphaStepRatio)
                {
                    conjugateDirections.Clear();
                    for (int i = 0; i < n; i++)
                    {
                        var direction = new double[n];
                        direction[i] = 1;
                        conjugateDirections.Add(direction);
                    }
                }
                else
                {
                    /*combine direction search. */
                    var xJump = StarMath.subtract(StarMath.multiply(2, xinner), x, n);
                    var fJump = calc_f(xJump);
                    if (fJump >= fBegin
                        || (fBegin - 2 * fk + fJump) * (fBegin - fk - maxImprovement) * (fBegin - fk - maxImprovement)
                        >= (fBegin - fJump) * (fBegin - fJump) * maxImprovement / 2)
                        x = xinner;
                    else
                    {
                        var combinedDir = StarMath.normalize(StarMath.subtract(xinner, x, n));
                        alphaStar = lineSearchMethod.findAlphaStar(xinner, combinedDir, true);
                        x = StarMath.add(xinner, StarMath.multiply(alphaStar, combinedDir));
                        conjugateDirections.RemoveAt(indexOfMaxImprovement);
                        conjugateDirections.Add(combinedDir);
                        k++;
                        fk = calc_f(x);
                    }
                }
                if (fk < fStar)
                {
                    fStar = fk;
                    xStar = (double[])x.Clone();
                }
                SearchIO.output("iteration=" + k, 3);
                SearchIO.output("f = " + fk, 3);
            } while (notConverged(k, numEvals, fk, x, null, gradF));
            return fStar;
        }

        #endregion
    }
}