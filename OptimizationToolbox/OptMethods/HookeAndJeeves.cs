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
    public class HookeAndJeeves : abstractOptMethod
    {
        #region Fields

        private readonly double alpha = 2;
        private readonly double beta = -0.5;
        private double stepSize = 1.0;
        private readonly double minimumStepSize = 1.0e-8;
        private readonly DirectSearchStepTooSmallConvergence stepTooSmallConvergence;
        private readonly sameCandidate _sameCandidate;
        #endregion

        #region Constructor

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

        public HookeAndJeeves(double alpha, double beta, double initialStepSize, double minimumStepSize)
            : this()
        {
            this.alpha = alpha;
            this.beta = beta;
            this.stepSize = initialStepSize;
            this.minimumStepSize = minimumStepSize;
        }

        #endregion

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

