﻿/*************************************************************************
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
using StarMathLib;


namespace OptimizationToolbox
{
    public class PowellMethod : abstractSearchDirection
    {
        private readonly double minimumAlpha = 0.001;
        private int innerK;
        private double[] xLast;
        private List<double[]> searchDirections;
        private readonly int n;
        private int itersToReset;
        private Boolean lastPass;


        public PowellMethod(int n, double minimumAlpha = 0.001, int itersToReset = -1)
        {
            this.n = n;
            this.minimumAlpha = minimumAlpha;
            this.itersToReset = itersToReset;
            initSearchDirections();
        }
        public override double[] find(double[] x, double[] gradf, double f, ref double initAlpha, Boolean reset = false)
        {
            if (lastPass)
            {
                var newDirection = StarMath.subtract(x, xLast);
                newDirection = StarMath.normalize(newDirection);
                searchDirections.RemoveAt(0);
                searchDirections.Add(newDirection);
                lastPass = false;
                innerK = 0;
            }
            if (reset || (itersToReset-- == 0)
                || ((Math.Abs(initAlpha) > 0) && (Math.Abs(initAlpha) <= minimumAlpha)))
            {
                initSearchDirections();
                innerK = 0;
                /* after one full pass, start a spacer stage. */
            }
            if (innerK == 1) xLast = (double[])x.Clone();
            var d = searchDirections[innerK % n];

            if (innerK == n) { innerK = 0; lastPass = true; }
            else innerK++;
            return d;
        }

        void initSearchDirections()
        {
            var identity = StarMath.makeIdentity(n);
            searchDirections = new List<double[]>();
            for (int i = 0; i < n; i++)
            {
                searchDirections.Add(StarMath.GetRow(i, identity));
            }
        }

    }
}
