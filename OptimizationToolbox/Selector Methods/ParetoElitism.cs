using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skewboid;

namespace OptimizationToolbox.Selector_Methods
{
    /// <summary>
    /// 
    /// </summary>
    public class ParetoElitism : abstractSelector
    {
        public ParetoElitism(params optimize[] optimizationDirections)
            : base(optimizationDirections)
        {
        }

        public override void SelectCandidates(ref List<ICandidate> candidates, double control = double.NaN)
        {                                                     
            candidates = ParetoFunctions.FindParetoCandidates(candidates, optDirections);
        }
    }

}
