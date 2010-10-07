using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class ArithmeticMean : abstractLineSearch
    {
        #region Constructors
        public ArithmeticMean(double epsilon, double stepSize, int kMax)
            : base(epsilon, stepSize, kMax) { }
        #endregion



        public override double findAlphaStar(double[] x, double[] dir)
        {
            double alphaLow = 0.0;
            double fLow = calcF(x, alphaLow, dir);
            double alphaHigh = stepSize;
            double fHigh = calcF(x, alphaHigh, dir);
            double alphaMid, alphaNew, fMid, fNew, fMax;
            double step = stepSize;

            k = 1;  //so that this local k corresponds to # of f'n evals (starting at 0)
            #region Setting up Brackets Phase
            if (fHigh <= fLow)
            {
                // these two lines are just to set up alphaMid and fMid so that the while loop will correctly
                alphaMid = alphaLow;
                fMid = fLow;
                do
                {
                    alphaLow = alphaMid;
                    fLow = fMid;
                    alphaMid = alphaHigh;
                    fMid = fHigh;
                    step *= 2;
                    alphaHigh = alphaMid + step;
                    fHigh = calcF(x, alphaHigh, dir);
                    k++;
                } while ((fHigh < fMid) && (k < kMax));
            }
            else
            {
                // these two lines are just to set up alphaMid and fMid so that the while loop will correctly
                alphaMid = alphaHigh;
                fMid = fHigh;
                do
                {
                    alphaHigh = alphaMid;
                    fHigh = fMid;
                    /* this may be different from the published DSC approach. In that method, 
                     * the mid point is in the middle of low and high. The problem is that when
                     * the arithmetic mean is taken, you arrive at the same point. So, borrowing
                     * from the four-point bracketing in golden-section and the 5/9 method, the 
                     * mid is placed at 2/3 the distance. In this the 3 points are positioned 
                     * similar to how they are in the expanding case. */
                    alphaMid = 2 * (alphaHigh - alphaLow) / 3;
                    fMid = calcF(x, alphaMid, dir);
                    k++;
                } while ((fLow < fMid) && (k < kMax));
            }
            #endregion
            #region Arithmetic Mean to reduce bracket
            alphaNew = (alphaLow + alphaMid + alphaHigh) / 3;
            fNew = calcF(x, alphaNew, dir);
            k++;
            do
            {
                fMax = Math.Max(fLow, Math.Max(fNew, Math.Max(fMid, fHigh)));
                // find the largest of the four values
                if ((fLow == fMax) ||
                    ((fNew == fMax) && (alphaNew < alphaMid)) ||
                    ((fMid == fMax) && (alphaMid < alphaNew))) //if lowest two alphas are max, then remove alphaLow
                {
                    // then remove alphaLow 
                    if (alphaNew < alphaMid)
                    { //alphaLow moves up to alphaNew, alphaMid, alphaHigh stay the same
                        alphaLow = alphaNew;
                        fLow = fNew;
                    }
                    else
                    { //alphaLow moves up to alphaMid,alphaNew changes to alphaMid, alphaHigh stays
                        alphaLow = alphaMid;
                        fLow = fMid;
                        alphaMid = alphaNew;
                        fMid = fNew;
                    }
                }
                else //else highest two alpha create fMax, so remove alpahHigh
                {
                    if (alphaNew > alphaMid)
                    { //alphaHigh moves down to alphaNew, alphaMid, alphaLow stay the same
                        alphaHigh = alphaNew;
                        fHigh = fNew;
                    }
                    else
                    { //alphaHigh moves down to alphaMid,alphaNew changes to alphaMid, alphaLow stays
                        alphaHigh = alphaMid;
                        fHigh = fMid;
                        alphaMid = alphaNew;
                        fMid = fNew;
                    }
                }
                alphaNew = (alphaLow + alphaMid + alphaHigh) / 3;
                fNew = calcF(x, alphaNew, dir);
                k++;
            }
            while ((Math.Abs(alphaNew - alphaMid) > epsilon) && (k < kMax));
            #endregion
           if (fMid < fNew) return  alphaMid;
            else return alphaNew;
        }
    }
}
