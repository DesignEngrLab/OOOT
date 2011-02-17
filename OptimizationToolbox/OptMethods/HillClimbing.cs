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
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    public class HillClimbing : abstractOptMethod
    {
        /// <summary>
        /// Gets the neighbor generator.
        /// </summary>
        /// <value>The neighbor generator.</value>
        public abstractGenerator neighborGenerator { get; private set; }
        /// <summary>
        /// Gets the selector.
        /// </summary>
        /// <value>The selector.</value>
        public abstractSelector selector { get; private set; }

        #region Constructor

        public HillClimbing()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = false;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
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
            var candidates = new List<Candidate>();
            candidates.Add(new Candidate(calc_f(x), x));
            while (notConverged(k++, numEvals, candidates[0].fValues[0], candidates[0].x))
            {
                SearchIO.output(k + ": f = " + candidates[0].fValues[0], 4);
                SearchIO.output("     x = " + StarMath.MakePrintString(candidates[0].x), 4);
                var neighbors = neighborGenerator.GenerateCandidates(candidates[0].x);
                candidates.AddRange(from neighbor in neighbors
                                    where ConstraintsSolvedWithPenalties || feasible(neighbor)
                                    let f = calc_f(neighbor)
                                    select new Candidate(f, neighbor));
                selector.selectCandidates(ref candidates);
            }
            xStar = candidates[0].x;
            return candidates[0].fValues[0];
        }
    }
}