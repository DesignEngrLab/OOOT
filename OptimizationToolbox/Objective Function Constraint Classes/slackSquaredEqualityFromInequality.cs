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
    internal class slackSquaredEqualityFromInequality : IEquality
    {
        private readonly IInequality formerIneq;
        private readonly int slackIndex;
        private readonly abstractOptMethod optMethod;

        public slackSquaredEqualityFromInequality(IInequality formerIneq, int slackIndex, abstractOptMethod optMethod)
        {
            this.formerIneq = formerIneq;
            this.slackIndex = slackIndex;
            this.optMethod = optMethod;
        }

        #region Implementation of IOptFunction

        public double h { get; set; }
        public differentiate findDerivBy { get; set; }
        public int numEvals { get; private set; }
        public double calculate(double[] x)
        {
            return optMethod.calculate(formerIneq,x) + x[slackIndex] * x[slackIndex];
        }
        #endregion
    }
}