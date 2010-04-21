using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class GoldenSection : abstractLineSearch
    {
        const double golden62 = 0.61803398874989484820458683436564;
        const double golden38 = 1 - golden62;

        #region Constructors
        public GoldenSection(abstractOptMethod optMethod, double epsilon, double stepSize, int kMax)
            : base(optMethod, epsilon, stepSize, kMax) { }
        #endregion

        public override double findAlphaStar(double[] x, double[] dir)
        {
            double alphaLow = 0.0;
            double alphaHigh = stepSize;
            double alpha1 = golden38 * alphaHigh;
            double alpha2 = golden62 * alphaHigh;

            double fLow = calcF(x, alphaLow, dir);
            double fHigh = calcF(x, alphaHigh, dir);
            double f2 = calcF(x, alpha2, dir);
            double f1 = double.NaN;

            #region Setting up Bracket
            /* not sure this has been published before, but borrowing the concept from DSC, what the
             * following while-loop is doing is to check if the three points are decreasing, if so
             * we define a new alphaHigh - in the GS-proportion so that the old alpahHigh can become
             * the new alpha2, the old alpha2 can become a new alpha1. In this way, can define a 
             * much lower initial bracket, so that it is similar to the other two line search methods. */
            while ((fHigh < f2) && (fHigh < fLow))
            {
                alpha1 = alpha2;
                f1 = f2;
                alpha2 = alphaHigh;
                f2 = fHigh;
                alphaHigh /= golden62;
                fHigh = calcF(x, alphaHigh, dir);
            }
            if (double.IsNaN(f1)) f1 = calcF(x, alpha1, dir);
            #endregion

            /* the number -2.078086921235 in the following formula is the 1/ln(golden62).
             * Since each new iteration (or function evaluations) will effectively reduce the
             * space by 62%, we can a priori determine the number of iterations that need to be
             * called. In this way, GS is the most robust, but perhaps not the most efficent. */
            kMax = (int)Math.Ceiling(-2.078086921235 * (Math.Log(epsilon, Math.E)-
                Math.Log(alphaHigh, Math.E)));

            for (k = 0; k <= kMax; k++)
            {
                if (f1 < f2)
                {
                    alphaHigh = alpha2;
                    fHigh = f2;
                    alpha2 = alpha1;
                    f2 = f1;
                    alpha1 = golden38 * (alphaHigh - alphaLow) + alphaLow;
                    f1 = calcF(x, alpha1, dir);
                    // reduce the upper bounds, alphaLow does not
                    // change in this case.
                }
                else
                {
                    alphaLow = alpha1;
                    fLow = f1;
                    alpha1 = alpha2;
                    f1 = f2;
                    alpha2 = golden62 * (alphaHigh - alphaLow) + alphaLow;
                    f2 = calcF(x, alpha2, dir);
                }
            }
            if (f1 < f2) return alpha1;
            else return alpha2;
        }
    }
}
