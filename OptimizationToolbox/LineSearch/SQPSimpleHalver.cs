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

namespace OptimizationToolbox
{
    public class SQPSimpleHalver : abstractLineSearch
    {
        private readonly double gamma;

        #region Constructors

        public SQPSimpleHalver(double gamma, int kMax)
            : base(double.NaN, double.NaN, kMax)
        {
            this.gamma = gamma;
        }

        #endregion

        public override double findAlphaStar(double[] x, double[] dir)
        {
            var f_at_current = calcF(x, 0, dir);
            double fnew;
            double alpha;
            k = 0;
            do
            {
                alpha = stepSize / (Math.Pow(2, k));
                fnew = calcF(x, alpha, dir);
            } while ((fnew > (f_at_current - gamma * stepSize * alpha)) && (k++ < kMax));

            /* truthfully, this already requires a full step of dk and then backs off on this
             * until a simple condition is satisfied - lookup convergence */
            return alpha;
        }
    }
}