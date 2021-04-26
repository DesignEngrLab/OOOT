// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="HookeAndJeeves.cs" company="OptimizationToolbox">
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

using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class HookeAndJeeves.
    /// Implements the <see cref="OptimizationToolbox.abstractOptMethod" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractOptMethod" />
    public class HookeAndJeeves : abstractOptMethod
    {
        #region Fields

        /// <summary>
        /// The alpha
        /// </summary>
        private readonly double alpha = 2;
        /// <summary>
        /// The beta
        /// </summary>
        private readonly double beta = -0.5;
        /// <summary>
        /// The step size
        /// </summary>
        private double stepSize = 1.0;
        /// <summary>
        /// The minimum step size
        /// </summary>
        private readonly double minimumStepSize = 1.0e-8;
        /// <summary>
        /// The step too small convergence
        /// </summary>
        private readonly DirectSearchStepTooSmallConvergence stepTooSmallConvergence;
        /// <summary>
        /// The same candidate
        /// </summary>
        private readonly sameCandidate _sameCandidate;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HookeAndJeeves"/> class.
        /// </summary>
        public HookeAndJeeves()
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
            _sameCandidate = new sameCandidate(Parameters.ToleranceForSame);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HookeAndJeeves"/> class.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="beta">The beta.</param>
        /// <param name="initialStepSize">Initial size of the step.</param>
        /// <param name="minimumStepSize">Minimum size of the step.</param>
        public HookeAndJeeves(double alpha, double beta, double initialStepSize, double minimumStepSize)
            : this()
        {
            this.alpha = alpha;
            this.beta = beta;
            this.stepSize = initialStepSize;
            this.minimumStepSize = minimumStepSize;
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
            fStar = calc_f(x);
            var xBase = x;
            var fBase = fStar;
            if (!explore(out stepSize))
            {
                xStar = xBase;
                return fBase;
            }
            var xAfterExplore = x;
            var fAfterExplore = fStar = calc_f(x);
            while (notConverged(k++, numEvals, fStar, x))
            {
                SearchIO.output("iter=" + k, 2);
                var xProject = x = xAfterExplore.add(xAfterExplore.subtract(xBase).multiply(alpha));
                fStar = calc_f(x);
                stepSize =Math.Max(stepSize, xProject.subtract(xAfterExplore).norm2());
                double nextStepSize;
                if (explore(out nextStepSize) && fStar < fAfterExplore)
                {
                    xBase = xProject;
                    xAfterExplore = x;
                    fAfterExplore = fStar;
                    stepSize = nextStepSize;
                }
                else
                {
                    SearchIO.output("explore failed", 5);
                    x = xAfterExplore;
                    fStar = fAfterExplore;
                    if (explore(out nextStepSize))
                    {
                        xBase = xAfterExplore;
                        xAfterExplore = x;
                        fAfterExplore = fStar;
                        stepSize = nextStepSize;
                    }
                    else stepTooSmallConvergence.hasConverged = true;
                }
            }
            xStar = x;
            return fStar;
        }

        /// <summary>
        /// Explores the specified this step size.
        /// </summary>
        /// <param name="thisStepSize">Size of the this step.</param>
        /// <returns>Boolean.</returns>
        private Boolean explore(out double thisStepSize)
        {
            var success = false;
            thisStepSize = stepSize / beta;
            do
            {
                thisStepSize *= beta;
                for (var i = 0; i < n; i++)
                {
                    var direction = new double[n];
                    direction[i] = 1;
                    var xNew = x.add(direction.multiply(thisStepSize));
                    var fNew = calc_f(xNew);
                    if (fNew < fStar)
                    {
                        success = true;
                        x = xNew;
                        fStar = fNew;
                    }
                    else
                    {
                        xNew = x.add(direction.multiply(-1 * thisStepSize));
                        fNew = calc_f(xNew);
                        if (fNew < fStar)
                        {
                            success = true;
                            x = xNew;
                            fStar = fNew;
                        }
                    }
                }
            } while (!success && thisStepSize * beta > minimumStepSize);
            return success;
        }
    }
}

