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
using StarMathLib;

namespace OptimizationToolbox
{
    public class CyclicCoordinates : abstractSearchDirection
    {
        private int counter;
        private double[] xLast;

        public override double[] find(double[] x, double[] gradf, double f, ref double initAlpha, Boolean reset = false)
        {
            if (counter == x.GetLength(0))
            {
                counter = 0;
                return x.subtract(xLast).normalize();
            }                                       
            if (counter == 0) xLast = (double[])x.Clone();   
            var d = new double[x.GetLength(0)];
            d[counter] = 1;
            counter++;
            return d;
        }
    }
}