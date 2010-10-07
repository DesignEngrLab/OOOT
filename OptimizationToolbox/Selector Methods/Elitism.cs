using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class Elitism : abstractSelector
    {
        public Elitism(optimize direction) : base(direction)
        {
        }

        public override SortedList<double, double[]> selectCandidates(SortedList<double, double[]> candidates, double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int) (candidates.Count*fractionToKeep);
            while (candidates.Count > numKeep) candidates.RemoveAt(numKeep);
            return candidates;
        }
    }
}
