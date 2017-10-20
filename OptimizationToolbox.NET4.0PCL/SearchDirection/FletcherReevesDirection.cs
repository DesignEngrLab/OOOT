﻿/*************************************************************************
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
using StarMathLib;

namespace OptimizationToolbox
{
    public class FletcherReevesDirection : abstractSearchDirection
    {
        private double[] dir;
        private double[] dirLast; //last search direction
        private double magGradF; //the magnitude of the current gradient of f
        private double magGradFLast; //the magnitude of the last gradient of f

        public override double[] find(double[] x, double[] gradf, double f, ref double initAlpha, bool reset = false)
        {
            /* if a TRUE is sent to reset, then we call a simple steepestDescent function. */
            if (reset) return steepestDescentReset(gradf);

            /* calc the magnitude of the new gradient, magGradF. This is used several
             * times so in order to minimize time, calc it once and save it. */
            magGradF = gradf.norm2();
            /* if it is all zeros then just return it. This is the only vector that can't be
             * normalized. */
            if (magGradF == 0) dir = gradf;
            else dir = (StarMath.multiply((-1.0 / magGradF), gradf));

            /* add the inertia from the last direction - if there was a last dir, dirLast */
            if ((magGradFLast != 0.0) && (dirLast != null))
                dir = dir.add(StarMath.multiply((magGradF / magGradFLast), dirLast));

            /* we no longer need magGradFLast, so we write over it with the new value. */
            magGradFLast = magGradF;

            /* now dir is better, but no longer a unit vector. Readjust and save as the 
             * new last values. */
            magGradF = dir.norm2();
            /* again, it's possible that it'll be all zeros. This happens when dirLast is in 
             * the exact opposite direction as dir. In such a case, forget the inertia idea,
             * and just use steepestDescent. */
            if (magGradF == 0) return steepestDescentReset(gradf);
            dir = (dir.divide(magGradF));
            dirLast = (double[])dir.Clone();
            return dir;
        }

        private double[] steepestDescentReset(double[] gradf)
        {
            magGradFLast = magGradF = gradf.norm2();
            if (magGradF == 0) return gradf;
            /* if the gradient of f is all zeros, then simply return it. */
            dir = StarMath.multiply((-1.0 / magGradF), gradf);
            dirLast = (double[])dir.Clone();
            return dir;
        }
    }
}