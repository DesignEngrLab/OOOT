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
using System.Collections.Generic;
using StarMathLib;
using System;
using System.Linq;
using SomeFunctions;


namespace OptimizationToolbox
{
    public class NelderMead : abstractOptMethod
    {
        #region Fields

		//alpha beta gamma delta 1,1+2/n,0.75-1/2n,1-1/n

		private readonly double chi = 2; //beta
		//private readonly double chi = 1+2/n;

		private readonly double initNewPointAddition = 0.5;
		private readonly double initNewPointPercentage = 0.01;

		private readonly double psi = 0.5; //gamma
		//private readonly double psi = 0.75-1/(2*8);

		private readonly double rho = 1; //alpha)
		//private readonly double rho = 1; 

		private readonly double sigma = 0.5; //delta
		//private readonly double sigma = 1-1/8; 

		private readonly double tau=0.5; 


		private const double minAlphaStepRatio = 1e-6;
		private double alphaStar;
		private double[] dk;
		private double fk;
		private readonly SortedList<double, double[]> vertices = 
			new SortedList<double, double[]>(new optimizeSort(optimize.minimize));
		private static SomeFunctions.Values trial_trial;
		private static SomeFunctions.Values f_val;
		private List<double[]> trialValues = new List<double[]>();

        #endregion

        #region Constructor

        public NelderMead()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
			RequiresLineSearchMethod = true;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
            RequiresDiscreteSpaceDescriptor = false;
        }

		public NelderMead(double alpha, double beta, double gamma, double delta, double initNewPointPercentage = double.NaN,
                          double initNewPointAddition = double.NaN)
            : this()
        {
			this.rho = alpha;
			this.chi = beta;
			this.psi = gamma;
			this.sigma = delta;
            if (!double.IsNaN(initNewPointPercentage)) this.initNewPointPercentage = initNewPointPercentage;
            if (!double.IsNaN(initNewPointAddition)) this.initNewPointAddition = initNewPointAddition;
        }

		public NelderMead(List<double[]> trialValues,double alpha, double beta, double gamma, double delta):this()
		{

			this.trialValues = trialValues;
			this.rho = alpha;
			this.chi = beta;
			this.psi = gamma;
			this.sigma = delta;

		}


        #endregion

        protected override double run(out double[] xStar)
        {



			trial_trial = new SomeFunctions.Values (x);

			trial_trial.addtoValues (x);
			var f_value = calc_f (x);
			f_val = new SomeFunctions.Values (f_value);



			vertices.Add( f_value, x);
			if(trialValues.Count==0)
			{

			#region generating neighbors
            // Creating neighbors in each direction and evaluating them
			//find the unit vector for the current x;
			double[] unitV = new double[n];
			double sum = 0.0;
			for (int i = 0; i < n; i++)
				sum += Math.Pow(x [i],2);
			sum = Math.Sqrt (sum);
			for (int i = 0; i < n; i++) 
				unitV [i] = x [i] / sum;
		
		
            for (var i = 0; i < n; i++)
            {
                var y = (double[])x.Clone();
				// y[i] = (1 + initNewPointPercentage) * y[i] + initNewPointAddition;
				y [i] = y [i] + tau * unitV [i];
				f_value = calc_f (y);
				vertices.Add(f_value, y);
				trial_trial.addtoValues (y);
				f_val.addtoValues (f_value);

            }
			}

			#endregion 

			else
			{
				for(int i=1;i<trialValues.Count;i++)
					vertices.Add(calc_f(trialValues[i]),trialValues[i]);

				var y = (double[])x.Clone();
				y[0] = (1 + initNewPointPercentage) * y[0] + initNewPointAddition;
				f_value = calc_f (y);
				vertices.Add(f_value, y);
				y = (double[])x.Clone();
				y[1] = (1 + initNewPointPercentage) * y[1] + initNewPointAddition;
				f_value = calc_f (y);
				vertices.Add(f_value, y);
				y = (double[])x.Clone();
				y[2] = (1 + initNewPointPercentage) * y[2] + initNewPointAddition;
				f_value = calc_f (y);
				vertices.Add(f_value, y);

			}
			var iteration = 0;

			while (notConverged(k, numEvals, vertices.Keys[0], vertices.Values[0], vertices.Values))
			//	while(iteration<100)
            {
				iteration++;
                #region Compute the REFLECTION POINT

                // computing the average for each variable for n variables NOT n+1
                var Xm = new double[n];
				for (var dim = 0; dim < n; dim++)
                {
					double sumX = 0;
                    for (var j = 0; j < n; j++)
						sumX += vertices.Values[dim][j];
                    Xm[dim] = sumX / n;
                }

				var Xr = CloneVertex(vertices.Values[n]);
                for (var i = 0; i < n; i++)
                    Xr[i] = (1 + rho) * Xm[i] - rho * Xr[i];
                var fXr = calc_f(Xr);
				trial_trial.addtoValues (Xr);
				f_val.addtoValues (fXr);




//				var Xr1 = CloneVertex(vertices.Values[0]);
//				for (var i = 0; i < n; i++)
//					Xr1[i] = (1 + rho) * Xm[i] - rho * Xr1[i];
//				var fXr1 = calc_f(Xr1);
               SearchIO.output("x_r = " + StarMathLib.StarMath.MakePrintString(Xr), 4);
                #endregion



                #region if reflection point is better than best

                if (fXr < vertices.Keys[0])
                {
                    #region Compute the Expansion Point

                    var Xe = CloneVertex(vertices.Values[n]);
                    for (var i = 0; i < n; i++)
                        Xe[i] = (1 + rho * chi) * Xm[i] - rho * chi * Xe[i];
                    var fXe = calc_f(Xe);
					trial_trial.addtoValues (Xe);
					f_val.addtoValues (fXe);

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
					 

					#region but it's better than second worst, still do reflect NOT USED

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

						if (fXr >= vertices.Keys[n-1] && fXr < vertices.Keys[n])
                        {
                            var Xc = CloneVertex(vertices.Values[n]);
                            for (var i = 0; i < n; i++)
                                Xc[i] = (1 + rho * psi) * Xm[i] - rho * psi * Xc[i];
                            var fXc = calc_f(Xc);
							trial_trial.addtoValues (Xc);
							f_val.addtoValues (fXc);

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
								List<double> newFs = new List<double>();
								List<double[]> newXs = new List<double[]>();
								newFs.Add(vertices.Keys[0]);
								newXs.Add(vertices.Values[0]);
                                for (var j = 1; j < n + 1; j++)
                                {
                                    var Xs = CloneVertex(vertices.Values[j]);
                                    for (var i = 0; i < n; i++)
                                    {
                                        Xs[i] = vertices.Values[0][i]
                                                + sigma * (Xs[i] - vertices.Values[0][i]);
                                    }
                                    var fXs = calc_f(Xs);
									f_val.addtoValues (fXs);

//									
//									{
//
//									vertices.RemoveAt(j);
//										//vertices.RemoveAt(n);
//                                    vertices.Add(fXs, Xs);
//									trial_trial.addtoValues (Xs);
//									}

									newFs.Add(fXs);
									newXs.Add(Xs);
                                }

								int count=1;
								while(vertices.Count>1)
									vertices.RemoveAt(count);

								for(int i=1;i<n+1;i++)
									vertices.Add(newFs[i],newXs[i]);

                                SearchIO.output("shrink towards best", 4);
                            }

                            #endregion
                        }
						else if(fXr >= vertices.Keys[n-1] && fXr>=vertices.Keys[n])
                        {
                            #region Compute Inside Contraction

                            var Xcc = CloneVertex(vertices.Values[n]);
                            for (var i = 0; i < n; i++)
                                Xcc[i] = (1 - psi) * Xm[i] + psi * Xcc[i];
                            var fXcc = calc_f(Xcc);
							trial_trial.addtoValues (Xcc);
							f_val.addtoValues (fXcc);

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
								List<double> newFs = new List<double>();
								List<double[]> newXs = new List<double[]>();
								newFs.Add(vertices.Keys[0]);
								newXs.Add(vertices.Values[0]);
                                for (var j = 1; j < n + 1; j++)
                                {
                                    var Xs = CloneVertex(vertices.Values[j]);
                                    for (var i = 0; i < n; i++)
                                    {
                                        Xs[i] = vertices.Values[0][i]
											- sigma * (Xs[i] - vertices.Values[0][i]);
                                    }
                                    var fXs = calc_f(Xs);
									f_val.addtoValues (fXs);

//									//if(fXs<vertices.Keys[n])
//									/*new code*/ 
//									//if(fXs<vertices.Keys[n])
//									{
//										//vertices.RemoveAt(j);
//										vertices.RemoveAt(j);
//                                        vertices.Add(fXs, Xs);
//										trial_trial.addtoValues (Xs);
//									}

									newFs.Add(fXs);
									newXs.Add(Xs);



								}
								int count=1;
								while(vertices.Count>1)
									vertices.RemoveAt(count);

								for(int i=1;i<n+1;i++)
									vertices.Add(newFs[i],newXs[i]);

                                SearchIO.output("shrink towards best and flip", 4);
                            }

                            #endregion
                        }
                    }
                }



                #endregion

				#region neighborhood search - not used
		
//				for (int x1 = 0; x1 <= n; x1++) {
//					double[] initPoints11 = (double[])vertices.Values [x1].Clone ();
//					var new_tral = trialValues;
//					for (int l = 0; l < trialValues.Count; l++) {
//						new_tral [l] = StarMath.add (initPoints11, trialValues [l]);
//					}
//
//					for (int i = 0; i < new_tral.Count; i++) {
//						var fNew = calc_f (new_tral [i]);
//
//						if (fNew < vertices.Keys [n]) {
//							//SearchIO.output ("neighborhood search " + fNew);
//							vertices.RemoveAt (n);
//							vertices.Add (fNew, new_tral [i]);
//
//						}
//
//
//					}
//				}
//
//				for (int x1 = 1; x1 < n; x1++) {
//					double[] initPoints11 = (double[])vertices.Values [x1].Clone ();
//					SomeFunctions.MyClass trials = new SomeFunctions.MyClass ();
//					var new_tral = trials.generateNeighbors (initPoints11);
//
//					for (int i = 0; i < new_tral.Count; i++) {
//						var fNew = calc_f (new_tral [i]);
//
//						if (fNew < vertices.Keys [n]) {
//							//SearchIO.output ("neighborhood search " + fNew);
//							vertices.RemoveAt (n);
//							vertices.Add (fNew, new_tral [i]);
//
//						}
//
//
//					}
//
//
//
//				}

				#endregion


				#region golden section 

				var conjugateDirections = new List<double[]> ();

					for (int ij = 0; ij < n; ij++) {
						var direction = new double[n];
					direction [ij] = 1;
						conjugateDirections.Add (direction);

					}

//				SearchIO.output ("Before GS:" + vertices.Keys[0]);
//					
//				/*for (int i = 0; i < n; i++)*/ {
					var xinner = (double[])vertices.Values [n].Clone ();
				
					for (int ij = 0; ij < n; ij++) {

						var dk = conjugateDirections [ij];
						var xinner1 = xinner;
						//var xinner1 = (double[])vertices.Values[n].Clone();

						alphaStar = lineSearchMethod.findAlphaStar (xinner1, dk, true);
						xinner1 = StarMath.add (xinner1, StarMath.multiply (alphaStar, dk));
						trial_trial.addtoValues (xinner1);
						var fNew = calc_f (xinner1);
						f_val.addtoValues (fNew);
						if (fNew < vertices.Keys [n]) {
							vertices.RemoveAt (n);
							vertices.Add (fNew, xinner1);
							//SearchIO.output ("golden section:" + fNew);

						}


					}
//
//				}

				#endregion 


				






                k++;
//                SearchIO.output("iter. = " + k);
//				SearchIO.output("Fitness = " + vertices.Keys[0], 2);
//
				SearchIO.output ("After GS:" + vertices.Keys[0]);

            } // END While Loop
            xStar = vertices.Values[0];
            fStar = vertices.Keys[0];

            vertices.Clear();

			//trial_trial.generatePercentageForEach (-100,100);
		
            return fStar;
        }

        private static double[] CloneVertex(double[] vertex)
        {
            return (double[])vertex.Clone();
        }

		public static SomeFunctions.Values Trials
		{

			get {return trial_trial; }
			set {trial_trial = value; }

		}

		public static SomeFunctions.Values FVal
		{

			get {return f_val; }
			set {f_val = value; }

		}
    }
}