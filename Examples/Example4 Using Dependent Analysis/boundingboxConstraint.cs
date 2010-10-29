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
using OptimizationToolbox;

namespace Example4_Using_Dependent_Analysis
{
    class boundingboxConstraint : IInequality
    {
        private readonly ForceVelocityPositionAnalysis FVPAnalysis;

        private readonly double minX;
        private readonly double maxX;
        private readonly double minY;
        private readonly double maxY;
        private readonly double minZ;
        private readonly double maxZ;

        public boundingboxConstraint(ForceVelocityPositionAnalysis fvpAnalysis, double minX, double maxX,
                                        double minY, double maxY, double minZ, double maxZ)
        {
            FVPAnalysis = fvpAnalysis;
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            this.minZ = minZ;
            this.maxZ = maxZ;
        }


        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            var p = double.NegativeInfinity;
            for (int i = 0; i < FVPAnalysis.numGears; i++)
            {
                p = Math.Max(p, ((FVPAnalysis.positions[i][0, 3] + (FVPAnalysis.diameters[i] / 2)) - maxX));
                p = Math.Max(p, (minX - (FVPAnalysis.positions[i][0, 3] - (FVPAnalysis.diameters[i] / 2))));
                p = Math.Max(p, ((FVPAnalysis.positions[i][1, 3] + (FVPAnalysis.diameters[i] / 2)) - maxY));
                p = Math.Max(p, (minY - (FVPAnalysis.positions[i][1, 3] - (FVPAnalysis.diameters[i] / 2))));
                p = Math.Max(p, ((FVPAnalysis.positions[i][2, 3] + (FVPAnalysis.diameters[i] / 2)) - maxZ));
                p = Math.Max(p, (minZ - (FVPAnalysis.positions[i][2, 3] - (FVPAnalysis.diameters[i] / 2))));
            }
            return p;
        }

        #endregion
    }
}
