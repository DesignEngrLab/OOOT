using System;
using System.Collections.Generic;


namespace OptimizationToolbox
{
    public abstract class abstractGenerator
    {

        public abstract SortedList<double, double[]> generateCandidates(SortedList<double, double[]> oldPop);
    }
}
