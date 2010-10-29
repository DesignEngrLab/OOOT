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
using OptimizationToolbox;

namespace Example4_Using_Dependent_Analysis
{
    class samePitch : IEquality, IDifferentiable
    {
        int pitchIndex1;
        int pitchIndex2;

        #region Constructor
        public samePitch(int pI1, int pI2)
        {
            this.pitchIndex1 = pI1;
            this.pitchIndex2 = pI2;
        }
        #endregion



        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            return x[pitchIndex1] - x[pitchIndex2];
        }

        #endregion

        #region Implementation of IDifferentiable

        public double deriv_wrt_xi(double[] x, int i)
        {
            if (i == pitchIndex1)
                return 1.0;
            else if (i == pitchIndex2)
                return -1.0;
            return 0.0;
        }

        #endregion
    }
}
