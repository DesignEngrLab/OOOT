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
    public class equalToValue : equality
    {
        public double constant;
        public int index;

        #region Constructor
        public equalToValue() { }
        public equalToValue(double constant, int index)
        {
            this.constant = constant;
            this.index = index;
        }
        #endregion

        protected override double calc(double[] x)
        {
            return x[index] - constant;
        }
        public override double deriv_wrt_xi(double[] x, int i)
        {
            if (i == index) return 1.0;
            else return 0.0;
        }
    }
}
