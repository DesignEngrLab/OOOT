// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="P-norm Proportional Selection.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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

namespace OptimizationToolbox
{
    /// <summary>
    /// Class PNormProportionalSelection.
    /// Implements the <see cref="OptimizationToolbox.abstractSelector" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractSelector" />
    public class PNormProportionalSelection : abstractSelector
    {
        /// <summary>
        /// The always keep best
        /// </summary>
        private readonly Boolean alwaysKeepBest;
        /// <summary>
        /// The random
        /// </summary>
        private readonly Random rnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="PNormProportionalSelection"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="AlwaysKeepBest">The always keep best.</param>
        /// <param name="Q">The q.</param>
        public PNormProportionalSelection(optimize direction, Boolean AlwaysKeepBest, double Q = 0.5)
            : base(new[] { direction })
        {
            rnd = new Random();
            alwaysKeepBest = AlwaysKeepBest;
            this.Q = Q;
        }

        /// <summary>
        /// Selects the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="fractionToKeep">The fraction to keep.</param>
        public override void SelectCandidates(ref List<ICandidate> candidates, double fractionToKeep = double.NaN)
        {
            var survivors = new List<ICandidate>();
            if (alwaysKeepBest)
            {
                double bestF = optDirections[0] == optimize.maximize
                    ? candidates.Select(a => a.objectives[0]).Max()
                    : candidates.Select(a => a.objectives[0]).Min();
                var best = candidates.FirstOrDefault(a => a.objectives[0] == bestF);
                survivors.Add(best);
                candidates.Remove(best);
            }
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int)(candidates.Count * fractionToKeep);

            while (survivors.Count < numKeep)
            {
                var probabilities = makeProbabilites(candidates);
                var z = findIndex(rnd.NextDouble(), probabilities);
                survivors.Add(candidates[z]);
                candidates.RemoveAt(z);
            }
            candidates = survivors;
        }

        /// <summary>
        /// Finds the index.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="probabilities">The probabilities.</param>
        /// <returns>System.Int32.</returns>
        private static int findIndex(double p, double[] probabilities)
        {
            for (int index = 0; index < probabilities.GetLength(0); index++)
                if (probabilities[index]>p)
                    return index;
            return -1;
        }

        /// <summary>
        /// Makes the probabilites.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <returns>System.Double[].</returns>
        private double[] makeProbabilites(IList<ICandidate> candidates)
        {
            var length = candidates.Count;
            var result = new double[length];
            for (var i = 0; i < length; i++)
                result[i] = Math.Pow(candidates[i].objectives[0], p);
            var sum = result.Sum();
            result[0] /= sum;
            for (var i = 1; i < length; i++)
                result[i] = result[i - 1] + (result[i] / sum);
            return result;
        }

        #region Converting Between Q and P

        /// <summary>
        /// The maximum p
        /// </summary>
        private const double maxP = 40;
        /// <summary>
        /// The minimum p
        /// </summary>
        private const double minP = 0.025;
        /// <summary>
        /// The maximum q
        /// </summary>
        private const double maxQ = 1.0;
        /// <summary>
        /// The minimum q
        /// </summary>
        private const double minQ = 0.0;
        /* after some empirical testing, I have settled on 40 and 1/40 as the highest and lowest
         * values for p. Since, on the generic level, the objective function may be any value, one
         * should be careful not to choose something too high as to go out of the range of a double (10e300). 
         * For example, for an f0=1million, a p greater than 51 will  cause an error. */

        /// <summary>
        /// v is the power that q is raised to convert it to p.
        /// </summary>
        private readonly double v = Math.Log((1 - minP) / (maxP - minP)) / -0.693147180559945;

        /// <summary>
        /// The p
        /// </summary>
        private double p;

        /// <summary>
        /// The q
        /// </summary>
        private double q;

        /// <summary>
        /// Gets or sets the p.
        /// </summary>
        /// <value>The p.</value>
        public double P
        {
            get { return p; }
            set
            {
                if (Math.Abs(value) < minP) p = (double)optDirections[0] * minP;
                else if (Math.Abs(value) > maxP) p = (double)optDirections[0] * maxP;
                else p = (double)optDirections[0] * Math.Abs(value);
                determineQfromP();
            }
        }

        /// <summary>
        /// Gets or sets the q.
        /// </summary>
        /// <value>The q.</value>
        public double Q
        {
            get { return q; }
            set
            {
                if (value < minQ) q = minQ;
                else if (value > maxQ) q = maxQ;
                else q = value;
                determinePfromQ();
            }
        }

        /// <summary>
        /// Determines the pfrom q.
        /// </summary>
        private void determinePfromQ()
        {
            if (q <= minQ)
                p = 0.0;
            else if (q >= maxQ)
                p = (double)optDirections[0] * maxP;
            else
                p = (double)optDirections[0] * (minP + (maxP - minP) * Math.Pow(q, v));


            SearchIO.output("", "", "", "q=" + q + ",p=" + p,
                            "q = " + q + " changed to p = " + p);
        }

        /// <summary>
        /// Determines the qfrom p.
        /// </summary>
        private void determineQfromP()
        {
            if (Math.Abs(p) <= minP)
                q = minQ;
            else if (Math.Abs(p) >= maxP)
                q = maxQ;
            else
                q = Math.Pow((Math.Abs(p) - minP) / (maxP - minP), 1.0 / v);


            SearchIO.output("", "", "", "p=" + p + ",q=" + q,
                            "p = " + p + " changed to q = " + q);
        }

        #endregion
    }
}