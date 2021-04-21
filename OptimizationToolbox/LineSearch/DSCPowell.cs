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
using System.Collections.Generic;
using System.Linq;
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
            // include the first two points in this SortedList (essentially, the a, b and c).
            var alphaAndF = new SortedList<double, double>
                                {{0.0, calcF(x, 0.0, dir)}, 
                                {stepSize, calcF(x, stepSize, dir)}};
            // third point is based on if second point is better or worse
            if (alphaAndF[stepSize] <= alphaAndF[0.0])
                alphaAndF.Add(3 * stepSize, calcF(x, 3 * stepSize, dir));
            else
                alphaAndF.Add(stepSize / 2, calcF(x, stepSize / 2, dir));

            var k = 0;
            var fMin = alphaAndF.Values.Min();
            var minIndex = alphaAndF.IndexOfValue(fMin);

            #region start main loop
            while (((Math.Abs(alphaAndF.Keys[2] - alphaAndF.Keys[0]) / 2) > epsilon) && (k++ < kMax))
            {
                k++;
                double alphaNew;
                DSCPowellLoopStatus loopStatus;
                #region find new alpha
                if (!quadraticApprox(out alphaNew, alphaAndF))
                {
                    /* if the quadratic approximatoin fails, we make note of it with this enumerator. */
                    loopStatus = DSCPowellLoopStatus.SecondDerivNegative;
                    SearchIO.output("<<< uh-oh! negative 2nd deriv @ k=" + k + "!>>>", 5);
                }
                else if (alphaAndF.Keys.Contains(alphaNew))
                {
                    /* if the alpha is already one of the three, then no point in keeping it. */
                    loopStatus = DSCPowellLoopStatus.NewPointAlreadyFound;
                    SearchIO.output("<<< uh-oh! new point is a repeat @ k=" + k + "!>>>", 5);
                }
                else
                {
                    var fNew = calcF(x, alphaNew, dir);
                    if (fNew > alphaAndF.Values.Min() && NewPointIsFarthest(alphaNew, alphaAndF.Keys, minIndex))
                    {
                        SearchIO.output("<<< uh-oh! new point is not best and too far away @ k=" + k + "!>>>", 5);
                        /* the new point is not the best and is farthest from the current best, so it will simply
                         * be deleted after it is added. That's not good. */
                        loopStatus = DSCPowellLoopStatus.NewPointIsFarthest;
                    }
                    else
                    {
                        /* whew! it looks like the new point (point d) is a keeper */
                        loopStatus = DSCPowellLoopStatus.Normal;
                        alphaAndF.Add(alphaNew, fNew);
                    }
                }
                #endregion
                #region if alpha via quadratic approximation failed, then set here.
                if (loopStatus != DSCPowellLoopStatus.Normal)
                {
                    switch (minIndex)
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
                    alphaAndF.Add(alphaNew, calcF(x, alphaNew, dir));
                }
                #endregion
                #region remove farthest value from min
                fMin = alphaAndF.Values.Min();
                minIndex = alphaAndF.IndexOfValue(fMin);
                switch (minIndex)
                {
                    case 0:
                        alphaAndF.RemoveAt(3); break;
                    case 1:
                        if (alphaAndF.Keys[3] - alphaAndF.Keys[1] > alphaAndF.Keys[1] - alphaAndF.Keys[0])
                            alphaAndF.RemoveAt(3);
                        else alphaAndF.RemoveAt(0);
                        break;
                    case 2:
                        if (alphaAndF.Keys[3] - alphaAndF.Keys[2] > alphaAndF.Keys[2] - alphaAndF.Keys[0])
                            alphaAndF.RemoveAt(3);
                        else alphaAndF.RemoveAt(0);
                        break;
                    case 3:
                        alphaAndF.RemoveAt(0); break;
                }
                #endregion
                fMin = alphaAndF.Values.Min();
                minIndex = alphaAndF.IndexOfValue(fMin);
            }
            #endregion

            return alphaAndF.Keys[minIndex];
        }

        private static Boolean NewPointIsFarthest(double alphaStar, IList<double> alphas, int minIndex)
        {
            var delta = Math.Abs(alphaStar - alphas[minIndex]);
            switch (minIndex)
            {
                case 0: return (alphaStar > alphas[2] || delta > alphas[2] - alphas[0]);
                case 1: return (delta > alphas[2] - alphas[1] && delta > alphas[1] - alphas[0]);
                case 2: return (alphaStar < alphas[0] || delta > alphas[2] - alphas[0]);
                default:
                    throw new Exception("All cases are accounted for. You should see this.");
            }
        }

        static Boolean quadraticApprox(out double alphaNew, SortedList<double, double> aFaP)
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
    enum DSCPowellLoopStatus
    {
        Normal,
        SecondDerivNegative,
        NewPointIsFarthest,
        NewPointAlreadyFound
    }

}