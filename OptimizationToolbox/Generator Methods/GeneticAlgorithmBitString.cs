using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class GeneticAlgorithmBitString : abstractOptMethod
    {
        double[] xk, xkLast, gradF, dk;
        double fk, fStar, alphaStar;



        #region Constructor
        public GeneticAlgorithmBitString()
        {
        }
        #endregion


        #region Main Function, run
        public override double run(double[] x0, out double[] xStar)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
