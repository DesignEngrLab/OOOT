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
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class DSCPowell : abstractLineSearch
    {
        #region Constructors

        public DSCPowell(double epsilon, double stepSize, int kMax)
            : base(epsilon, stepSize, kMax)
        {
        }

        #endregion

        public override double findAlphaStar(double[] x, double[] dir)
        {
            var aFaP = new SortedList<double, double>(new optimizeSort(optimize.minimize))
                           {
                               {0.0, calcF(x, 0.0, dir)},
                               {stepSize, calcF(x, stepSize, dir)}
                           };
            var alphaNew = 0.0;

            k = 1; //so that this local k corresponds to # of f'n evals (starting at 0)

            #region set up first three points

            if (aFaP[stepSize] <= aFaP[0.0])
                aFaP.Add(3 * stepSize, calcF(x, 3 * stepSize, dir));
            else
                aFaP.Add(2 * stepSize / 3, calcF(x, 2 * stepSize / 3, dir));
            k++;

            #endregion

            #region start

            var fMin = Math.Min(aFaP.Values[0], Math.Min(aFaP.Values[1], aFaP.Values[2]));
            var minIndex = aFaP.IndexOfValue(fMin);
            do
            {
                k++;
                if (quadraticApprox(ref alphaNew, aFaP))
                {
                }
                else switch (minIndex)
                {
                    case 0:
                        alphaNew = aFaP.Keys[0] - (aFaP.Keys[2] - aFaP.Keys[0]);
                        break;
                    case 1:
                        alphaNew = (aFaP.Keys[0] + aFaP.Keys[1] + aFaP.Keys[2]) / 3;
                        break;
                    case 2:
                        alphaNew = aFaP.Keys[2] + (aFaP.Keys[2] - aFaP.Keys[0]);
                        break;
                }
                var fNew = calcF(x, alphaNew, dir);
                var fMax = Math.Max(fNew, Math.Max(aFaP.Values[0], Math.Max(aFaP.Values[1], aFaP.Values[2])));
                // find the largest of the four values
                //even though we don't know for sure whether alphaNew is in the bracket, we still will only
                // throw out either
                if ((aFaP.Values[0] == fMax) ||
                    ((fNew == fMax) && (alphaNew < aFaP.Keys[1])) ||
                    ((aFaP.Values[1] == fMax) && (aFaP.Keys[1] < alphaNew)))
                    //if lowest two alphas are max, then remove aFaP.Keys[0]
                    aFaP.RemoveAt(0);
                else //else highest two alpha create fMax, so remove alpahHigh
                    aFaP.RemoveAt(2);
                aFaP.Add(alphaNew, fNew);
                fMin = Math.Min(aFaP.Values[0], Math.Min(aFaP.Values[1], aFaP.Values[2]));
                minIndex = aFaP.IndexOfValue(fMin);
            } while (((Math.Abs(aFaP.Keys[2] - aFaP.Keys[0]) / 2) > epsilon) && (k++ < kMax));

            #endregion

            return aFaP.Keys[minIndex];
        }

        private static Boolean quadraticApprox(ref double alphaNew, SortedList<double, double> aFaP)
        {
            var a = aFaP.Keys[0];
            var fa = aFaP.Values[0];
            var b = aFaP.Keys[1];
            var fb = aFaP.Values[1];
            var c = aFaP.Keys[2];
            var fc = aFaP.Values[2];
            var term1 = (b - a) * fc + (c - b) * fa + (a - c) * fb;
            var term2 = (b * b - c * c) * fa + (c * c - a * a) * fb + (a * a - b * b) * fc;
            var term3 = (a - b) * (b - c) * (c - a);

            alphaNew = -0.5 * term2 / term1;
            return (2*term1/term3 > 0.0);
        }
    }
}