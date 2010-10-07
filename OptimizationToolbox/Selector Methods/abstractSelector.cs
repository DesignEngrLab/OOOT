using System;
using System.Collections.Generic;
using System.Linq;


namespace OptimizationToolbox
{
    public abstract class abstractSelector
    {
        private readonly optimize direction;
        protected abstractSelector(optimize direction)
        {
            this.direction = direction;
        }
        public abstract SortedList<double, double[]> selectCandidates(SortedList<double, double[]> candidates, double control = double.NaN);

        protected static List<int> makeRandomIntList(int size)
        {
            Random rnd = new Random();
            var result = new List<int>(size);
            for (int i = 0; i < size; i++)
                result.Insert(rnd.Next(result.Count), i);
            return result;
        }

        protected Boolean betterThan(double x, double y)
        {
            if (x < y) return (direction == optimize.minimize);
            if (x > y) return (direction == optimize.maximize);
            return false;
        }
    }
}
