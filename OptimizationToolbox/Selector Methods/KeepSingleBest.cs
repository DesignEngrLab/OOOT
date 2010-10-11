using System;
using System.Linq;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class KeepSingleBest : abstractSelector
    {
        public KeepSingleBest(optimize direction)
            : base(direction)
        {
        }

        public override void selectCandidates(ref List<KeyValuePair<double, double[]>> candidates, double control = double.NaN)
        {
            double bestF ;
            if (direction == optimize.maximize) bestF = candidates.Select(a =>a.Key).Max();
            else bestF = candidates.Select(a =>a.Key).Min();
            candidates.RemoveAll(a => a.Key != bestF);
            if (candidates.Count > 1) candidates = candidates.Take(1).ToList();
        }

    }
}
