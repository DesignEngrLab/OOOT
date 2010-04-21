using System;
using StarMathLib;


namespace OptimizationToolbox
{
    public class BFGSDirection : abstractSearchDirection
    {
        double[,] T = null;
        double[,] u = null;
        double[,] IMinusT = null;
        double[,] invH = null;
        double[,] invHLast = null;
        double[] xLast = null;
        double[] gradFLast = null;
        double[] dir = null;
        double magDir;

        public override double[] find(double[] x, double[] gradf, double f)
        {
            return find(x, gradf, f, false);
        }

        public override double[] find(double[] x, double[] gradf, double f, Boolean reset)
        {
            /* if a TRUE is sent to reset, then we call a simple steepestDescent function. */
            if ((reset) || (invHLast == null) || (xLast == null) || (gradFLast == null))
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
