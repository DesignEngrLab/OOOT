using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class RandomPairwiseCompare : abstractSelector
    {
        private Random rnd;
        public RandomPairwiseCompare(optimize direction) : base( direction)
        {
            rnd = new Random();
        }
        public override SortedList<double, double[]> selectCandidates(SortedList<double, double[]> candidates, double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int)(candidates.Count * fractionToKeep);
            KeyValuePair<double, double[]> t = candidates[0];
            var randList = makeRandomIntList(candidates.Count);
            while (candidates.Count > numKeep)
            {
                if (betterThan(candidates.Keys[randList[0]], candidates.Keys[randList[1]]))
                {
                    var keepIndex = randList[0];
                    candidates.RemoveAt(randList[1]);
                    randList.RemoveAt(0);
                    randList.RemoveAt(1);
                    randList.Insert(randList.Count, keepIndex);
                }
                if (betterThan(candidates.Keys[randList[1]], candidates.Keys[randList[0]]))
                {
                    var keepIndex = randList[1];
                    candidates.RemoveAt(randList[0]);
                    randList.RemoveAt(0);
                    randList.RemoveAt(1);
                    randList.Insert(randList.Count, keepIndex);
                }
            }
            return candidates;
        }
    }
}
