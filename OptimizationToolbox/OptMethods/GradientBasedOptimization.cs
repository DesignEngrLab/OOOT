using StarMathLib;

namespace OptimizationToolbox
{
    public class GradientBasedOptimization : abstractOptMethod
    {
        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */
        double[] gradF, dk;

        /* fk is the value of f(xk). */
        double fk;

        /* alphaStar is what is returned by the line search (1-D) search method. It is used
         * to update xk. */
        double alphaStar;

        #region Constructor
        public GradientBasedOptimization()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            RequiresMeritFunction = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = true;
            RequiresLineSearchMethod = true;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = false;
        }
        #endregion


        #region Main Function, run
        protected override double run(out double[] xStar)
        {
            /* this is just to overcome a small issue with the compiler. It thinks that xStar will
             * not have a value since it only appears in a conditional statement below. This initi-
             * alization is to "ensure" that it does and that the code compiles. */
            xStar = null;
            //evaluate f(x0)
            fStar = fk = calc_f(x);
            do
            {
                gradF = calc_f_gradient(x);
                dk = this.searchDirMethod.find(x, gradF, fk);

                // use line search (arithmetic mean) to find alphaStar
                alphaStar = lineSearchMethod.findAlphaStar(x, dk);
                x = StarMath.add(x, StarMath.multiply(alphaStar, dk));
                SearchIO.output("iteration=" + k, 3);
                k++;
                fk = calc_f(x);
                if (fk < fStar)
                {
                    fStar = fk;
                    xStar = (double[])x.Clone();
                }
                SearchIO.output("f = " + fk, 3);
            }
            while (notConverged(k, fk, x, gradF));

            return fStar;
        }


        #endregion


    }
}
