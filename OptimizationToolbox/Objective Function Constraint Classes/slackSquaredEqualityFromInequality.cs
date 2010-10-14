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
    class slackSquaredEqualityFromInequality : equality
    {
        int slackIndex;
        abstractOptFunction formerIneq;
        public slackSquaredEqualityFromInequality(abstractOptFunction formerIneq, int slackIndex)
        {
            this.formerIneq = formerIneq;
            this.slackIndex = slackIndex;
        }

        protected override double calc(double[] x)
        {
            return formerIneq.calculate(x) + x[slackIndex] * x[slackIndex];
        }

        public override double deriv_wrt_xi(double[] x, int i)
        {
            // the reason this returns 2xi is that the slack variable is squared, xi^2
            // (see calculate above), meaning the derivative is 2xi
            if (i == slackIndex) return 2.0 * x[i];

            else return formerIneq.deriv_wrt_xi(x, i);
        }
    }
}
