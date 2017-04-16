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
using StarMathLib;

namespace OptimizationToolbox
{
    public sealed class GeneralizedReducedGradientActiveSet : abstractOptMethod
    {
        #region Fields

        /* the following lists indicate what elements of x are decision variable,
         * Xd and which are constrained or dependent variables, Xc. These are created
         * in the function "divideXintoDecisionAndDependentVars", and can be accessed
         * and changed by the properties: xc and xd. */
        private readonly double epsilon;
        private double alphaStar;
        private Boolean divideX;
        private double[] dk;

        /* fk is the value of f(xk). */
        private double fk;

        /* alphaStar is what is returned by the line search (1-D) search method. It is used
         * to update xk. */

        /* gradA is the gradient of the active constraints as an m by n matrix. It is then
         * divided into two subsets: the gradient with respect to (wrt) xc which would be a
         * square m-by-m matrix, and the grade w.r.t. xd which is m-by-n-m. The inverse of the
         * former is required to solve for xc. */
        private double[,] gradA, gradA_wrt_xc, gradA_wrt_xd;
        private double[] gradF;
        private double[,] invGradA_wrt_xc;
        private List<int> xcIndices;
        private List<int> xdIndices;

        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */
        private double[] xkLast;

        /* divideX is used to schedule when x should be divided into Xc and Xd. This needs to 
         * happen at the very beginning but also whenever the active set changes from the line
         * search. This is also used to inform the search direction that it should be reset. */

        #endregion

        #region Constructor

        public GeneralizedReducedGradientActiveSet(double epsilon = 0.00001, int innerMax = 500, int outerMax = 5)
        {
            this.epsilon = epsilon;
            feasibleInnerLoopMax = innerMax;
            feasibleOuterLoopMax = outerMax;
            Add(new ArithmeticMean(epsilon, 1, 100));
            lineSearchMethod.SetOptimizationDetails(this);
            Add(new FletcherReevesDirection());

            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = false;
            RequiresMeritFunction = true;
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

        /* these are completely dependent on the list of indices for Xc and Xd.
         * They do not change what variables are in each, but they can be used
         * to access (get) and change (set) the values of Xc and Xd. */

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
            //evaluate f(x0)
            fStar = fk = calc_f(x);
            dk = new double[n];

            /* this is the iteration counter for updating Xc it's compared with feasibleOuterLoopMax. */
            foreach (var c in h)
                active.Add(c);
            /* this forces formulateActiveSetAndGradients to do the division. */
            divideX = true;

            do
            {
                gradF = calc_f_gradient(x);
                formulateActiveSetAndGradients(x, divideX);
                calculateReducedGradientSearchDirection();

                /* whether or not the previous two lines of code reformulated Xc and Xd, we now
                 * set it to false so that it won't do so next iteration. However, there are two
                 * places in which this might change. First, if the update for Xc requires a lot of
                 * effort and the alphaStar needs to be reduced; and Second, in formulateActive... if
                 * there are new violated constraints or an old constraint is no longer infeasible. */
                divideX = false;

                /* use line search (e.g. arithmetic mean) to find alphaStar */
                alphaStar = lineSearchMethod.findAlphaStar(x, dk);

                xkLast = x;
                x = xkLast.add(StarMath.multiply(alphaStar, dk));
                int outerFeasibleK = 0;
                while (!updateXc() && (++outerFeasibleK <= feasibleOuterLoopMax))
                {
                    alphaStar /= 2;
                    x = xkLast.add(StarMath.multiply(alphaStar, dk));
                    divideX = true;
                }
                k++;
                fk = calc_f(x);

                SearchIO.output("x(" + k + ") = " + x.MakePrintString() + " = " + fk.ToString("0.00"), 4);
            } while (notConverged(k, numEvals, fk, x, null, gradF));

            fStar = fk;
            xStar = x;
            return fStar;
        }

        #endregion

        private void formulateActiveSetAndGradients(double[] xk, Boolean divideBool)
        {
            /* this list is ordered from largest to smallest. We only want to introduce ONE new member 
             * into the active set. Which would be the first element (j.e.value) of this sorted list. */
            var newInfeasibles
                = new SortedList<double, IConstraint>(new optimizeSort(optimize.maximize));

            divideX = divideBool;

            /* this foreach loop can remove any number of g's from the active set, and puts
             * any newly violated g's into the sorted list newInfeasbles. */
            foreach (IInequality c in g)
            {
                var gVal = calculate(c,xk);
                if (gVal < 0.0)
                {
                    if (active.Contains(c))
                    {
                        active.Remove(c);
                        divideX = true;
                    }
                }
                else newInfeasibles.Add(gVal, c);
            }
            /* only want to add one newInfeasible to the list of active. Hence the break statement. */
            foreach (var kvp in newInfeasibles)
                if (!active.Contains(kvp.Value))
                {
                    active.Add(kvp.Value);
                    divideX = true;
                    break;
                }
            if (divideX)
            {
                m = active.Count;
                gradA = calc_active_gradient(xk);
                divideX = divideXintoDecisionAndDependentVars();
                if (divideX)
                {
                    divideGradA_intoXcAndXdParts();
                    invGradA_wrt_xc = gradA_wrt_xc.inverse(); //should this just be inverseUpper?
                }
            }
        }

        private Boolean divideXintoDecisionAndDependentVars()
        {
            // divide x into xc and xd using automated Guassian eliminiation approach
            var differentFromBefore = false;
            var xcOldices = xcIndices;
            xcIndices = new List<int>(m);
            xdIndices = new List<int>(n - m);
            for (var i = 0; i < m; i++)
            {
                //if (j < q) maxCol = n - j - 1;
                //else
                //{
                double maxForRow = 0.0;
                int maxCol = -1;
                for (var j = 0; j < n; j++)
                    if (Math.Abs(gradA[i, j]) > maxForRow)
                    {
                        maxForRow = Math.Abs(gradA[i, j]);
                        maxCol = j;
                    }
                //}
                xcIndices.Add(maxCol);
                // if outerK equal m-1 continue!
                for (var ii = i + 1; ii < m; ii++)
                {
                    // first we find B coefficent to multiply all elements of row outerK with
                    double coeff = gradA[ii, maxCol] / gradA[i, maxCol];
                    for (var j = 0; j < n; j++)
                        gradA[ii, j] -= coeff * gradA[i, j];
                }
            }
            for (var i = 0; i < n; i++)
                if (!xcIndices.Contains(i))
                    xdIndices.Add(i);
            if ((xcOldices == null) || (xcOldices.Count != xcIndices.Count)) differentFromBefore = true;
            else
                foreach (int i in xcOldices)
                    if (!xcIndices.Contains(i)) differentFromBefore = true;
            return differentFromBefore;
        }

        private void divideGradA_intoXcAndXdParts()
        {
            gradA_wrt_xc = new double[m, m];

            for (var i = 0; i < m; i++)
                for (var j = 0; j < m; j++)
                    gradA_wrt_xc[j, i] = gradA[j, xcIndices[i]];

            gradA_wrt_xd = new double[m, n - m];
            for (var i = 0; i < n - m; i++)
                for (var j = 0; j < m; j++)
                    gradA_wrt_xd[j, i] = gradA[j, xdIndices[i]];
        }

        private void 
            calculateReducedGradientSearchDirection()
        {
            var gradFXc = new double[m];
            var gradFXd = new double[n - m];
            for (var i = 0; i < n - m; i++)
                gradFXd[i] = gradF[xdIndices[i]];
            for (var i = 0; i < m; i++)
                gradFXc[i] = gradF[xcIndices[i]];
            //how to handle the problem when slack goes to zero/epsilon and the invgrad then goes to infinity
            var dir_Xd =
                gradFXd.subtract(gradFXc.multiply(invGradA_wrt_xc.multiply(gradA_wrt_xd)));
            var dir_Xc =
                StarMath.multiply(-1.0,
                                  invGradA_wrt_xc.multiply(gradA_wrt_xd.multiply(dir_Xd)));
            for (var i = 0; i < n - m; i++)
                dk[xdIndices[i]] = dir_Xd[i];
            for (var i = 0; i < m; i++)
                dk[xcIndices[i]] = dir_Xc[i];

            dk = searchDirMethod.find(x, dk, fk, divideX);
        }

        private bool updateXc()
        {
            var innerFeasibleK = 0;
            var dir_Xd = new double[n - m];

            for (var i = 0; i < n - m; i++)
                dir_Xd[i] = dk[xdIndices[i]];

            double[] xcOld = xc;
            xc = xcOld.subtract(invGradA_wrt_xc.multiply(calc_active_vector(x)));
            while (xc.norm1(xcOld) / xc.norm1() > epsilon)
            {
                gradA_wrt_xc = calc_active_gradient(x, xcIndices);
                invGradA_wrt_xc = gradA_wrt_xc.inverse();
                if (++innerFeasibleK == feasibleInnerLoopMax) return false;
                xcOld = xc;
                xc = xcOld.subtract(invGradA_wrt_xc.multiply(calc_active_vector(x)));
            }
            return true;
        }
    }
}