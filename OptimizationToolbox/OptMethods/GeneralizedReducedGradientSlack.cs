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
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    public sealed class GeneralizedReducedGradientSlack : abstractOptMethod
    {
        private readonly double epsilon;
        private double alphaStar;
        private double[] dk;
        private double fk;
        private double[] gradF;

        private double[,] gradH, gradHWRT_xc, gradHWRT_xd;
        private double[,] invGradHWRT_xc;

        //inner loop counters and limits
        private int xcAttempts;
        private List<int> xcIndices;
        private List<int> xdIndices;

        private double[] xkLast;

        #region Constructor

        public GeneralizedReducedGradientSlack(double epsilon = 0.00001, int innerMax = 500, int outerMax = 5)
        {
            this.epsilon = epsilon;
            feasibleInnerLoopMax = innerMax;
            feasibleOuterLoopMax = outerMax;
            Add(new ArithmeticMean(epsilon, 1, 100));
            lineSearchMethod.SetOptimizationDetails(this);
            Add(new FletcherReevesDirection());

            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = false;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = true;
            RequiresLineSearchMethod = true;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = true;
            RequiresDiscreteSpaceDescriptor = false;
        }

        #endregion

        #region Xd and Xc properties

        public double[] xc
        {
            get
            {
                var vars = new double[xcIndices.Count];
                for (var i = 0; i < xcIndices.Count; i++)
                    vars[i] = x[xcIndices[i]];
                return vars;
            }
            set
            {
                if ((xcIndices != null) && (xcIndices.Count != 0) &&
                    (xcIndices.Count == value.GetLength(0)))
                    for (var i = 0; i != xcIndices.Count; i++)
                        x[xcIndices[i]] = value[i];
            }
        }

        // these properties will use the x*Indices
        public double[] xd
        {
            get
            {
                var vars = new double[xdIndices.Count];
                for (var i = 0; i < xdIndices.Count; i++)
                    vars[i] = x[xdIndices[i]];
                return vars;
            }
            set
            {
                if ((xdIndices != null) && (xdIndices.Count != 0) &&
                    (xdIndices.Count == value.GetLength(0)))
                    for (var i = 0; i != xdIndices.Count; i++)
                        x[xdIndices[i]] = value[i];
            }
        }

        #endregion

        #region Main Function, run

        protected override double run(out double[] xStar)
        {
            fStar = fk = calc_f(x);

            /* this is the iteration counter for updating Xc it's compared with feasibleOuterLoopMax. */
            foreach (IEquality c in h)
                active.Add(c);
            divideXintoDecisionAndDependentVars();

            do
            {
                gradH = calc_h_gradient(x);
                divideGradH_intoXcAndXdParts();
                invGradHWRT_xc = StarMath.inverse(gradHWRT_xc); //should this just be inverseUpper?
                gradF = calc_f_gradient(x);
                calculateReducedGradientSearchDirection();

                // use line search (arithmetic mean) to find alphaStar
                lineSearchMethod.lastFeasAlpha4G = 0.0;
                lineSearchMethod.findAlphaStar(x, dk);
                alphaStar = lineSearchMethod.lastFeasAlpha4G;
                //alphaStar = lineSearchMethod.findAlphaStar(xk, dk, g);

                xkLast = x;
                x = StarMath.add(xkLast, StarMath.multiply(alphaStar, dk));
                var outerFeasibleK = 0;
                while (!updateXc() && (++outerFeasibleK == feasibleOuterLoopMax))
                {
                    alphaStar /= 2;
                    x = StarMath.add(xkLast, StarMath.multiply(alphaStar, dk));
                }
                k++;
                fk = calc_f(x);
                SearchIO.output("X = " + x[0] + ", " + x[1], 3); // + ", " + xk[2]
                SearchIO.output("F(" + k + ") = " + fk, 3);
            } while (notConverged(k, numEvals, fk, x, null, gradF));
            fStar = fk;
            xStar = x;
            SearchIO.output("X* = " + x[0] + ", " + x[1], 2);
            SearchIO.output("F* = " + fk, 2);
            return fStar;
        }

        #endregion

        private Boolean divideXintoDecisionAndDependentVars()
        {
            // divide x into xc and xd using automated Guassian eliminiation approach
            var xcOldices = xcIndices;
            xcIndices = new List<int>(m);
            xdIndices = new List<int>(n - m);
            for (var i = 0; i < m; i++)
            {
                int maxCol;
                if (i < q) maxCol = n - i - 1;
                else
                {
                    var maxForRow = 0.0;
                    maxCol = -1;
                    for (var j = 0; j < n; j++)
                        if (Math.Abs(gradH[i, j]) > maxForRow)
                        {
                            maxForRow = Math.Abs(gradH[i, j]);
                            maxCol = j;
                        }
                }
                xcIndices.Add(maxCol);
                // if outerK equal m-1 continue!
                for (var ii = i + 1; ii < m; ii++)
                {
                    // first we find B coefficent to multiply all elements of row outerK with
                    var coeff = gradH[ii, maxCol] / gradH[i, maxCol];
                    for (var j = 0; j < n; j++)
                        gradH[ii, j] -= coeff * gradH[i, j];
                }
            }
            for (var i = 0; i < n; i++)
                if (!xcIndices.Contains(i))
                    xdIndices.Add(i);
            if ((xcOldices == null) || (xcOldices.Count != xcIndices.Count)) return true;
            return (xcOldices.Any(i => !xcIndices.Contains(i)));
        }

        private void divideGradH_intoXcAndXdParts()
        {
            gradHWRT_xc = new double[m, m];

            for (var i = 0; i < m; i++)
                for (var j = 0; j < m; j++)
                    gradHWRT_xc[j, i] = gradH[j, xcIndices[i]];

            gradHWRT_xd = new double[m, n - m];
            for (var i = 0; i < n - m; i++)
                for (var j = 0; j < m; j++)
                    gradHWRT_xd[j, i] = gradH[j, xdIndices[i]];
        }

        private void calculateReducedGradientSearchDirection()
        {
            var gradFXc = new double[m];
            var gradFXd = new double[n - m];
            for (var i = 0; i < n - m; i++)
                gradFXd[i] = gradF[xdIndices[i]];
            for (var i = 0; i < m; i++)
                gradFXc[i] = gradF[xcIndices[i]];
            //how to handle the problem when slack goes to zero/epsilon and the invgrad then goes to infinity
            var dir_Xd =
                StarMath.subtract(gradFXd,
                                  StarMath.multiply(gradFXc,
                                                    StarMath.multiply(invGradHWRT_xc, gradHWRT_xd)));
            var dir_Xc =
                StarMath.multiply(-1.0,
                                  StarMath.multiply(invGradHWRT_xc,
                                                    StarMath.multiply(gradHWRT_xd, dir_Xd)));
            for (var i = 0; i < n - m; i++)
                dk[xdIndices[i]] = dir_Xd[i];
            for (var i = 0; i < m; i++)
                dk[xcIndices[i]] = dir_Xc[i];

            dk = searchDirMethod.find(x, dk, fk);
        }

        private bool updateXc()
        {
            var innerFeasibleK = 0;
            var dir_Xd = new double[n - m];

            for (var i = 0; i < n - m; i++)
                dir_Xd[i] = dk[xdIndices[i]];

            var xcOld = xc;
            xc = StarMath.subtract(xcOld, StarMath.multiply(invGradHWRT_xc, calc_h_vector(x)));
            while (StarMath.norm1(xc, xcOld) / StarMath.norm1(xc) > epsilon)
            {
                gradHWRT_xc = calc_h_gradient(x, xcIndices);
                invGradHWRT_xc = StarMath.inverse(gradHWRT_xc);
                if (++innerFeasibleK == feasibleInnerLoopMax) return false;
                xcOld = xc;
                xc = StarMath.subtract(xcOld, StarMath.multiply(invGradHWRT_xc, calc_h_vector(x)));
                foreach (int i in xcIndices)
                    if ((i >= n - q) && (x[i] < 0))
                        /*then it's a slack variable that's gone negative and should simply be set to eps*/
                        x[i] = epsilon;
            }
            return true;
        }
    }
}