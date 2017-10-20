using System;
using System.Collections.Generic;
using StarMathLib;

//Ref/Source: http://msdn.microsoft.com/en-us/magazine/hh335067.aspx
namespace OptimizationToolbox
{
    public class partiswarm : abstractOptMethod
    {
      //fields
		/// <summary>
		/// Gets the neighbor generator.
		/// </summary>
		/// <value>The neighbor generator.</value>
		public abstractGenerator neighborGenerator { get; private set; }
		/// <summary>
		/// Gets the selector.
		/// </summary>
		/// <value>The selector.</value>
		public abstractSelector selector { get; private set; }


		private readonly int numberParticles = 10;
        private readonly int numberIterations = 100;
		private readonly double minX=-25;
		private readonly double maxX=25;
		private double bestGlobalFitness = double.MaxValue;
		private List<double[]> xStars = new List<double[]> ();
		private List<double[]> trialValues = new List<double[]> ();
		private double x_d=0.0;
		private double y_d = 0.0; 
		private List<int> sliderindices = new List<int> ();
		private const double minAlphaStepRatio = 1e-6;
		private double alphaStar;
		private double[] dk;

		/*		 fk is the value of f(xk). */
		private double fk;

        private  int iteration=0;
        
		private static SomeFunctions.Values trial_trial;
		private static SomeFunctions.Values f_val;

        public partiswarm()
        {
            RequiresObjectiveFunction = true;
            ConstraintsSolvedWithPenalties = true;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = false;
			RequiresDiscreteSpaceDescriptor = false;
			RequiresLineSearchMethod = true;
        }

		public partiswarm(int numberParticles, int numberIterations, double maxX, double minX,List<double[]> xStars,List<double[]> trialValues,double bestGlobalFitness=double.MaxValue)
            : this()
        {
            this.numberIterations = numberIterations;
            this.numberParticles = numberParticles;
			this.maxX = maxX;
			this.minX = minX;
			this.bestGlobalFitness = bestGlobalFitness;
			this.xStars = xStars;this.trialValues = trialValues;
        }
		public partiswarm(int numberParticles, int numberIterations, double maxX, double minX,double x_d,double y_d,List<int>sliderindices)
			: this()
		{
			this.numberIterations = numberIterations;
			this.numberParticles = numberParticles;
			this.maxX = maxX;
			this.minX = minX;
			this.x_d = x_d;
			this.y_d = y_d;
			this.sliderindices = sliderindices;
		}


        protected override double run (out double[] xStar)
		{
			Particle[] swarm = new Particle[numberParticles];


			trial_trial = new SomeFunctions.Values (x);
			trial_trial.addtoValues (x);


			double[] bestGlobalPosition = new double[x.Length];
			x.CopyTo (bestGlobalPosition, 0);

			double minV = -1.0 * maxX;
			double maxV = maxX;
			Random ran = new Random ();
			var conjugateDirections = new List<double[]> ();

			for (int ij = 0; ij < n; ij++) {
				var direction = new double[n];
				direction [ij] = 1;
				conjugateDirections.Add (direction);

			}



			for (int i = 0; i < swarm.Length; ++i) { // initialize each Particle in the swarm
				double[] randomPosition = new double[x.Length];
               

				if (xStars.Count > 0) {
					if (i == 0)
						x.CopyTo (randomPosition, 0);
					else {
						for (int j = 0; j < randomPosition.Length; ++j) {

							double lo = minX;
							double hi = maxX;
				
							randomPosition [j] = (hi - lo) * x [j] + lo;
							//randomPosition [j] = x [j]+(hi-lo)*ran.NextDouble();
							randomPosition[j] =(hi - lo) * ran.NextDouble() + lo;
						}
					}

				} else {
					if (i == 0)
						x.CopyTo (randomPosition, 0);
					else {
						for (int j = 0; j < randomPosition.Length; ++j) {

							double lo = minX;
							double hi = maxX;

							//randomPosition [j] = (hi - lo) * x [j] + lo;
							//randomPosition [j] = x [j]+(hi-lo)*ran.NextDouble();
							randomPosition[j] =(hi - lo) * ran.NextDouble() + lo;
						}
					}

				}
				double fitness = calc_f (randomPosition);
				f_val = new SomeFunctions.Values (fitness);
				double[] randomVelocity = new double[x.Length];

				for (int j = 0; j < randomVelocity.Length; ++j) {
					double lo = -1.0 * Math.Abs (maxX - minX);
					double hi = Math.Abs (maxX - minX);
					randomVelocity [j] = (hi - lo) * ran.NextDouble() + lo;
					//randomVelocity [j] = (hi - lo) * x[j] + lo;
				}
				swarm [i] = new Particle (randomPosition, fitness, randomVelocity, randomPosition, fitness);

				// does current Particle have global best position/solution?
				if (swarm [i].fitness < bestGlobalFitness) {
					bestGlobalFitness = swarm [i].fitness;
					swarm [i].position.CopyTo (bestGlobalPosition, 0);
				}
			}

		

			double w = 0.729; // inertia weight. see http://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=00870279
			double c1 = 1.49445; // cognitive/local weight 1.49445
			double c2 = 1.49445; // social/global weight
			double r1, r2; // cognitive and social randomizations
			while (notConverged (iteration, numEvals, bestGlobalFitness, bestGlobalPosition)) {
				//SearchIO.output (iteration);
				++iteration;
				double[] newVelocity = new double[x.Length];
				double[] newPosition = new double[x.Length];
				double newFitness;

				for (int i = 0; i < swarm.Length; ++i) { // each Particle
					Particle currP = swarm [i];

					for (int j = 0; j < currP.velocity.Length; ++j) { // each x value of the velocity
						r1 = ran.NextDouble ();
						r2 = ran.NextDouble ();

						newVelocity [j] = w * (currP.velocity [j]) +
						(c1 * r1 * (currP.bestPosition [j] - currP.position [j])) +
						(c2 * r2 * (bestGlobalPosition [j] - currP.position [j]));

						if (newVelocity [j] < (minV-50.0))
							//newVelocity [j] = minV;
							newVelocity [j] = (maxV-minV)*ran.NextDouble()+minV;
						else if (newVelocity [j] > (maxV+50.0))
							//newVelocity [j] = maxV;
							newVelocity [j] = (maxV-minV)*ran.NextDouble()+minV;
					}

					newVelocity.CopyTo (currP.velocity, 0);

					for (int j = 0; j < currP.position.Length; ++j) {
						newPosition [j] = currP.position [j] + newVelocity [j];
						if (newPosition [j] < (minX - 50.0))
							//newPosition [j] = minX;
							newPosition [j] = (maxX - minX) * ran.NextDouble() + minX;
						else if (newPosition [j] > (maxX+50.0))
							//newPosition [j] = maxX;
							newPosition [j] = (maxX - minX) * ran.NextDouble() + minX;
					}

//					var trial = finddesiredPathrepeats (newPosition, x_d, y_d,sliderindices);
//					trial.CopyTo (newPosition, 0);
					newPosition.CopyTo (currP.position, 0);
					newFitness = calc_f (newPosition);
					f_val.addtoValues (newFitness);
//                    
					currP.fitness = newFitness;
					trial_trial.addtoValues (newPosition);
					if (newFitness < currP.bestFitness) {
						newPosition.CopyTo (currP.bestPosition, 0);
						currP.bestFitness = newFitness;
						//SearchIO.output ("better than best fitness: " + newFitness);	


					}

					if (newFitness < bestGlobalFitness) {
						newPosition.CopyTo (bestGlobalPosition, 0);
						bestGlobalFitness = newFitness;
						//SearchIO.output ("before golden section new fitness obtained : " + newFitness);

					}



					/*if (newFitness > bestGlobalFitness)*/
					else	{
//						var conjugateDirections1 = new List<double[]> ();
//
//						for (int ij = 0; ij < n; ij++) {
//							var direction = new double[n];
//							direction [ij] = 1;
//							conjugateDirections1.Add (direction);
//
//						}
//						var xinner11 = (double[])newPosition.Clone ();
//						for (int ij = 0; ij < n; ij++) {
//
//							var dk1 = conjugateDirections1 [ij];
//							var xinner1 = xinner11;
//							alphaStar = lineSearchMethod.findAlphaStar (xinner1, dk1, true);
//							xinner1 = StarMath.add (xinner1, StarMath.multiply (alphaStar, dk1));
//
//
//
//							//I have the new position, try to update the velocity now now
//
//
//							for (int j = 0; j < newPosition.Length; ++j) {
//								r1 = ran.NextDouble ();
//								r2 = ran.NextDouble ();
//
//								newVelocity [j] = w * (currP.velocity [j]) +
//									(c1 * r1 * (currP.bestPosition [j] - xinner1 [j])) +
//									(c2 * r2 * (bestGlobalPosition [j] - xinner1 [j]));
//
//								if (newVelocity [j] < minV)
//									newVelocity [j] = minV;
//								else if (newVelocity [j] > maxV)
//									newVelocity [j] = maxV;
//							}
//
//
//							var fNew = calc_f (xinner1);
//							trial_trial.addtoValues (newPosition);
//
//							if (fNew < newFitness) {
//								newPosition.CopyTo (currP.position, 0);
//								currP.fitness = fNew;
//
//
//								if (fNew < currP.bestFitness) {
//									xinner1.CopyTo (currP.bestPosition, 0);
//									currP.bestFitness = fNew;
//
//									//SearchIO.output ("Last after golden sectiom new fitness obtained : " + fNew);
//								}
//
//								if (fNew < bestGlobalFitness) {
//									xinner1.CopyTo (bestGlobalPosition, 0);
//									bestGlobalFitness = fNew;
//
//									//SearchIO.output ("Last after golden sectiom new fitness obtained : " + fNew);
//								
//
//								}
//							}
//
//						}

//						var initPoints11 = (double[])currP.position.Clone ();
//						var new_tral11 = xStars;
//						for (int l = 0; l < xStars.Count; l++) {
//							new_tral11 [l] = StarMath.add (initPoints11, xStars [l]);
//						}
////
//						for (int j = 0; j < new_tral11.Count; j++) {
//							//var trial = NelderMead (new_tral1 [j]);
//							var f_star = calc_f (new_tral11 [j]);
//							trial_trial.addtoValues (new_tral11 [j]);
////							new_tral11 [j].CopyTo (currP.position, 0);
////							currP.fitness = f_star;
//							//trial_trial.addtoValues (trial);
//
//							if (f_star < currP.bestFitness) {
//								new_tral11 [j].CopyTo (currP.bestPosition, 0);
//								currP.bestFitness = f_star;
//								new_tral11 [j].CopyTo (currP.position, 0);
//								currP.fitness = f_star;
//								SearchIO.output ("neighbour search new currP best fitness obtained : " + f_star);
////							trial.CopyTo (currP.position, 0);
////							currP.fitness = f_star;
//
//							}
////
//							if (f_star < bestGlobalFitness) {
//								new_tral11 [j].CopyTo (bestGlobalPosition, 0);
//								bestGlobalFitness = f_star;
//								new_tral11 [j].CopyTo (currP.position, 0);
//								currP.fitness = f_star;
//								SearchIO.output ("neighbour search best new global fitness obtained : " + f_star);
////							trial.CopyTo (currP.position, 0);
////							currP.fitness = f_star;
//
//							}
////
////					}
//
//						}


					}
				}
			

				Console.WriteLine ("bestGlobalFitness:" + bestGlobalFitness);
				}
			
	

            

            xStar = bestGlobalPosition;
            fStar = bestGlobalFitness;

            return fStar;
        }

		public double[] finddesiredPathrepeats(double[] vector,double x_d,double y_d,List<int> sliderindices)
		{



			var newPoints = new double[vector.GetLength(0)];

			List<double> newvector = new List<double> ();
			for (int i = 0; i < vector.GetLength (0); i++) {

				bool isInList = sliderindices.IndexOf (i) != -1;
				if (!isInList)
				{
					newvector.Add (vector [i]);


				}


			}
			var vector1 = newvector.ToArray ();
			var numbercoord=(vector1.GetLength(0))/2;

				var k=0;
				var x_coord=new double[numbercoord];
				var y_coord=new double[numbercoord];
				
		

				vector1.CopyTo(newPoints,0);
				Random rna = new Random();

			for(int j=0;j<vector1.GetLength(0);j=j+2)
				{
				x_coord[k]=vector1[j];
				y_coord[k]=vector1[j+1];
					k++;
				}
				k=0;

				//now I have the x-coor and y-coord separately
			for(int j=0;j<x_coord.GetLength(0);j++)
				{
					if(x_coord[j]==x_d)
					{
						if(y_coord[j]==y_d)
						{
							//then we have to form a different set of numbers 
						newPoints[j*2]=(maxX-minX)*rna.NextDouble()+minX;
						newPoints[j*2+1]=(maxX-minX)*rna.NextDouble()+minX;
						k = 1;

						}

					}

				}

			if (k == 1) {

				//have to copy to the original format
				int p = 0;
				List<double> newVector = new List<double> ();
				for (int i = 0; i < vector.GetLength (0); i++) {
					bool isInList = sliderindices.IndexOf (i) != -1;
					if (!isInList) {
						newVector.Add (newPoints [p++]);


					} else {

						newVector.Add (vector [sliderindices [i]]);

					}



				}

				var trialarray = newVector.ToArray ();
				return trialarray;




			} else
				return vector;




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
