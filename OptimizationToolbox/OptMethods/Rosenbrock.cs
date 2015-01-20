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
    public class Rosenbrock : abstractOptMethod
    {
        #region Fields

        private readonly double alpha = 2;
        private readonly double beta = -0.5;
        private double initialStepSize = 1.0;
        private readonly double minimumStepSize = 1.0e-8;
        private readonly DirectSearchStepTooSmallConvergence stepTooSmallConvergence;

        #endregion

        #region Constructor

        public Rosenbrock()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = false;
            stepTooSmallConvergence = new DirectSearchStepTooSmallConvergence();
            ConvergenceMethods.Add(stepTooSmallConvergence);
        }

        public Rosenbrock(double alpha, double beta, double initialStepSize, double minimumStepSize)
            : this()
        {
            this.alpha = alpha;
            this.beta = beta;
            this.initialStepSize = initialStepSize;
            this.minimumStepSize = minimumStepSize;
        }

        #endregion

        protected override double run(out double[] xStar)
        {
            fStar = calc_f(x);
            // Set initial direction vectors to the basis vectors
            var directions = new double[n][];
            var distanceTraveled = new double[n];
            var steps = new double[n];
            for (int i = 0; i < n; i++)
            {
                var dir = new double[n];
                dir[i] = 1.0;
                directions[i] = dir;
            }
            // Now, start the main loop
            while (notConverged(k++, numEvals, fStar, x))
            {
                SearchIO.output("iter=" + k,2);
                // first, we move in each direction using simple line search
                for (var i = 0; i < n; i++)
                {
                    steps[i] = initialStepSize;
                    distanceTraveled[i] = 0.0;
                    var oneSuccess = false;
                    var oneFailure = false;
                    while (!(oneFailure && oneSuccess) && Math.Abs(steps[i]) > minimumStepSize)
                    {                                  
                        var xNew = x.add(directions[i].multiply(steps[i]));
                        var fNew = calc_f(xNew);
                        if (fNew < fStar)
                        {
                            oneSuccess = true;
                            x = xNew;
                            fStar = fNew;
                            distanceTraveled[i] += steps[i];
                            steps[i] *= alpha;
                        }
                        else
                        {                           
                            oneFailure = true;
                            steps[i] *= beta;
                        }
                    }
                }
                // finding new search directions   
                var newDirs = new double[n][];
                for (int i = 0; i < n; i++)
                {
                    newDirs[i] = new double[n];
                    for (int j = i; j < n; j++)
                        newDirs[i] = newDirs[i].add(directions[j].multiply(distanceTraveled[j]));
                }
                initialStepSize = newDirs[0].norm2();
                if (initialStepSize < minimumStepSize)
                {
                    stepTooSmallConvergence.hasConverged = true;
                    continue;
                }
                directions[0] = newDirs[0].divide(initialStepSize);
                for (int i = 1; i < n; i++)
                {
                    var newDir = (double[])newDirs[i].Clone();
                    for (int j = 0; j < i; j++)
                        newDir = newDir.subtract(directions[j].multiply(newDirs[i].dotProduct(directions[j])));
                    var length = newDir.norm2();
                    directions[i] = (length == 0) ? new double[n] : newDir.divide(length);
                }
                // Next, we update the directions using the Gram-Schmidt Orthogonalization
            }
            xStar = x;
            return fStar;
        }
    }
}

