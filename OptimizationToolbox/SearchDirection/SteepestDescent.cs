using System;
using StarMathLib;


namespace OptimizationToolbox
{
    public class SteepestDescent : abstractSearchDirection
    {
        public override double[] find(double[] x, double[] gradf, double f)
        {
            /* calc the magnitude of the new gradient, magGradF. This is used several
             * times so in order to minimize time, calc it once and save it. */
            double magGradF = StarMath.norm2(gradf);
            if (magGradF == 0) return gradf;
            /* if the gradient of f is all zeros, then simply return it. */

            else return (StarMath.multiply((-1.0 / magGradF), gradf));
        }
    }
}
