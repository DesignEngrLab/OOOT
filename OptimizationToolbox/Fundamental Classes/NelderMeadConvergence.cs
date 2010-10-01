using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarMathLib;

namespace OptimizationToolbox
{
    public class NelderMeadConvergence : abstractConvergence
    {
        int kmax;
        int maxAge;
        //double deltaX;
        double deltaF;
        double explore;

        #region Properties
        //int _kmax = int.MaxValue;
        //int _maxAge = int.MaxValue;
        //double _deltaX = 0.0;
        //double _deltaF = 0.0;
        //double _deltaGradF = 0.0;
        //public int kmax
        //{
        //    get { return _kmax; }
        //    set
        //    {
        //        if ((value != _kmax) && (value != int.MaxValue)
        //             && (value != int.MaxValue) && (value != 0))
        //        {
        //            _kmax = value;
        //            kmaxSet = true;
        //        }
        //    }
        //}
        //public int maxAge
        //{
        //    get { return _maxAge; }
        //    set
        //    {
        //        if ((value != _maxAge) && (value != int.MaxValue)
        //             && (value != int.MaxValue) && (value != 0))
        //        {
        //            _maxAge = value;
        //            //ageOfBestSet = true;
        //        }
        //    }
        //}
        //public double deltaX
        //{
        //    get { return _deltaX; }
        //    set
        //    {
        //        if ((value != _deltaX) && (value != double.Epsilon)
        //             && (value != double.MinValue) && (value != double.MaxValue)
        //             && (!double.IsInfinity(value)) && (!double.IsNaN(value)) && (value != 0.0))
        //        {
        //            _deltaX = value;
        //            deltaXSet = true;
        //        }
        //    }
        //}
        //public double deltaF
        //{
        //    get { return _deltaF; }
        //    set
        //    {
        //        if ((value != _deltaF) && (value != double.Epsilon)
        //             && (value != double.MinValue) && (value != double.MaxValue)
        //             && (!double.IsInfinity(value)) && (!double.IsNaN(value)) && (value != 0.0))
        //        {
        //            _deltaF = value;
        //            deltaFSet = true;
        //        }
        //    }
        //}
        //public double deltaGradF
        //{
        //    get { return _deltaGradF; }
        //    set
        //    {
        //        if ((value != _deltaGradF) && (value != double.Epsilon)
        //             && (value != double.MinValue) && (value != double.MaxValue)
        //             && (!double.IsInfinity(value)) && (!double.IsNaN(value)) && (value != 0.0))
        //        {
        //            _deltaGradF = value;
        //            //deltaGradFSet = true;
        //        }
        //    }
        //}
        #endregion
        public NelderMeadConvergence(int maxK, int ageMax, double fDelta, double exploratoryFactor = 2)
        {
            this.maxAge = ageMax;
            this.kmax = maxK;
            //xDelta = deltaX;
            this.deltaF = fDelta;
            this.explore = exploratoryFactor;
        }
        bool result = false;
        int ageBest = 0;
        int lastBestAge = 0;
        double lastBestF = double.PositiveInfinity;
        double fPrev = double.PositiveInfinity;
        double[] lastBestX;
        
        /// <summary>
        /// Nelder Mead Convergence Criteria
        /// </summary>
        /// <param name="k">Current iteration</param>
        /// <param name="x">Coordinate Vector of the best vertex of the Current Simplex</param>
        /// <param name="f">Evaluation Value of the best vertex of the Current Simplex</param>
        /// <param name="gradf">!!Not the Gradient!! Actually the Position Vector of Worst Point</param>
        /// <returns></returns>
        public override bool converged(int k, double[] x, double f, double[] gradf)
        {
            if (lastBestX == null)
            {
                lastBestX = new double[x.GetLength(0)];
                for (int i = 0; i < lastBestX.Length; i++ )
                    lastBestX[i] = double.PositiveInfinity;
            }

            result = sameX(x);
            if (result) //A better point was not found
            {
                ageBest++;
                
                result = (k >= kmax) || (ageBest >= maxAge) && 
                    ((ageBest)/(lastBestAge+1) > (explore));// && ((Math.Abs(f - lastBestF) <= deltaF));
            }
            else //Only when a better point is found
            {
                lastBestAge = ageBest;
                ageBest = 0;
                result = (k >= kmax);
                lastBestX = x;
                lastBestF = fPrev;  
            }
            fPrev = f;
            return result;
        }

        private bool sameX(double[] x)
        {
            return (StarMath.norm1(x, lastBestX) < (x.GetLength(0) * double.Epsilon));
        }
        //private Boolean same(double[] x)
        //{
        //    if (xlast == null) return false;
        //    return (StarMath.norm1(x, xlast)
        //        < (x.GetLength(0) * double.Epsilon));
        //}
    }
}
