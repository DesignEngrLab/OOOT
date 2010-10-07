using System;
using System.Linq;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class Elitism : abstractSelector
    {
        public Elitism(optimize direction)
            : base(direction)
        {
        }

        public override void selectCandidates(ref List<KeyValuePair<double, double[]>> candidates, double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int)(candidates.Count * fractionToKeep);
            sort(ref candidates);
            candidates = candidates.Take(numKeep).ToList();
        }

    }
}
