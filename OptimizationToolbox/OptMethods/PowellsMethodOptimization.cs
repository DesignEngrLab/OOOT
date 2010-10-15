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
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public class PowellsMethodOptimization : abstractOptMethod
    {
        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */
        private double alphaStar;
        private double[] dk;

        /* fk is the value of f(xk). */
        private double fk;


        /* alphaStar is what is returned by the line search (1-D) search method. It is used
         * to update xk. */
        private int searchDirColumn;
        //Each column represents a potential search direction
        private double[,] searchDirMatrix;

        #region Constructor

        public PowellsMethodOptimization()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            RequiresMeritFunction = true;
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
            #region Initialization

            xStar = null;
            //evaluate f(x0)
            fStar = fk = calc_f(x);
            dk = new double[n];

            #endregion

            do
            {
                /* Powell's optimization description
                 * Build n conjugate directions sweeping thru the searchDirMatrix
                n+1 times replacing the last swept column with new conjugate direction
                Start by leaving the first conjugate direction as the nth coord direction
                 
                 * For each conjStep sweep from newest conjugate step all the way back around
                 and then re-search in that same conjugate direction but starting with 
                 the new point arrived at after searching the other directions in between.
                 
                 * Then Powell's method says that direction created by joining the two local line
                 minimas of the last created conjugate direction will create the new conjugate direction
                 Repeat this until all conjugate directions are created*/

                /*Foreach iteration of Powell's optimization reset the search directions back
                 * to the coordinate directions*/

                #region Generate and search Conjugate directions

                searchDirMatrix = StarMath.makeIdentity(n);
                for (var conjStep = 0; conjStep < n; conjStep++)
                {
                    var dconj = new double[n];
                    if (conjStep == 0)
                        searchDirColumn = n - 1;
                    else
                        searchDirColumn = conjStep - 1;
                    //Search n+1 directions
                    for (var i = 0; i < n + 1; i++)
                    {
                        dk = StarMath.GetColumn(searchDirColumn, searchDirMatrix);
                        // use line search (arithmetic mean) to find alphaStar
                        alphaStar = lineSearchMethod.findAlphaStar(x, dk, true);
                        var xl = StarMath.multiply(alphaStar, dk);
                        x = StarMath.add(x, xl);
                        if (i > 0)
                            dconj = StarMath.add(dconj, xl);
                        searchDirColumn++;
                        if (searchDirColumn == n) searchDirColumn = 0;
                    }
                    StarMath.SetColumn(conjStep, searchDirMatrix, dconj);
                }

                #endregion

                #region Finally search along all the conjugate directions now in the matrix

                for (var searchDirNum = 0; searchDirNum < n; searchDirNum++)
                {
                    dk = StarMath.GetColumn(searchDirNum, searchDirMatrix);
                    // use line search (arithmetic mean) to find alphaStar
                    alphaStar = lineSearchMethod.findAlphaStar(x, dk, true);

                    x = StarMath.add(x, StarMath.multiply(alphaStar, dk));
                }

                #endregion

                #region Output, Check if fstar should be updated, Increment k

                SearchIO.output("iteration=" + k, 3);
                k++;
                fk = calc_f(x);
                if (fk < fStar)
                {
                    fStar = fk;
                    xStar = (double[])x.Clone();
                }
                SearchIO.output("f = " + fk, 3);

                #endregion
            } while (notConverged(k, fk, x, null, new List<double[]>()));

            return fStar;
        }

        #endregion
    }
}