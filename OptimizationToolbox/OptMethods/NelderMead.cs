// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="NelderMead.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT X11 License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT X11 License for more details.
 *  


 *     
 *     Please find further details and contact information on OOOT
 *     at http://designengrlab.github.io/OOOT/.
 *************************************************************************/
using System.Collections.Generic;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class NelderMead.
    /// Implements the <see cref="OptimizationToolbox.abstractOptMethod" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractOptMethod" />
    public class NelderMead : abstractOptMethod
    {
        #region Fields

        /// <summary>
        /// The chi
        /// </summary>
        private readonly double chi = 2;
        /// <summary>
        /// The initialize new point addition
        /// </summary>
        private readonly double initNewPointAddition = 0.5;
        /// <summary>
        /// The initialize new point percentage
        /// </summary>
        private readonly double initNewPointPercentage = 0.01;
        /// <summary>
        /// The psi
        /// </summary>
        private readonly double psi = 0.5;
        /// <summary>
        /// The rho
        /// </summary>
        private readonly double rho = 1;
        /// <summary>
        /// The sigma
        /// </summary>
        private readonly double sigma = 0.5;

        /// <summary>
        /// The vertices
        /// </summary>
        private readonly SortedList<double, double[]> vertices =
            new SortedList<double, double[]>(new optimizeSort(optimize.minimize));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NelderMead"/> class.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="NelderMead"/> class.
        /// </summary>
        /// <param name="rho">The rho.</param>
        /// <param name="chi">The chi.</param>
        /// <param name="psi">The psi.</param>
        /// <param name="sigma">The sigma.</param>
        /// <param name="initNewPointPercentage">The initialize new point percentage.</param>
        /// <param name="initNewPointAddition">The initialize new point addition.</param>
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

        /// <summary>
        /// Runs the specified optimization method. This includes the details
        /// of the optimization method.
        /// </summary>
        /// <param name="xStar">The x star.</param>
        /// <returns>System.Double.</returns>
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

            while (notConverged(k, numEvals, vertices.Keys[0], vertices.Values[0], vertices.Values))
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
                SearchIO.output("x_r = " + StarMathLib.StarMath.MakePrintString(Xr), 4);
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

                    vertices.RemoveAt(n);  // remove the worst
                    if (fXe < fXr)
                    {
                        vertices.Add(fXe, Xe);
                        SearchIO.output("expand", 4);
                    }
                    else
                    {
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
                        vertices.RemoveAt(n);  // remove the worst
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
                                vertices.RemoveAt(n);  // remove the worst
                                vertices.Add(fXc, Xc);
                                SearchIO.output("outside constract", 4);
                            }
                        #endregion
                            #region Shrink all others towards best
                            else
                            {
                                var newXs = new List<double[]>();
                                for (var j = n; j >= 1; j--)
                                {
                                    var Xs = CloneVertex(vertices.Values[j]);
                                    for (var i = 0; i < n; i++)
                                        Xs[i] = vertices.Values[0][i]
                                                    + sigma * (Xs[i] - vertices.Values[0][i]);
                                    newXs.Add(Xs);
                                    vertices.RemoveAt(j);
                                }
                                for (int j = 0; j < n; j++)
                                    vertices.Add(calc_f(newXs[j]), newXs[j]);
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
                                vertices.RemoveAt(n);  // remove the worst
                                vertices.Add(fXcc, Xcc);
                                SearchIO.output("inside contract", 4);
                            }
                            #endregion
                            #region Shrink all others towards best and flip over

                            else
                            {
                                var newXs = new List<double[]>();
                                for (var j = n; j >= 1; j--)
                                {
                                    var Xs = CloneVertex(vertices.Values[j]);
                                    for (var i = 0; i < n; i++)
                                        Xs[i] = vertices.Values[0][i]
                                                    - sigma * (Xs[i] - vertices.Values[0][i]);
                                    newXs.Add(Xs);
                                    vertices.RemoveAt(j);
                                }
                                for (int j = 0; j < n; j++)
                                    vertices.Add(calc_f(newXs[j]), newXs[j]);
                                SearchIO.output("shrink towards best and flip", 4);
                            }

                            #endregion
                        }
                    }
                }

                #endregion

                k++;
                SearchIO.output("iter. = " + k, 2);
                SearchIO.output("Fitness = " + vertices.Keys[0], 2);
            } // END While Loop
            xStar = vertices.Values[0];
            fStar = vertices.Keys[0];
            vertices.Clear();
            return fStar;
        }

        /// <summary>
        /// Clones the vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>System.Double[].</returns>
        private static double[] CloneVertex(double[] vertex)
        {
            return (double[])vertex.Clone();
        }
    }
}