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

namespace OptimizationToolbox
{
    /// <summary>
    /// Implicit Enumeration is a catch-all for Composite Decision Process (as
    /// deemed by Kanal and Kumar, 1988) for methods like Branch-and-Bound, 
    /// best first search, and A*.
    /// </summary>
    public class ImplicitEnueration : abstractOptMethod
    {
        private readonly optimizeSort comparer;
        private readonly optimize direction;
        private readonly DesignSpaceDescription spaceDescription;
        /// <summary>
        /// Gets the neighbor generator method class.
        /// </summary>
        /// <value>The neighbor generator.</value>
        public abstractGenerator neighborGenerator { get; private set; }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplicitEnueration"/> class.
        /// </summary>
        /// <param name="SpaceDescription">The space description.</param>
        /// <param name="direction">The direction.</param>
        public ImplicitEnueration(DesignSpaceDescription SpaceDescription, optimize direction)
        {
            spaceDescription = SpaceDescription;
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

        public override void Add(object function)
        {
            if (typeof(abstractGenerator).IsInstanceOfType(function))
                neighborGenerator = (abstractGenerator)function;
            else base.Add(function);
        }


        protected override double run(out double[] xStar)
        {
            var worstF = ((int)direction) * double.NegativeInfinity;
            var candidates = new SortedList<double, double[]>(comparer);
            if (xStart != null) x = (double[])xStart.Clone();
            else x = spaceDescription.GetVariableVector(new long[n]);
            candidates.Add(0.0, x);

            if (feasible(x)) candidates.Add(calc_f(x), x);

            while (notConverged(k++, numEvals, fStar, x) && candidates.Count != 0)
            {
                x = candidates.Values[0];
                fStar = candidates.Keys[0];
                candidates.RemoveAt(0);
                var children = neighborGenerator.GenerateCandidates(x);
                if (children.Count == 0)
                    break;
                foreach (var child in children)
                {
                    if (meritFunction != null || feasible(child))
                    {
                        var fValue = calc_f(child, (meritFunction != null));
                        candidates.Add(fValue, child);
                    }
                    else candidates.Add(worstF, x);
                }
            }
            xStar = x;
            return fStar;
        }
    }
}