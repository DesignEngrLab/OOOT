using System;
using StarMathLib;


namespace OptimizationToolbox
{
    public class FletcherReevesDirection : abstractSearchDirection
    {
        double[] dirLast = null; //last search direction
        double[] dir = null;
        double magGradF; //the magnitude of the current gradient of f
        double magGradFLast; //the magnitude of the last gradient of f

        public override double[] find(double[] x, double[] gradf, double f)
        {
            return find(x, gradf, f, false);
        }

        public override double[] find(double[] x, double[] gradf, double f, Boolean reset)
        {
            /* if a TRUE is sent to reset, then we call a simple steepestDescent function. */
            if (reset) return steepestDescentReset(x, gradf, f);

            /* calc the magnitude of the new gradient, magGradF. This is used several
             * times so in order to minimize time, calc it once and save it. */
            magGradF = StarMath.norm2(gradf);
            /* if it is all zeros then just return it. This is the only vector that can't be
             * normalized. */
            if (magGradF == 0) dir = gradf;
            else dir = (StarMath.multiply((-1.0 / magGradF), gradf));

            /* add the inertia from the last direction - if there was a last dir, dirLast */
            if ((magGradFLast != 0.0) && (dirLast != null))
                dir = StarMath.add(dir, StarMath.multiply((magGradF / magGradFLast), dirLast));
            
            /* we no longer need magGradFLast, so we write over it with the new value. */
            magGradFLast = magGradF;

            /* now dir is better, but no longer a unit vector. Readjust and save as the 
             * new last values. */
            magGradF = StarMath.norm2(dir);
            /* again, it's possible that it'll be all zeros. This happens when dirLast is in 
             * the exact opposite direction as dir. In such a case, forget the inertia idea,
             * and just use steepestDescent. */
            if (magGradF == 0) return steepestDescentReset(x, gradf, f);
            else
            {
                dir = (StarMath.multiply((1.0 / magGradF), dir));
                dirLast = (double[])dir.Clone();
                return dir;
            }
        }
        private double[] steepestDescentReset(double[] x, double[] gradf, double f)
        {
            magGradFLast = magGradF = StarMath.norm2(gradf);
            if (magGradF == 0) return gradf;
            /* if the gradient of f is all zeros, then simply return it. */
            else
            {
                dir = StarMath.multiply((-1.0 / magGradF), gradf);
                dirLast = (double[])dir.Clone();
                return dir;
            }
        }
    }
}
