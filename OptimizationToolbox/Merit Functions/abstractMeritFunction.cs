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
    /// </summary>
    public abstract class abstractMeritFunction
    {
        /// <summary>
        /// references back to the optimization method
        /// </summary>
        protected abstractOptMethod optMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="abstractMeritFunction"/> class.
        /// </summary>
        /// <param name="optMethod">The opt method.</param>
        /// <param name="penaltyWeight">The penalty weight.</param>
        protected abstractMeritFunction(abstractOptMethod optMethod, double penaltyWeight)
        {
            this.optMethod = optMethod;
            this.penaltyWeight = penaltyWeight;
        }

        /// <summary>
        /// Gets or sets the penalty weight.
        /// </summary>
        /// <value>The penalty weight.</value>
        public double penaltyWeight { get; set; }

        /// <summary>
        /// Calcs the gradient of penalty.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public abstract double[] calcGradientOfPenalty(double[] point);
        /// <summary>
        /// Calcs the penalty.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public abstract double calcPenalty(double[] point);
    }
}