using System;
using System.Collections.Generic;


namespace OptimizationToolbox
{
    public abstract class abstractPrune
    {

        public abstract SortedList<double, double[]> pruneCandidates(SortedList<double, double[]> oldPop);
    }
}
