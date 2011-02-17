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

namespace OptimizationToolbox
{
    public class NelderMead : abstractOptMethod
    {
        #region Fields

        private readonly double chi = 2;
        private readonly double initNewPointAddition = 0.5;
        private readonly double initNewPointPercentage = 0.01;
        private readonly double psi = 0.5;
        private readonly double rho = 1;
        private readonly double sigma = 0.5;

        private readonly SortedList<double, double[]> vertices =
            new SortedList<double, double[]>(new optimizeSort(optimize.minimize));

        #endregion

        #region Constructor

        public NelderMead()
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
        }

        public NelderMead(double rho, double chi, double psi, double sigma, double initNewPointPercentage = double.NaN,
                          double initNewPointAddition = double.NaN)
            : this()
        {
            this.rho = rho;
            this.chi = chi;
            this.psi = psi;
            this.sigma = sigma;
            if (!double.IsNaN(initNewPointPercentage)) this.initNewPointPercentage = initNewPointPercentage;
            if (!double.IsNaN(initNewPointAddition)) this.initNewPointAddition = initNewPointAddition;
        }

        #endregion

        protected override double run(out double[] xStar)
        {
            vertices.Add(calc_f(x), x);
            // Creating neighbors in each direction and evaluating them
            for (var i = 0; i < n; i++)
            {
                var y = (double[])x.Clone();
                y[i] = (1 + initNewPointPercentage) * y[i] + initNewPointAddition;
                vertices.Add(calc_f(y), y);
            }

            while (notConverged(k, numEvals, vertices.Keys[0], vertices.Values[0], vertices.Values, null))
            {
                #region Compute the REFLECTION POINT

                // computing the average for each variable for n variables NOT n+1
                var Xm = new double[n];
                for (var dim = 0; dim < n; dim++)
                {
                    double sumX = 0;
                    for (var j = 0; j < n; j++)
                        sumX += vertices.Values[j][dim];
                    Xm[dim] = sumX / n;
                }

                var Xr = CloneVertex(vertices.Values[n]);
                for (var i = 0; i < n; i++)
                    Xr[i] = (1 + rho) * Xm[i] - rho * Xr[i];
                var fXr = calc_f(Xr);

                #endregion

                #region if reflection point is better than best

                if (fXr < vertices.Keys[0])
                {
                    #region Compute the Expansion Point

                    var Xe = CloneVertex(vertices.Values[n]);
                    for (var i = 0; i < n; i++)
                        Xe[i] = (1 + rho * chi) * Xm[i] - rho * chi * Xe[i];
                    var fXe = calc_f(Xe);

                    #endregion

                    if (fXe < fXr)
                    {
                        vertices.RemoveAt(n);
                        vertices.Add(fXe, Xe);
                        SearchIO.output("expand", 4);
                    }
                    else
                    {
                        vertices.RemoveAt(n);
                        vertices.Add(fXr, Xr);
                        SearchIO.output("reflect", 4);
                    }
                }
                #endregion
                #region if reflection point is NOT better than best

                else
                {
                    #region but it's better than second worst, still do reflect

                    if (fXr < vertices.Keys[n - 1])
                    {
                        vertices.RemoveAt(n);
                        vertices.Add(fXr, Xr);
                        SearchIO.output("reflect", 4);
                    }
                    #endregion

                    else
                    {
                        #region if better than worst, do Outside Contraction

                        if (fXr < vertices.Keys[n])
                        {
                            var Xc = CloneVertex(vertices.Values[n]);
                            for (var i = 0; i < n; i++)
                                Xc[i] = (1 + rho * psi) * Xm[i] - rho * psi * Xc[i];
                            var fXc = calc_f(Xc);

                            if (fXc <= fXr)
                            {
                                vertices.RemoveAt(n);
                                vertices.Add(fXc, Xc);
                                SearchIO.output("outside constract", 4);
                            }
                        #endregion
                            #region Shrink all others towards best

                            else
                            {
                                for (var j = 1; j < n + 1; j++)
                                {
                                    var Xs = CloneVertex(vertices.Values[j]);
                                    for (var i = 0; i < n; i++)
                                    {
                                        Xs[i] = vertices.Values[0][i]
                                                + sigma * (Xs[i] - vertices.Values[0][i]);
                                    }
                                    var fXs = calc_f(Xs);
                                    vertices.RemoveAt(j);
                                    vertices.Add(fXs, Xs);
                                }
                                SearchIO.output("shrink towards best", 4);
                            }

                            #endregion
                        }
                        else
                        {
                            #region Compute Inside Contraction

                            var Xcc = CloneVertex(vertices.Values[n]);
                            for (var i = 0; i < n; i++)
                                Xcc[i] = (1 - psi) * Xm[i] + psi * Xcc[i];
                            var fXcc = calc_f(Xcc);

                            if (fXcc < vertices.Keys[n])
                            {
                                vertices.RemoveAt(n);
                                vertices.Add(fXcc, Xcc);
                                SearchIO.output("inside contract", 4);
                            }
                            #endregion
                            #region Shrink all others towards best and flip over

                            else
                            {
                                for (var j = 1; j < n + 1; j++)
                                {
                                    var Xs = CloneVertex(vertices.Values[j]);
                                    for (var i = 0; i < n; i++)
                                    {
                                        Xs[i] = vertices.Values[0][i]
                                                - sigma * (Xs[i] - vertices.Values[0][i]);
                                    }
                                    var fXs = calc_f(Xs);
                                    vertices.RemoveAt(j);
                                    vertices.Add(fXs, Xs);
                                }
                                SearchIO.output("shrink towards best and flip", 4);
                            }

                            #endregion
                        }
                    }
                }

                #endregion

                k++;
                SearchIO.output("iter. = " + k, 2);
                ////mattica SearchIO.output("Fitness = " + vertices.Keys[0].ToString(), 2);
                SearchIO.output("Fitness = " + vertices.Keys[n], 2);
            } // END While Loop
            xStar = vertices.Values[0];
            fStar = vertices.Keys[0];
            vertices.Clear();
            return fStar;
        }

        private static double[] CloneVertex(double[] vertex)
        {
            return (double[])vertex.Clone();
        }
    }
}