using StarMathLib;

namespace OptimizationToolbox
{
    public class PowellsMethodOptimization : abstractOptMethod
    {
        /* xk is the value of x at a particular iteration, k. xkLast is the previous
         * value. gradF is the gradient of f and dk is the search direction at iteration
         * k. All of these vectors have the same length which is not set until the run
         * function is called. */
        double[] dk, dconj, xl;

        /* fk is the value of f(xk). */
        double fk;


        /* alphaStar is what is returned by the line search (1-D) search method. It is used
         * to update xk. */
        double alphaStar;
        int searchDirColumn;
        //Each column represents a potential search direction
        double[,] searchDirMatrix;

        #region Constructor
        public PowellsMethodOptimization(double penaltyWeight)
        {
            this.ConstraintsSolvedWithPenalties = true;
            this.RequiresSearchDirectionMethod = false;
            this.RequiresLineSearchMethod = true;
           // this.penaltyWeight = penaltyWeight;
            this.k = 0;
        }
        #endregion


        #region Main Function, run
        protected override double run(out double[] xStar)
        {
            #region Initialization
            xStar = null;
            //evaluate f(x0)
            fStar = fk = calc_f(x);
            dk = new double[n];
            dconj = new double[n];
            #endregion

            do
            {
                /* Powell's optimization description
                 * Build n conjugate directions sweeping thru the searchDirMatrix
                n+1 times replacing the last swept column with new conjugate direction
                Start by leaving the first conjugate direction as the nth coord direction
                 
                 * For each conjStep sweep from newest conjugate step all the way back around
                 and then research in that same conjugate direction but starting with 
                 the new point arrived at after searching the other directions in between.
                 
                 * Then Powell's method says that direction created by joining the two local line
                 minimas of the last created conjugate direction will create the new conjugate direction
                 Repeat this until all conjugate directions are created*/

                /*Foreach iteration of Powell's optimization reset the search directions back
                 * to the coordinate directions*/

                #region Generate and search Conjugate directions
                this.searchDirMatrix = StarMath.makeIdentity(n);
                for (int conjStep = 0; conjStep < n; conjStep++)
                {
                    for (int row = 0; row < n; row++)
                        dconj[row] = 0;
                    if (conjStep == 0)
                        searchDirColumn = n-1;
                    else
                        searchDirColumn = conjStep - 1;
                    //Search n+1 directions
                    for (int searchDirNum = 0; searchDirNum < n + 1; searchDirNum++)
                    {
                        dk = StarMath.GetColumn(searchDirColumn, searchDirMatrix);
                        // use line search (arithmetic mean) to find alphaStar
                        alphaStar = lineSearchMethod.findAlphaStar(x, dk, true);
                        xl = StarMath.multiply(alphaStar, dk);
                        x = StarMath.add(x, xl);
                        if (searchDirNum > 0)
                            dconj = StarMath.add(dconj, xl);
                        searchDirColumn++;
                        if (searchDirColumn > n-1)
                            searchDirColumn = 0;
                    }
                    StarMath.SetColumn(conjStep, searchDirMatrix, dconj);
                }
                #endregion

                #region Finally search along all the conjugate directions now in the matrix
                for (int searchDirNum = 0; searchDirNum < n; searchDirNum++)
                {
                    dk = StarMath.GetColumn(searchDirNum, searchDirMatrix);
                    // use line search (arithmetic mean) to find alphaStar
                    alphaStar = lineSearchMethod.findAlphaStar(x, dk, true);
                    xl = StarMath.multiply(alphaStar, dk);
                    x = StarMath.add(x, xl);
                }
                #endregion

                #region Output, Check if fstar should be updated, Increment k
                SearchIO.output("iteration=" + k, 3);
                k++;
                fk = calc_f(x);
                if (fk < fStar)
                {
                    fStar = fk;
                    xStar = (double[])x.Clone();
                }
                SearchIO.output("f = " + fk, 3);
                #endregion
            }
            while (notConverged(k, fk,x));

            return fStar;
        }


        #endregion


    }
}
