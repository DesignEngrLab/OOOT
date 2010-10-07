using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
 public   class MetropolisCriteria : abstractSelector
    {
     public MetropolisCriteria(optimize direction) : base(direction)
     {
     }

     public override SortedList<double, double[]> selectCandidates(SortedList<double, double[]> sortedList, double control = double.NaN)
     {
         throw new NotImplementedException();
     }
    }
}
