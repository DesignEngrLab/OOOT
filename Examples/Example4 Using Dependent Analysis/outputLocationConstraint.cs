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
using StarMathLib;

namespace Example4_Using_Dependent_Analysis
{
    internal class outputLocationConstraint : IEquality
    {
        private readonly ForceVelocityPositionAnalysis FVPAnalysis;
        private readonly double tolerance;

        private readonly double xtarget;
        private readonly double ytarget;
        private readonly double ztarget;

        #region Constructor

        public outputLocationConstraint(ForceVelocityPositionAnalysis fvpAnalysis, double tolerance,
            double xtarget, double ytarget, double ztarget)
        // double thetaX, double thetaY, double thetaZ)
        {
            FVPAnalysis = fvpAnalysis;
            this.tolerance = tolerance;
            this.xtarget = xtarget;
            this.ytarget = ytarget;
            this.ztarget = ztarget;
            //this.thetaX = thetaX;
            //this.thetaY = thetaY;
            //this.thetaZ = thetaZ;
        }

        #endregion

        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            var targetOut = new[] { xtarget, ytarget, ztarget, 1.0 };
            var candidateOut = StarMath.GetColumn(3, FVPAnalysis.positions[FVPAnalysis.numGears - 1]);
            var hVal = StarMath.norm2(targetOut, candidateOut, true);
            if (hVal < tolerance) return 0.0;
            return hVal;
        }

        #endregion
    }
}