using System;
using System.Collections.Generic;



namespace OptimizationToolbox
{
    //
    // this file needs to be updated. The original PNPPS in GraphSynth.Guidance
    // was improved, and differs from this in the retun type (candidate 
    // vs. array) - so I don't the two can be merged.
    public class PNormProportionalPopulationSelection : abstractSelector
    {
        Random rnd = new Random();
        int fIndex;
        int sortDirection = 0;
        Boolean alwaysKeepBest;
        Boolean dontAdjust = false;
        double lastQ = double.NaN;
        double lastP = double.NaN;
        const float maxP = 40f;
        const float minP = 0.025f;
        const float maxQ = 1.0f;
        const float minQ = 0.0f;
        /* after some empirical testing, I have settled on 40 and 1/40 as the highest and lowest
         * values for p. Since, on the generic level, the objective function may be any value, one
         * should be careful not to choose something too high as to go out of the range of a double (10e300). 
         * For example, for an f0=1million, a p greater than 51 will  cause an error. */
        double e;
        int numKeep;
        double newQP;

        #region Constructors
        /* Here are four different ways to initiate this method. The first argument in all cases is 
         * the index of the performance parameter that is being used for dictating a candidates worth. 
         * The second argument is  whether one is seeking to minimize that parameter (smaller is 
         * better) or maximize that parameter (bigger is better).
         * NOTE: only positive numbers should be used for the performance parameters.
         * The third argument specifies whether or not to adjust the p provided as an input to this class.
         * If adjustP is false, then the p provided is used directly (any values from 0 to infinity). The
         * behavior is difficult to predict, so we provide a way to adjust P from 0 to 1.
         * 0 means completely random.
         * 1 means completely deterministic.
         * and 0.5 means linearly preferred stochastic weighted. The 0.5 is adjusted to 1 - raising the 
         * performance parameters to 1 is linear.
         * The fourth argument is also a Boolean that states whether or not the best one is always kept
         * at the top of the returned set.
         * Additionally, one can specify a initialQorP which simplifies the arguments sent to the main prune 
         * function. initialQorP is still subject to adjustment, both by adjustP and if a new p argument is pro-
         * vided to prune.
         * The maxP of 25 and minP or 0.04 can also be changed in the constructor by specifying the
         * arguments minimumP and maximumP.
         * */
        public PNormProportionalPopulationSelection(int fIndex, optimize sortDirection, Boolean alwaysKeepBest)
            : this(fIndex, sortDirection, alwaysKeepBest, Double.NaN, false) { }
        public PNormProportionalPopulationSelection(int fIndex, optimize sortDirection, Boolean alwaysKeepBest,
            double initialQorP)
            : this(fIndex, sortDirection, alwaysKeepBest, initialQorP, false) { }
        public PNormProportionalPopulationSelection(int fIndex, optimize sortDirection, Boolean alwaysKeepBest,
            Boolean dontAdjust_PisGiven)
            : this(fIndex, sortDirection, alwaysKeepBest, Double.NaN, dontAdjust_PisGiven) { }

        public PNormProportionalPopulationSelection(int fIndex, optimize sortDirection, Boolean alwaysKeepBest,
            double initialQorP, Boolean dontAdjust_PisGiven) :base(sortDirection)
        {
            this.fIndex = fIndex;
            this.sortDirection = (int)sortDirection;
            this.alwaysKeepBest = alwaysKeepBest;
            this.dontAdjust = dontAdjust_PisGiven;
            if (dontAdjust) lastP = determineP(initialQorP);
            else
            {
                e = Math.Log((1 - minP) / (maxP - minP)) / -0.693147180559945;
                lastP = determineP(initialQorP);
                lastQ = initialQorP;
            }
            /* The idea behind this equation is to fit a line through three points. When the
             * Q is 0.5 then the adjusted value of p should be 1. When Q is 1, then the adjusted should
             * be maxP (40 by default), and finally when Q is less than 0.025, we adjust P down to zero. 
             * This is to regularize Q values so that at 0.5 one could fathom the linear probability approach
             * as is typically used in proportional (roulette-wheel) selection. */
        }


        #endregion

        /* The single function prune will simply reduce candidates, to the ones
         * used at the beginning of the next iteration. A simple overload exists
         * where the first argument is either a double or an int. If the double, 
         * fractionKeep, is used then we first find the integer equivalent. It must
         * at least one. The "Ceiling" function is used to round up to the near 
         * integer. The value of p determines how deterministic versus random the 
         * process is. If it is zero, then all candidates are equally preferred, 
         * if it is infinity only the best is kept.
         * In the end the function simply removes candidates from the list, and
         * nothing needs to be returned.         */
        #region Prune - overloads
        public SortedList<double, double[]> pruneCandidates(double fractionKeep, SortedList<double, double[]> candidates)
        { return pruneCandidates(fractionKeep, double.NaN, candidates); }

        public SortedList<double, double[]> pruneCandidates(double fractionKeep, double qp, SortedList<double, double[]> candidates)
        {
            if (fractionKeep == 0) this.numKeep = 1;
            else this.numKeep = (int)Math.Ceiling(fractionKeep * candidates.Count);
            this.newQP = qp;
            return selectCandidates(candidates);
        }
        public SortedList<double, double[]> pruneCandidates(int numKeep, SortedList<double, double[]> candidates)
        { return pruneCandidates(numKeep, double.NaN, candidates); }
        public SortedList<double, double[]> pruneCandidates(int numKeep, double qp, SortedList<double, double[]> candidates)
        {
            this.numKeep = numKeep;
            this.newQP = qp;
            return selectCandidates(candidates);
        }
        #endregion


        #region Prune - main function
        public override void selectCandidates(List<KeyValuePair<double, double[]>> candidates, double control = double.NaN)
        {
            double p = determineP(newQP);
            double[] bestCandidate = null;
            double bestCandidateKey = 0.0;
            SearchIO.output("", "", "guide", "PNPPS", "Guide PNPPS, p = " + p.ToString());
            if (numKeep > candidates.Count) numKeep = candidates.Count;
            if (alwaysKeepBest)
            {
                bestCandidate = candidates.Values[0];
                bestCandidateKey = candidates.Keys[0];
                candidates.RemoveAt(0);
                numKeep--;
            }
            if (Math.Abs(p) >= maxP)
            /* the process is DETERMINISTIC - 
             * we sort the list from best to worst and simply keep the top numKeep cand's.*/
            {
                for (int i = candidates.Count; i != numKeep; i--)
                    candidates.RemoveAt(i - 1);
            }
            else if ((-minP < p) && (p < minP))
            /* if p is zero or very close to it, then the process is completely RANDOM -
             * simply keep numKeep random candidates.*/
            {
                SearchIO.output("keeping any random " + numKeep.ToString(), 4);
                while (candidates.Count > numKeep)
                {
                    candidates.RemoveAt(rnd.Next(candidates.Count));
                }
                if (alwaysKeepBest)
                {
                    candidates.Add(bestCandidateKey, bestCandidate);
                    numKeep++;
                }
            }
            else
            /*  then minP <= p < maxP exclusive, and the STOCHASTIC roulette wheel
             * method is implemented. */
            {
                SortedList<double, double[]> keepers = new SortedList<double, double[]>(numKeep);
                double[] probabilities;

                /* the following loop is to add the winners/keepers to the list keepers
                 * after each addition, the candidate is removed from candidates and the process
                 * continues again without that candidate. */
                for (int i = 0; i != numKeep; i++)
                {
                    /* the two helper functions below complete this loop. The vector probabilities
                     * corresponds directly to the list of candidates. The assignProbabilities
                     * function calculates the values and normalizes them. */
                    probabilities = new double[candidates.Count];
                    assignProbabilities(p, probabilities, candidates);

                    /* the winner is stored in keeperIndex, it is found throught the findKeeper
                     * function, which accepts a random number between 0 and 1. */
                    int keeperIndex = findKeeper(probabilities, rnd.NextDouble());

                    /* now that candidate will live into the next iteration. We increment its age,
                     * remove from candidates and start over. */

                    keepers.Add(candidates.Keys[keeperIndex], candidates.Values[keeperIndex]);
                    candidates.RemoveAt(keeperIndex);
                }
                /* at the end we empty out the candidates bin and set it to the keepers. */
                candidates.Clear();
                candidates=keepers;
            }
            if (alwaysKeepBest)
            {
                candidates.Add(bestCandidateKey, bestCandidate);
                numKeep++;
            }
            return candidates;
        }

        #endregion

        #region assignProbabilities
        void assignProbabilities(double p, double[] probabilities, SortedList<double, double[]> candidates)
        {
            double total = 0.0;
            for (int i = 0; i != candidates.Count; i++)
            {
                probabilities[i] = Math.Pow(candidates.Keys[i], p);
                total += probabilities[i];
            }
            for (int i = 0; i != candidates.Count; i++)
                probabilities[i] /= total;
        }
        #endregion

        #region findKeeper
        int findKeeper(double[] probabilities, double z)
        {
            SearchIO.output("random = " + z.ToString(), 4);
            int i = probabilities.GetLength(0);
            while (z > 0)
            {
                i--;
                z -= probabilities[i];
            }
            SearchIO.output("candidate #" + i.ToString() + " chosen with prob = " + probabilities[i].ToString(), 4);
            return i;
        }
        #endregion

        #region Determine P
        private double determineP(double q)
        {
            /* first some conditions to check is q is not a number. */
            if (double.IsNaN(q) && double.IsNaN(lastP))
                throw new Exception("The value of qp was never specified. Consider setting as part of " +
                    "constructor with initialQorP.");
            if (double.IsNaN(q)) return lastP;

            /* here are some quick conditions when the q input is essentially the correct p output. Here the
             * function just flips over the value if it is not inline with the stated sortDirection. */
            if (dontAdjust)
            {
                if (((q > 0) && (sortDirection == -1)) || ((q < 0) && (sortDirection == 1)))
                    lastP = -q;
                else lastP = q;

                return lastP;
            }
            else
            {
                /* else, we must convert from q between 0 to 1 to p between -maxP to +maxP. */
                q = Math.Abs(q);
                if (q == lastQ) /*same values as last time, no need to recalculate. */ 
                    q = lastQ;

                else if (q <= minQ)
                {
                    lastQ = 0.0;
                    lastP = 0.0;
                }
                else if (q >= maxQ)
                {
                    lastQ = maxQ;
                    lastP = (double)sortDirection * maxP;
                }
                else
                {
                    lastQ = q;
                    lastP = (double)sortDirection * (minP + (maxP - minP) * Math.Pow(lastQ, e));
                }

                SearchIO.output("", "", "", "q=" + lastQ.ToString() + ",p=" + lastP.ToString(),
                    "q = " + lastQ.ToString() + " changed to p = " + lastP.ToString());
                return lastP;
            }
        }
        #endregion

    }
}
