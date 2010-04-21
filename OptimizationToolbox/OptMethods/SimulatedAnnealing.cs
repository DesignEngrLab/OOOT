using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class SimulatedAnnealing : abstractOptMethod
    {
        double[] xk, xkLast, gradF, dk;
        double fk, fStar, alphaStar;



        #region Constructor
        public SimulatedAnnealing()
        {
        }
        #endregion


        #region Main Function, run
        public override double run(double[] x0, out double[] xStar)
        {
            throw new NotImplementedException();
            #region Initialization
            /* Initialize xStar so that something can be returned if the search crashes. */
            if (x0 != null) xStar = (double[])x0.Clone();
            else xStar = new double[0];
            /* initialize and check is part of the abstract class. GRG requires a feasible start point
             * so if none is found, we return infinity.*/
            if (!initializeAndCheck(ref x0)) return fStar;
            xk = (double[])x0.Clone();
            //evaluate f(x0)
            fStar = fk = calc_f(xk);
            dk = new double[n];
            // k = 0 --> iteration counter
            k = 0;
            #endregion


        }
        #endregion


    }
}
