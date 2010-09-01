using System;

using System.Xml.Serialization;
using StarMathLib;

namespace OptimizationToolbox
{
    public class convergenceBasic : abstractConvergence
    {
        #region Fields
        int _kmax = int.MaxValue;
        int _maxAge = int.MaxValue;
        double _deltaX = 0.0;
        double _deltaF = 0.0;
        double _deltaGradF = 0.0;
        Boolean kmaxSet = false;
        Boolean deltaXSet = false;
        Boolean deltaFSet = false;
        Boolean deltaGradFSet = false;
        Boolean ageOfBestSet = false;

        BasicConvergenceTypes convergeMethod;

        int age = 0;
        double[] xlast;
        double flast;
        #endregion
        #region Properties
        public int kmax
        {
            get { return _kmax; }
            set
            {
                if ((value != _kmax) && (value != int.MaxValue)
                     && (value != int.MaxValue) && (value != 0))
                {
                    _kmax = value;
                    kmaxSet = true;
                }
            }
        }
        public int maxAge
        {
            get { return _maxAge; }
            set
            {
                if ((value != _maxAge) && (value != int.MaxValue)
                     && (value != int.MaxValue) && (value != 0))
                {
                    _maxAge = value;
                    ageOfBestSet = true;
                }
            }
        }
        public double deltaX
        {
            get { return _deltaX; }
            set
            {
                if ((value != _deltaX) && (value != double.Epsilon)
                     && (value != double.MinValue) && (value != double.MaxValue)
                     && (!double.IsInfinity(value)) && (!double.IsNaN(value)) && (value != 0.0))
                {
                    _deltaX = value;
                    deltaXSet = true;
                }
            }
        }
        public double deltaF
        {
            get { return _deltaF; }
            set
            {
                if ((value != _deltaF) && (value != double.Epsilon)
                     && (value != double.MinValue) && (value != double.MaxValue)
                     && (!double.IsInfinity(value)) && (!double.IsNaN(value)) && (value != 0.0))
                {
                    _deltaF = value;
                    deltaFSet = true;
                }
            }
        }
        public double deltaGradF
        {
            get { return _deltaGradF; }
            set
            {
                if ((value != _deltaGradF) && (value != double.Epsilon)
                     && (value != double.MinValue) && (value != double.MaxValue)
                     && (!double.IsInfinity(value)) && (!double.IsNaN(value)) && (value != 0.0))
                {
                    _deltaGradF = value;
                    deltaGradFSet = true;
                }
            }
        }
        #endregion
        #region Constructor
        public convergenceBasic() { }
        public convergenceBasic(BasicConvergenceTypes howConverge, int kmax, double deltaX, double deltaF,
                    double deltaGradF, int bestAge)
        {
            this.convergeMethod = howConverge;
            this.kmax = kmax;
            this.deltaX = deltaX;
            this.deltaF = deltaF;
            this.deltaGradF = deltaGradF;
            this.maxAge = bestAge;
        }
        public convergenceBasic(BasicConvergenceTypes howConverge)
        {
            this.convergeMethod = howConverge;
        }
        public convergenceBasic(int kmax)
        {
            this.convergeMethod = BasicConvergenceTypes.JustMaxIterations;
            this.kmax = kmax;
        }
        public convergenceBasic(double deltaX)
        {
            this.convergeMethod = BasicConvergenceTypes.JustDeltaX;
            this.deltaX = deltaX;
        }
        #endregion


        public override Boolean converged(int k, double[] x, double f, double[] gradf)
        {
            Boolean result = false;
            if (xlast == null) return result;
            findAgeOfBest(x);
            switch (this.convergeMethod)
            {
                case BasicConvergenceTypes.AndBetweenSetConditions:
                    if (!(ageOfBestSet || kmaxSet || deltaXSet || deltaFSet || deltaGradFSet))
                        throw new Exception("None of the five possible criteria were ever set.");
                    else result = ((!kmaxSet || (k >= kmax)) &&
                            (!ageOfBestSet || (age >= maxAge)) &&
                            (!deltaXSet || (StarMath.norm1(x, xlast) <= deltaX)) &&
                            (!deltaFSet || (Math.Abs(f - flast) <= deltaF)) &&
                            (!deltaGradFSet || (StarMath.norm1(gradf) <= deltaGradF)));
                    break;
                case BasicConvergenceTypes.OrBetweenSetConditions:
                    if (!(ageOfBestSet || kmaxSet || deltaXSet || deltaFSet || deltaGradFSet))
                        throw new Exception("None of the five possible criteria were ever set.");
                    else result = ((kmaxSet && (k >= kmax)) ||
                            (ageOfBestSet && (age >= maxAge)) ||
                            (deltaXSet && (StarMath.norm1(x, xlast) <= deltaX)) ||
                            (deltaFSet && (Math.Abs(f - flast) <= deltaF)) ||
                            (deltaGradFSet && (StarMath.norm1(gradf) <= deltaGradF)));
                    break;
                case BasicConvergenceTypes.JustMaxIterations:
                    if (!kmaxSet) throw new Exception("Max. iterations never set.");
                    else result = (k >= kmax);
                    break;
                case BasicConvergenceTypes.JustDeltaX:
                    if (!deltaXSet) throw new Exception("Minimum delta x never set.");
                    else result = (StarMath.norm1(x, xlast) <= deltaX);
                    break;
                case BasicConvergenceTypes.JustDeltaF:
                    if (!deltaFSet) throw new Exception("Minium delta f never set.");
                    else result = (Math.Abs(f - flast) <= deltaF);
                    break;
                case BasicConvergenceTypes.JustDeltaGradF:
                    if (!deltaGradFSet) throw new Exception("Minimum delta gradient(f) never set.");
                    else result = (StarMath.norm1(gradf) <= deltaGradF);
                    break;
                case BasicConvergenceTypes.JustAgeOfBest:
                    if (!ageOfBestSet) throw new Exception("Maximum age of best never set.");
                    else result = (age >= maxAge);
                    break;
            }
            xlast = x;
            flast = f;
            return result;
        }

        private void findAgeOfBest(double[] x)
        {
            if ((xlast != null) && (StarMath.norm1(x, xlast) <= deltaX)) age++;
            else age = 0;
        }

    }

    [XmlInclude(typeof(convergenceBasic))]
    public abstract class abstractConvergence
    {
        public abstract Boolean converged(int k, double[] x, double f, double[] gradf);
    }

    public enum BasicConvergenceTypes
    {
        AndBetweenSetConditions,
        OrBetweenSetConditions,
        JustMaxIterations,
        JustDeltaX,
        JustDeltaF,
        JustDeltaGradF,
        JustAgeOfBest,
    }
}