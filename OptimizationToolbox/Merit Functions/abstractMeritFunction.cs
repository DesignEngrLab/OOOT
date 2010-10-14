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

namespace OptimizationToolbox
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class abstractMeritFunction
    {
       protected abstractOptMethod optMethod;
        public double penaltyWeight {get; set; }
        public abstractMeritFunction(abstractOptMethod optMethod, double penaltyWeight)
        {
            this.optMethod = optMethod;
            this.penaltyWeight = penaltyWeight;
        }

        public abstract double[] calcGradientOfPenalty(double[] point);
        public abstract double calcPenalty(double[] point);
    }
}