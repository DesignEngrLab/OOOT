using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class RandomPairwiseCompare : abstractSelector
    {
        private readonly Random rnd;
        public RandomPairwiseCompare(optimize direction)
            : base(direction)
        {
            rnd = new Random();
        }
        public override void selectCandidates(ref List<KeyValuePair<double, double[]>> candidates, double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int)(candidates.Count * fractionToKeep);
            randomizeList(ref candidates);
            while (candidates.Count > numKeep)
            {
                var contestantA = candidates[0];
                candidates.RemoveAt(0);
                var contestantB = candidates[0];
                candidates.RemoveAt(0);
                if (betterThan(contestantA.Key, contestantB.Key))
                    candidates.Add(contestantA);
                else if (betterThan(contestantB.Key, contestantA.Key))
                    candidates.Add(contestantB);
                else
                {   /* if it's a tie, add them both to the list, but in off-chance that this match is played
                     * again, contestantB is added at a random location in the list. */
                    candidates.Add(contestantA);
                    candidates.Insert(rnd.Next(candidates.Count), contestantB);
                }
            }
        }

    }
}
