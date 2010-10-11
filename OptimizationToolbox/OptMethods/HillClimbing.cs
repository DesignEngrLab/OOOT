using System;
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    public class HillClimbing : abstractOptMethod
    {
        public abstractGenerator neighborGenerator { get; set; }
        public abstractSelector selector { get; set; }

        #region Constructor
        public HillClimbing()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = false;
            RequiresMeritFunction = false;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = true;
            RequiresDiscreteSpaceDescriptor = true;
        }
        #endregion

        public override void Add(object function)
        {
            if (typeof(abstractGenerator).IsInstanceOfType(function))
                neighborGenerator = (abstractGenerator)function;
            else if (typeof(abstractSelector).IsInstanceOfType(function))
                selector = (abstractSelector)function;
            else base.Add(function);
        }

        protected override double run(out double[] xStar)
        {
            var candidates = new List<KeyValuePair<double, double[]>>();
            candidates.Add(new KeyValuePair<double, double[]>(calc_f(x), x));
            while (notConverged(k++, candidates[0].Key, candidates[0].Value))
            {
                SearchIO.output(k + ": f = " + candidates[0].Key);
                SearchIO.output("     x = " + StarMath.MakePrintString(candidates[0].Value));
                var neighbors = neighborGenerator.GenerateCandidates(candidates[0].Value);
                foreach (var neighbor in neighbors)
                    if (feasible(neighbor))
                        candidates.Add(new KeyValuePair<double, double[]>(calc_f(neighbor), neighbor));
                selector.selectCandidates(ref candidates);
            }
            xStar = candidates[0].Value;
            return candidates[0].Key;
        }

    }
}

