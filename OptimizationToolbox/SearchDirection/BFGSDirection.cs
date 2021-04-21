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
    public class BFGSDirection : abstractSearchDirection
    {
        private readonly double minimumAlpha;
        private double[,] IMinusT;
        private double[,] T;
        private double[] dir;
        private double[] gradFLast;
        private double[,] invH;
        private double[,] invHLast;
        private int itersToReset;
        private double magDir;
        private double[,] u;
        private double[] xLast;

        public BFGSDirection(double minimumAlpha = 0.001, int itersToReset = -1)
        {
            this.minimumAlpha = minimumAlpha;
            this.itersToReset = itersToReset;
        }

        public override double[] find(double[] x, double[] gradf, double f, ref double initAlpha, bool reset = false)
        {
            /* if a TRUE is sent to reset, then we call a simple steepestDescent function. */
            if (reset || (itersToReset-- == 0) || ((Math.Abs(initAlpha) > 0) && (Math.Abs(initAlpha) <= minimumAlpha))
                || (invHLast == null) || (xLast == null) || (gradFLast == null))
                return steepestDescentReset(x, gradf);

            /* I could write a bunch of comments here on what these terms mean, but it would 
             * really require a lot of symoblic math. Basically, invH is the approximation of 
             * the inverse Hessian that starts out as the identity matrix. For the rest of
             * the terms consult an optimization textbook...like the one I'm writing ever so
             * slowly. */
            var diffX = x.subtract(xLast);
            var diffGradF = gradf.subtract(gradFLast);
            T = diffX.multiplyVectorsIntoAMatrix(diffGradF);
            T = StarMath.multiply((1 / diffX.dotProduct(diffGradF)), T);

            u = diffX.multiplyVectorsIntoAMatrix(diffX);
            u = StarMath.multiply((1 / diffX.dotProduct(diffGradF)), u);

            IMinusT = StarMath.makeIdentity(T.GetLength(0)).subtract(T);

            invH = IMinusT.multiply(invHLast.multiply(IMinusT)).add(u);
            dir = invH.multiply(gradf);

            gradFLast = (double[])gradf.Clone();
            xLast = (double[])xLast.Clone();
            invHLast = (double[,])invH.Clone();
            magDir = dir.norm2();
            if (magDir == 0) return gradf;
            /* if the gradient of f is all zeros, then simply return it. */
            dir = StarMath.multiply((-1.0 / magDir), gradf);
            return dir;
        }


        private double[] steepestDescentReset(double[] x, double[] gradf)
        {
            gradFLast = (double[])gradf.Clone();
            invHLast = StarMath.makeIdentity(x.GetLength(0));
            magDir = gradf.norm2();
            if (magDir == 0) return gradf;
            /* if the gradient of f is all zeros, then simply return it. */
            dir = StarMath.multiply((-1.0 / magDir), gradf);
            return dir;
        }
    }
}