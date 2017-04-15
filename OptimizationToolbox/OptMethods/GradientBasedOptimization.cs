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
using StarMathLib;

namespace OptimizationToolbox
{
    public class GradientBasedOptimization : abstractOptMethod
    {
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

        public GradientBasedOptimization()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = true;
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
            /* this is just to overcome a small issue with the compiler. It thinks that xStar will
             * not have a value since it only appears in a conditional statement below. This initi-
             * alization is to "ensure" that it does and that the code compiles. */
            xStar = null;
            //evaluate f(x0)
            fStar = fk = calc_f(x);
            do
            {
                gradF = calc_f_gradient(x);
                var step = StarMath.norm2(gradF);
                dk = searchDirMethod.find(x, gradF, fk, ref alphaStar);
                // use line search (arithmetic mean) to find alphaStar
                alphaStar = lineSearchMethod.findAlphaStar(x, dk, step);
                x = StarMath.add(x, StarMath.multiply(alphaStar, dk));
                SearchIO.output("iteration=" + k, 3);
                k++;
                fk = calc_f(x);
                if (fk < fStar)
                {
                    fStar = fk;
                    xStar = (double[])x.Clone();
                }
                SearchIO.output("f = " + fk, 3);
            } while (notConverged(k, numEvals, fk, x, null, gradF));

            return fStar;
        }

        #endregion

        /* alphaStar is what is returned by the line search (1-D) search method. It is used
         * to update xk. */
    }
}