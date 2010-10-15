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
using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class MetropolisCriteria : abstractSelector
    {
        private readonly Random rnd;

        public MetropolisCriteria(optimize direction)
            : base(direction)
        {
            rnd = new Random();
        }

        public override void selectCandidates(ref List<KeyValuePair<double, double[]>> candidates,
                                              double temperature = double.NaN)
        {
            var fOld = candidates[0].Key;
            var fNew = candidates[1].Key;
            if ((fNew == fOld) || (betterThan(fNew, fOld)))
                /* throw away the old and keep the new */
                candidates.RemoveAt(0);
            else
            {
                var probability = Math.Exp(((int)direction) * (fNew - fOld) / temperature);
                if (rnd.NextDouble() <= probability)
                    /* throw away the old and keep the new */
                    candidates.RemoveAt(0);
                /* otherwise stay with the old */
                else candidates.RemoveAt(1);
            }
        }
    }
}