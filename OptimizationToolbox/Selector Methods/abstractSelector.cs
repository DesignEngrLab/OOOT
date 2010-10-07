using System;
using System.Collections.Generic;
using System.Linq;


namespace OptimizationToolbox
{
    public abstract class abstractSelector
    {
        protected readonly optimize direction;
        protected readonly optimizeSort directionComparer;
        protected abstractSelector(optimize direction)
        {
            this.direction = direction;
            directionComparer = new optimizeSort(direction, true);
        }
        public abstract void selectCandidates(ref List<KeyValuePair<double, double[]>> candidates, double control = double.NaN);

        protected static List<int> makeRandomIntList(int size)
        {
            Random rnd = new Random();
            var result = new List<int>(size);
            for (int i = 0; i < size; i++)
                result.Insert(rnd.Next(result.Count), i);
            return result;
        }


        protected void sort(ref List<KeyValuePair<double, double[]>> candidates)
        {
            candidates = candidates.OrderBy(a => a.Key, new optimizeSort(direction, true)).ToList();
        }
        protected void randomizeList(ref List<KeyValuePair<double, double[]>> candidates)
        {
            var r = new Random();
            candidates = candidates.OrderBy(a => r.NextDouble()).ToList();
        }
        protected Boolean betterThan(double x, double y)
        {
            return (1 == directionComparer.Compare(x, y));
        }
    }
}
