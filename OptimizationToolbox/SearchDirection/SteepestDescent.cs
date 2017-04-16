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
using StarMathLib;

namespace OptimizationToolbox
{
    public class SteepestDescent : abstractSearchDirection
    {
        public override double[] find(double[] x, double[] gradf, double f, ref double initAlpha, bool reset = false)
        {
            /* calc the magnitude of the new gradient, magGradF. This is used several
             * times so in order to minimize time, calc it once and save it. */
            var magGradF = gradf.norm2();
            if (magGradF == 0) return gradf;
            /* if the gradient of f is all zeros, then simply return it. */

            return (StarMath.multiply((-1.0 / magGradF), gradf));
        }
    }
}