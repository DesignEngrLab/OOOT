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

namespace OptimizationToolbox
{
    public class PNormProportionalSelection : abstractSelector
    {
        private readonly Boolean alwaysKeepBest;
        private readonly Random rnd;

        public PNormProportionalSelection(optimize direction, Boolean AlwaysKeepBest, double Q = 0.5)
            : base(direction)
        {
            rnd = new Random();
            alwaysKeepBest = AlwaysKeepBest;
            this.Q = Q;
        }

        public override void selectCandidates(ref List<KeyValuePair<double, double[]>> candidates,
                                              double fractionToKeep = double.NaN)
        {
            var survivors = new List<KeyValuePair<double, double[]>>();
            if (alwaysKeepBest)
            {
                double bestF;
                if (direction == optimize.maximize) bestF = candidates.Select(a => a.Key).Max();
                else bestF = candidates.Select(a => a.Key).Min();
                survivors.Add(candidates.Find(a => a.Key != bestF));
                candidates.Remove(survivors[0]);
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

        private static int findIndex(double p, IList<double> probabilities)
        {
            var i = 0;
            while (p > 0) p -= probabilities[i++];
            return i;
        }

        private double[] makeProbabilites(IList<KeyValuePair<double, double[]>> candidates)
        {
            var length = candidates.Count;
            var result = new double[length];
            for (var i = 0; i < length; i++)
                result[i] = Math.Pow(candidates[i].Key, p);
            var sum = result.Sum();
            for (var i = 0; i < length; i++)
                result[i] /= sum;
            return result;
        }

        #region Converting Between Q and P

        private const double maxP = 40;
        private const double minP = 0.025;
        private const double maxQ = 1.0;
        private const double minQ = 0.0;
        /* after some empirical testing, I have settled on 40 and 1/40 as the highest and lowest
         * values for p. Since, on the generic level, the objective function may be any value, one
         * should be careful not to choose something too high as to go out of the range of a double (10e300). 
         * For example, for an f0=1million, a p greater than 51 will  cause an error. */

        /// <summary>
        ///   v is the power that q is raised to convert it to p.
        /// </summary>
        private readonly double v = Math.Log((1 - minP) / (maxP - minP)) / -0.693147180559945;

        private double p;

        private double q;

        public double P
        {
            get { return p; }
            set
            {
                if (Math.Abs(value) < minP) p = (double)direction * minP;
                else if (Math.Abs(value) > maxP) p = (double)direction * maxP;
                else p = (double)direction * Math.Abs(value);
                determineQfromP();
            }
        }

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

        private void determinePfromQ()
        {
            if (q <= minQ)
                p = 0.0;
            else if (q >= maxQ)
                p = (double)direction * maxP;
            else
                p = (double)direction * (minP + (maxP - minP) * Math.Pow(q, v));


            SearchIO.output("", "", "", "q=" + q + ",p=" + p,
                            "q = " + q + " changed to p = " + p);
        }

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