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
using System.Xml.Serialization;
using StarMathLib;

namespace OptimizationToolbox
{
    public abstract class abstractOptFunction
    {
        private readonly double h = 0.05;
        protected differentiate findDerivBy = differentiate.Analytic;

        private double flast;
        [XmlIgnore]
        public int numEvals;
        private double[] xlast;

        #region Constructors

        protected abstractOptFunction()
        {
        }

        protected abstractOptFunction(differentiate d, double h)
        {
            findDerivBy = d;
            this.h = h;
        }

        protected abstractOptFunction(differentiate d)
        {
            findDerivBy = d;
        }

        #endregion

        #region Public Functions to execute equation

        public double calculate(double[] x)
        {
            if (same(x)) return flast;
            numEvals++;
            flast = calc(x);
            xlast = (double[])x.Clone();
            return flast;
        }

        public double[] gradient(double[] x)
        {
            var n = x.GetLength(0);
            var grad = new double[n];
            for (var i = 0; i != n; i++)
                grad[i] = deriv_wrt_xi(x, i);
            return grad;
        }

        public double[] gradient(double[] x, List<int> workingSet)
        {
            var size = workingSet.Count;
            var grad = new double[size];
            for (var i = 0; i != size; i++)
                grad[i] = deriv_wrt_xi(x, workingSet[i]);
            return grad;
        }

        #endregion

        #region Functions to Override in derived classes

        protected abstract double calc(double[] x);

        public virtual double deriv_wrt_xi(double[] x, int i)
        {
            switch (findDerivBy)
            {
                case differentiate.Analytic:
                    throw new Exception("No deriv_wrt_xi function found to perform analytical differentiation.");

                case differentiate.Back1:
                    return calcBack1(x, i);
                case differentiate.Forward1:
                    return calcForward1(x, i);
                case differentiate.Central2:
                    return calcCentral2(x, i);
                case differentiate.Back2:
                    return calcBack2(x, i);
                case differentiate.Forward2:
                    return calcForward2(x, i);
                case differentiate.Central4:
                    return calcCentral4(x, i);
            }
            return double.NaN;
        }

        #endregion

        #region finite difference

        private double calcCentral2(double[] x, int i)
        {
            var xStep1 = (double[])x.Clone();
            var xStep2 = (double[])x.Clone();
            xStep1[i] += h;
            xStep2[i] -= h;
            numEvals += 2;
            return (calc(xStep1) - calc(xStep2)) / (2 * h);
        }

        private double calcForward1(double[] x, int i)
        {
            var xStep1 = (double[])x.Clone();
            xStep1[i] += h;
            numEvals++;
            return (calc(xStep1) - calculate(x)) / h;
        }

        private double calcBack1(double[] x, int i)
        {
            var xStep1 = (double[])x.Clone();
            xStep1[i] -= h;
            numEvals++;
            return (calculate(x) - calc(xStep1)) / h;
        }

        private double calcBack2(double[] x, int i)
        {
            var xStep1 = (double[])x.Clone();
            xStep1[i] -= h;

            var xStep2 = (double[])x.Clone();
            xStep2[i] -= 2 * h;
            numEvals += 2;
            return (calc(xStep2) - 4 * calc(xStep1) + 3 * calculate(x)) / (2 * h);
        }

        private double calcForward2(double[] x, int i)
        {
            var xStep1 = (double[])x.Clone();
            xStep1[i] += h;

            var xStep2 = (double[])x.Clone();
            xStep2[i] += 2 * h;
            numEvals += 2;
            return (-3 * calculate(x) + 4 * calc(xStep1) - calc(xStep2)) / (2 * h);
        }

        private double calcCentral4(double[] x, int i)
        {
            var forStep1 = (double[])x.Clone();
            forStep1[i] += h;
            var forStep2 = (double[])x.Clone();
            forStep2[i] += 2 * h;
            var backStep1 = (double[])x.Clone();
            backStep1[i] -= h;
            var backStep2 = (double[])x.Clone();
            backStep2[i] -= 2 * h;
            numEvals += 4;
            return (calc(backStep2) - 8 * calc(backStep1)
                    + 8 * calc(forStep1) - calc(forStep2)) / (12 * h);
        }

        #endregion

        private Boolean same(double[] x)
        {
            if ((x == null) || (xlast == null)) return false;
            return (StarMath.norm1(x, xlast)
                    < (x.GetLength(0) * double.Epsilon));
        }
    }
}