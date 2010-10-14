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
using StarMathLib;


namespace OptimizationToolbox
{
    public class BFGSDirection : abstractSearchDirection
    {
        double[,] T;
        double[,] u ;
        double[,] IMinusT ;
        double[,] invH ;
        double[,] invHLast ;
        double[] xLast ;
        double[] gradFLast ;
        double[] dir ;
        double magDir;
        private double minimumAlpha;
        private int itersToReset;

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
                return steepestDescentReset(x, gradf, f);

            /* I could write a bunch of comments here on what these terms mean, but it would 
             * really require a lot of symoblic math. Basically, invH is the approximation of 
             * the inverse Hessian that starts out as the identity matrix. For the rest of
             * the terms consult an optimization textbook...like the one I'm writing ever so
             * slowly. */
            double[] diffX = StarMath.subtract(x, xLast);
            double[] diffGradF = StarMath.subtract(gradf, gradFLast);
            T = StarMath.multiplyVectorsIntoAMatrix(diffX, diffGradF);
            T = StarMath.multiply((1 / StarMath.multiplyDot(diffX, diffGradF)), T);

            u = StarMath.multiplyVectorsIntoAMatrix(diffX, diffX);
            u = StarMath.multiply((1 / StarMath.multiplyDot(diffX, diffGradF)), u);

            IMinusT = StarMath.subtract(StarMath.makeIdentity(T.GetLength(0)), T);

            invH = StarMath.add(StarMath.multiply(IMinusT,
                StarMath.multiply(invHLast, IMinusT)), u);
            dir = StarMath.multiply(invH, gradf);

            gradFLast = (double[])gradf.Clone();
            xLast = (double[])xLast.Clone();
            invHLast = (double[,])invH.Clone();
            magDir = StarMath.norm2(dir);
            if (magDir == 0) return gradf;
            /* if the gradient of f is all zeros, then simply return it. */
            else
            {
                dir = StarMath.multiply((-1.0 / magDir), gradf);
                return dir;
            }
        }


        private double[] steepestDescentReset(double[] x, double[] gradf, double f)
        {
            gradFLast = (double[])gradf.Clone();
            xLast = (double[])x.Clone();
            invHLast = StarMath.makeIdentity(x.GetLength(0));
            magDir = StarMath.norm2(gradf);
            if (magDir == 0) return gradf;
            /* if the gradient of f is all zeros, then simply return it. */
            else
            {
                dir = StarMath.multiply((-1.0 / magDir), gradf);
                return dir;
            }
        }
    }
}
