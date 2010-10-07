using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
 public   class MetropolisCriteria : abstractSelector
    {
     public MetropolisCriteria(optimize direction) : base(direction)
     {
     }

     public override void selectCandidates(ref List<KeyValuePair<double, double[]>> candidates, double temperature = double.NaN)
     {
         var oldCandidate = candidates[0];
         var newCandidate = candidates[1];
          if ((newCandidate.Key==oldCandidate.Key)||(betterThan(newCandidate.Key, oldCandidate.Key)))
              candidates.Add(oldCandidate);
         //else 
         //{  

         //}
     }
    }
}
