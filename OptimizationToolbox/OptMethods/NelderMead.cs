using System;
using System.Collections.Generic;


namespace OptimizationToolbox
{
    public class NelderMead : abstractOptMethod
    {
        #region Fields
        double rho = 1;
        double chi = 2;
        double psi = 0.5;
        double sigma = 0.5;
        double initNewPointPercentage = 0.01;
        double initNewPointAddition = 0.5;
        SortedList<double, IList<double>> vertices = new SortedList<double, IList<double>>(new optimizeSort(optimize.minimize));
        #endregion


        #region Constructor
        public NelderMead(Boolean defaultMethod = true, double eps = 0.0001)
        {
            this.ConstraintsSolvedWithPenalties = true;
            this.SearchDirectionMethodNeeded = false;
            this.LineSearchMethodNeeded = false;
            this.epsilon = eps;
            if (defaultMethod)
                this.Add(new NelderMeadConvergence(500, 20, epsilon, 0.01));
        }
        #endregion

        public override double run(double[] x0, out double[] xStar)
        {
            #region Initialize the parameters for creating simplex
            /* Initialize xStar so that something can be returned if the search crashes. */
            if (x0 != null) xStar = (double[])x0.Clone();
            else xStar = new double[0];
            /* initialize and check is part of the abstract class. GRG requires a feasible start point
             * so if none is found, we return infinity.*/
            if (!initializeAndCheck(ref x0)) return fStar;
            // k = 0 --> iteration counter
            k = 0;

            vertices.Add(calc_f(x0), x0);
            #endregion


            // Creating neighbors in each direction and evaluating them
            for (int i = 0; i < n; i++)
            {
                double[] y = (double[])x0.Clone();
                y[i] = (1 + initNewPointPercentage) * y[i] + initNewPointAddition;
                vertices.Add(calc_f(y), y);
            }

            while (!convergeMethod.converged(k, fStar, null, null, vertices.Values))
            {
                #region Compute the REFLECTION POINT
                // computing the average for each variable for n variables NOT n+1
                double sumX;
                double[] Xm = new double[n];
                for (int dim = 0; dim < n; dim++)
                {
                    sumX = 0;
                    for (int j = 0; j < n; j++)
                        sumX += vertices.Values[j][dim];
                    Xm[dim] = sumX / n;
                }

                double[] Xr = CloneVertex(vertices.Values[n]);
                for (int i = 0; i < n; i++)
                    Xr[i] = (1 + rho) * Xm[i] - rho * Xr[i];
                double fXr = calc_f(Xr);
                #endregion

                #region if reflection point is better than best
                if (fXr < vertices.Keys[0])
                {
                    #region Compute the Expansion Point
                    double[] Xe = CloneVertex(vertices.Values[n]);
                    for (int i = 0; i < n; i++)
                        Xe[i] = (1 + rho * chi) * Xm[i] - rho * chi * Xe[i];
                    double fXe = calc_f(Xe);
                    #endregion
                    if (fXe < fXr)
                    {
                        vertices.RemoveAt(n);
                        vertices.Add(fXe, Xe);
                        SearchIO.output("expand", 4);
                    }
                    else
                    {
                        vertices.RemoveAt(n);
                        vertices.Add(fXr, Xr);
                        SearchIO.output("reflect", 4);
                    }
                }
                #endregion
                #region if reflection point is NOT better than best
                else
                {
                    #region but it's better than second worst, still do reflect
                    if (fXr < vertices.Keys[n - 1])
                    {
                        vertices.RemoveAt(n);
                        vertices.Add(fXr, Xr);
                        SearchIO.output("reflect", 4);
                    }
                    #endregion
                    else
                    {
                        #region if better than worst, do Outside Contraction
                        if (fXr < vertices.Keys[n])
                        {
                            double[] Xc = CloneVertex(vertices.Values[n]);
                            for (int i = 0; i < n; i++)
                                Xc[i] = (1 + rho * psi) * Xm[i] - rho * psi * Xc[i];
                            double fXc = calc_f(Xc);

                            if (fXc <= fXr)
                            {
                                vertices.RemoveAt(n);
                                vertices.Add(fXc, Xc);
                                SearchIO.output("outside constract", 4);
                            }
                        #endregion
                            #region Shrink all others towards best
                            else
                            {
                                for (int j = 1; j < n + 1; j++)
                                {
                                    double[] Xs = CloneVertex(vertices.Values[j]);
                                    for (int i = 0; i < n; i++)
                                    {
                                        Xs[i] = vertices.Values[0][i]
                                            + sigma * (Xs[i] - vertices.Values[0][i]);
                                    }
                                    double fXs = calc_f(Xs);
                                    vertices.RemoveAt(j);
                                    vertices.Add(fXs, Xs);
                                }
                                SearchIO.output("shrink towards best", 4);
                            }
                            #endregion
                        }
                        else
                        {
                            #region Compute Inside Contraction
                            double[] Xcc = CloneVertex(vertices.Values[n]);
                            for (int i = 0; i < n; i++)
                                Xcc[i] = (1 - psi) * Xm[i] + psi * Xcc[i];
                            double fXcc = calc_f(Xcc);

                            if (fXcc < vertices.Keys[n])
                            {
                                vertices.RemoveAt(n);
                                vertices.Add(fXcc, Xcc);
                                SearchIO.output("inside contract", 4);
                            }
                            #endregion
                            #region Shrink all others towards best and flip over
                            else
                            {
                                for (int j = 1; j < n + 1; j++)
                                {
                                    double[] Xs = CloneVertex(vertices.Values[j]);
                                    for (int i = 0; i < n; i++)
                                    {
                                        Xs[i] = vertices.Values[0][i]
                                            - sigma * (Xs[i] - vertices.Values[0][i]);
                                    }
                                    double fXs = calc_f(Xs);
                                    vertices.RemoveAt(j);
                                    vertices.Add(fXs, Xs);
                                }
                                SearchIO.output("shrink towards best and flip", 4);
                            }
                            #endregion
                        }
                    }
                }
                #endregion

                k++;
                SearchIO.output("iter. = " + k.ToString(), 2);
                ////mattica SearchIO.output("Fitness = " + vertices.Keys[0].ToString(), 2);
                SearchIO.output("Fitness = " + vertices.Keys[n].ToString(), 2);

            } // END While Loop
            xStar = (double[])vertices.Values[0];
            fStar = vertices.Keys[0];
            vertices.Clear();
            return fStar;
        }

        private double[] CloneVertex(IList<double> iList)
        {
            return (double[])((double[])vertices.Values[n]).Clone();
        }

        private double[] getMaxes(SortedList<double, double[]> vertices)
        {
            double tempV, maxV;
            double[] maxes = new double[n];

            for (int dim = 0; dim < n; dim++)
            {
                maxV = double.NegativeInfinity;
                for (int j = 1; j <= n; j++)
                {
                    tempV = Math.Abs(vertices.Values[j][dim] - vertices.Values[0][dim]);
                    if (tempV > maxV) maxV = tempV;
                }
                maxes[dim] = maxV;
            }
            return maxes;
        }
    }
}
