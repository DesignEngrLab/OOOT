using System;
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    public class ExhaustiveSearch : abstractOptMethod
    {
        private readonly DesignSpaceDescription spaceDescription;
        private readonly optimize direction;
        private optimizeSort comparer;
        private long[] CurrentIndices;
        private const long timePreditionIndex = 1000;

        #region Constructor
        public ExhaustiveSearch(DesignSpaceDescription SpaceDescription, optimize direction)
        {
            spaceDescription = SpaceDescription;
            if (!SpaceDescription.AllDiscrete)
                throw new Exception("Exhaustive Search can only be used when Space is all discrete");
            this.direction = direction;
            comparer = new optimizeSort(direction);
            RequiresObjectiveFunction = false;
            ConstraintsSolvedWithPenalties = false;
            RequiresMeritFunction = false;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = false;
            RequiresConvergenceCriteria = false;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = true;
        }
        #endregion

        protected override double run(out double[] xStar)
        {
            var startTime = DateTime.Now;
            fStar = double.PositiveInfinity;
            CurrentIndices = new long[n];
            x = spaceDescription.GetVariableVector(CurrentIndices);
            xStar = x;
            if (feasible(x)) fStar = calc_f(x);
            while (notConverged(k++, fStar, xStar) && IncrementIndices())
            {
                if (k == timePreditionIndex) performTimePrediction(startTime);
                x = spaceDescription.GetVariableVector(CurrentIndices);
                var fNew = calc_f(x);
                if ((feasible(x)) && (!comparer.BetterThan(fStar, fNew)))
                {
                    fStar = fNew;
                    xStar = x;
                }
            }
            return fStar;
        }

        private void performTimePrediction(DateTime startTime)
        {
            double span = (DateTime.Now - startTime).Ticks;
            span /= timePreditionIndex;
            span *= spaceDescription.SizeOfSpace;
            DateTime endTime = startTime + new TimeSpan((long)span);
            SearchIO.output("Predicted time for the process to end:\n" + endTime);
        }



        private Boolean IncrementIndices(int IndicesIndex = 0)
        {
            if (IndicesIndex == n) return false;
            CurrentIndices[IndicesIndex]++;
            if (CurrentIndices[IndicesIndex] >= spaceDescription.MaxVariableSizes[IndicesIndex])
            {
                CurrentIndices[IndicesIndex] = 0;
                return IncrementIndices(IndicesIndex + 1);
            }
            else return true;
        }

    }
}

