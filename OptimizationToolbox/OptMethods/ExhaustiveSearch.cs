/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System;

namespace OptimizationToolbox
{
    public class ExhaustiveSearch : abstractOptMethod
    {
        private const long timePreditionIndex = 1000;

        #region Constructor

        public ExhaustiveSearch(DesignSpaceDescription SpaceDescription, optimize direction)
        {
            spaceDescription = SpaceDescription;
            if (!SpaceDescription.AllDiscrete)
                throw new Exception("Exhaustive Search can only be used when Space is all discrete");
            comparer = new optimizeSort(direction);
            RequiresObjectiveFunction = false;
            ConstraintsSolvedWithPenalties = false;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = false;
            RequiresConvergenceCriteria = false;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = true;
        }

        #endregion

        private readonly optimizeSort comparer;
        private readonly DesignSpaceDescription spaceDescription;
        private long[] CurrentIndices;

        protected override double run(out double[] xStar)
        {
            var startTime = DateTime.Now;
            fStar = double.PositiveInfinity;
            CurrentIndices = new long[n];
            x = spaceDescription.GetVariableVector(CurrentIndices);
            xStar = x;
            if (feasible(x)) fStar = calc_f(x);
            while (notConverged(k++, numEvals, fStar, xStar) && IncrementIndices())
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
            var endTime = startTime + new TimeSpan((long)span);
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
            return true;
        }
    }
}