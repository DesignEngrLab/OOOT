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
using StarMathLib;

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
            var alphaAndF = new SortedList<double, double>
                                {{0.0, calcF(x, 0.0, dir)}, 
                                {stepSize, calcF(x, stepSize, dir)}};

            k = 1; //so that this local k corresponds to # of f'n evals (starting at 0)

            #region set up first three points
            if (alphaAndF[stepSize] <= alphaAndF[0.0])
                alphaAndF.Add(3 * stepSize, calcF(x, 3 * stepSize, dir));
            else
                alphaAndF.Add(2 * stepSize / 3, calcF(x, 2 * stepSize / 3, dir));
            k++;
            #endregion

            #region start

            var fMin = StarMath.Min(alphaAndF.Values);
            var minIndex = alphaAndF.IndexOfValue(fMin);
            do
            {
                k++;
                double alphaNew;
                if (quadraticApprox(out alphaNew, alphaAndF))
                {
                }
                else switch (minIndex)
                    {
                        case 0:
                            alphaNew = alphaAndF.Keys[0] + (alphaAndF.Keys[0] - alphaAndF.Keys[2]);
                            break;
                        case 1:
                            alphaNew = (alphaAndF.Keys[0] + alphaAndF.Keys[1] + alphaAndF.Keys[2]) / 3;
                            break;
                        case 2:
                            alphaNew = alphaAndF.Keys[2] + (alphaAndF.Keys[2] - alphaAndF.Keys[0]);
                            break;
                    }
                if (alphaAndF.Keys[1] == alphaNew)
                {
                    if (alphaAndF.Values[2] > alphaAndF.Values[0]) alphaNew = (alphaAndF.Keys[1] + alphaAndF.Keys[2]) / 2;
                    else alphaNew = (alphaAndF.Keys[1] + alphaAndF.Keys[0]) / 2;
                }
                var fNew = calcF(x, alphaNew, dir);
                var fMax = Math.Max(fNew, StarMath.Max(alphaAndF.Values));
                // find the largest of the four values
                //even though we don't know for sure whether alphaNew is in the bracket, we still will only
                // throw out either the current left or right of the bracket.
                if ((alphaAndF.Values[0] == fMax) ||
                    ((fNew == fMax) && (alphaNew < alphaAndF.Keys[1])) ||
                    ((alphaAndF.Values[1] == fMax) && (alphaAndF.Keys[1] < alphaNew)))
                    //if lowest two alphas are max, then remove aFaP.Keys[0]
                    alphaAndF.RemoveAt(0);
                else //else highest two alpha create fMax, so remove alpahHigh
                    alphaAndF.RemoveAt(2);
                alphaAndF.Add(alphaNew, fNew);
                fMin = Math.Min(alphaAndF.Values[0], Math.Min(alphaAndF.Values[1], alphaAndF.Values[2]));
                minIndex = alphaAndF.IndexOfValue(fMin);
            } while (((Math.Abs(alphaAndF.Keys[2] - alphaAndF.Keys[0]) / 2) > epsilon) && (k++ < kMax));

            #endregion

            return alphaAndF.Keys[minIndex];
        }

        private static Boolean quadraticApprox(out double alphaNew, SortedList<double, double> aFaP)
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
            return (2 * term1 / term3 > 0.0);
        }
    }
}