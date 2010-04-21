using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class DSCPowell : abstractLineSearch
    {
        #region Constructors
        public DSCPowell(abstractOptMethod optMethod, double epsilon, double stepSize, int kMax)
            : base(optMethod, epsilon, stepSize,kMax) {  }
        #endregion
        

        public override double findAlphaStar(double[] x, double[] dir)
        {
            SortedList<double, double> aFaP = new SortedList<double, double>(new optimizeSort(optimize.minimize));
            aFaP.Add(0.0, calcF(x, 0.0, dir));
            aFaP.Add(stepSize, calcF(x, stepSize, dir));
            double alphaNew = 0.0;
            double fMin, fMax, fNew;
            int minIndex;

            k = 1;  //so that this local k corresponds to # of f'n evals (starting at 0)

            #region set up first three points
            if (aFaP[stepSize] <= aFaP[0.0])
                aFaP.Add(3 * stepSize, calcF(x, 3 * stepSize, dir));
            else
                aFaP.Add(2 * stepSize / 3, calcF(x, 2 * stepSize / 3, dir));
            k++;
            #endregion
            #region start
            fMin = Math.Min(aFaP.Values[0], Math.Min(aFaP.Values[1], aFaP.Values[2]));
            minIndex = aFaP.IndexOfValue(fMin);
            do
            {
                k++;
                if (quadraticApprox(ref alphaNew, aFaP)) { }
                else if (minIndex == 0)
                    alphaNew = aFaP.Keys[0] - (aFaP.Keys[2] - aFaP.Keys[0]);
                else if (minIndex == 1)
                    alphaNew = (aFaP.Keys[0] + aFaP.Keys[1] + aFaP.Keys[2]) / 3;
                else if (minIndex == 2)
                    alphaNew = aFaP.Keys[2] + (aFaP.Keys[2] - aFaP.Keys[0]);
                fNew = calcF(x, alphaNew, dir);
                fMax = Math.Max(fNew, Math.Max(aFaP.Values[0], Math.Max(aFaP.Values[1], aFaP.Values[2])));
                // find the largest of the four values
                //even though we don't know for sure whether alphaNew is in the bracket, we still will only
                // throw out either
                if ((aFaP.Values[0] == fMax) ||
                    ((fNew == fMax) && (alphaNew < aFaP.Keys[1])) ||
                    ((aFaP.Values[1] == fMax) && (aFaP.Keys[1] < alphaNew))) //if lowest two alphas are max, then remove aFaP.Keys[0]
                    aFaP.RemoveAt(0);
                else //else highest two alpha create fMax, so remove alpahHigh
                    aFaP.RemoveAt(2);
                aFaP.Add(alphaNew, fNew);
                fMin = Math.Min(aFaP.Values[0], Math.Min(aFaP.Values[1], aFaP.Values[2]));
                minIndex = aFaP.IndexOfValue(fMin);
            }
            while (((Math.Abs(aFaP.Keys[2] - aFaP.Keys[0]) / 2) > epsilon) && (k++ < kMax));
            #endregion
            
            return aFaP.Keys[minIndex];
        }

        private Boolean quadraticApprox(ref double alphaNew, SortedList<double, double> aFaP)
        {
            double a = aFaP.Keys[0];
            double fa = aFaP.Values[0];
            double b = aFaP.Keys[1];
            double fb = aFaP.Values[1];
            double c = aFaP.Keys[2];
            double fc = aFaP.Values[2];
            double term1 = (b - a) * fc + (c - b) * fa + (a - c) * fb;
            double term2 = (b * b - c * c) * fa + (c * c - a * a) * fb + (a * a - b * b) * fc;
            double term3 = (a - b) * (b - c) * (c - a);

            alphaNew = -0.5 * term2 / term1;
            if (2 * term1 / term3 <= 0.0) return false;
            else return true;

        }
    }
}
