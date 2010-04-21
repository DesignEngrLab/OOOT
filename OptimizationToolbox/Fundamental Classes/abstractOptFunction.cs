using System;

using System.Collections.Generic;
using System.Xml.Serialization;
using StarMathLib;


namespace OptimizationToolbox
{
    public abstract class abstractOptFunction
    {
        private double h = 0.05;
        protected differentiate findDerivBy = differentiate.Analytic;
        private double[] xlast;
        [XmlIgnore]
        public int numEvals = 0;

        private double flast;

        #region Constructors
        public abstractOptFunction() { }
        public abstractOptFunction(differentiate d, double h)
        {
            this.findDerivBy = d;
            this.h = h;
        }
        public abstractOptFunction(differentiate d)
        {
            this.findDerivBy = d;
        }
        #endregion

        #region Public Functions to execute equation
        public double calculate(double[] x)
        {
            if (same(x)) return flast;
            else
            {
                numEvals++;
                flast = calc(x);
                xlast = (double[])x.Clone();
                return flast;
            }
        }
        public double[] gradient(double[] x)
        {
            int n = x.GetLength(0);
            double[] grad = new double[n];
            for (int i = 0; i != n; i++)
                grad[i] = deriv_wrt_xi(x, i);
            return grad;
        }
        public double[] gradient(double[] x, List<int> workingSet)
        {
            int size = workingSet.Count;
            double[] grad = new double[size];
            for (int i = 0; i != size; i++)
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
            double[] xStep1 = (double[])x.Clone();
            double[] xStep2 = (double[])x.Clone();
            xStep1[i] += h;
            xStep2[i] -= h;
            numEvals += 2;
            return (calc(xStep1) - calc(xStep2)) / (2 * h);
        }

        private double calcForward1(double[] x, int i)
        {
            double[] xStep1 = (double[])x.Clone();
            xStep1[i] += h;
            numEvals++;
            return (calc(xStep1) - calculate(x)) / h;
        }

        private double calcBack1(double[] x, int i)
        {
            double[] xStep1 = (double[])x.Clone();
            xStep1[i] -= h;
            numEvals++;
            return (calculate(x) - calc(xStep1)) / h;
        }

        private double calcBack2(double[] x, int i)
        {
            double[] xStep1 = (double[])x.Clone();
            xStep1[i] -= h;

            double[] xStep2 = (double[])x.Clone();
            xStep2[i] -= 2 * h;
            numEvals += 2;
            return (calc(xStep2) - 4 * calc(xStep1) + 3 * calculate(x)) / (2 * h);
        }
        private double calcForward2(double[] x, int i)
        {
            double[] xStep1 = (double[])x.Clone();
            xStep1[i] += h;

            double[] xStep2 = (double[])x.Clone();
            xStep2[i] += 2 * h;
            numEvals += 2;
            return (-3 * calculate(x) + 4 * calc(xStep1) - calc(xStep2)) / (2 * h);
        }
        private double calcCentral4(double[] x, int i)
        {
            double[] forStep1 = (double[])x.Clone();
            forStep1[i] += h;
            double[] forStep2 = (double[])x.Clone();
            forStep2[i] += 2 * h;
            double[] backStep1 = (double[])x.Clone();
            backStep1[i] -= h;
            double[] backStep2 = (double[])x.Clone();
            backStep2[i] -= 2 * h;
            numEvals += 4;
            return (calc(backStep2) - 8 * calc(backStep1)
                + 8 * calc(forStep1) - calc(forStep2)) / (12 * h);
        }
        #endregion
        private Boolean same(double[] x)
        {
            return (StarMath.norm1(x, xlast)
                < (x.GetLength(0) * double.Epsilon));
        }


    }
}
