using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class SimulatedAnnealing : abstractOptMethod
    {
        double[] xkLast, gradF, dk;
        double fk;



        #region Constructor
        public SimulatedAnnealing()
        {
        }
        #endregion


        #region Main Function, run
        protected override double run(out double[] xStar)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
