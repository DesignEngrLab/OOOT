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
    public class SkewboidDiversity : abstractSelector
    {                                  
        public SkewboidDiversity(params optimize[] optimizationDirections) : base(optimizationDirections)
        {
        }

        public override void SelectCandidates(ref List<ICandidate> candidates, double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int) (candidates.Count()*fractionToKeep);
            double alphaTarget;
            candidates = ParetoFunctions.FindGivenNumCandidates(candidates, numKeep, out alphaTarget, null,
                optDirections);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SkewboidWeighted : abstractSelector
    {
        public SkewboidWeighted(double[] weights, params optimize[] optimizationDirections)
            : base(optimizationDirections)
        {
            if (weights.GetLength(0) != optimizationDirections.GetLength(0))
                throw new Exception("Received an unequal array of weights and optimization directions in Weighted Skewboid.");
            this.weights = (double[])weights.Clone();
        }

        public double[] weights { get; set; }

        public override void SelectCandidates(ref List<ICandidate> candidates, double fractionToKeep = double.NaN)
        {
            if (double.IsNaN(fractionToKeep)) fractionToKeep = 0.5;
            var numKeep = (int)(candidates.Count() * fractionToKeep);
            double alphaTarget;
            candidates = ParetoFunctions.FindGivenNumCandidates(candidates, numKeep, out alphaTarget, weights,
                optDirections);
        }
    }

}
