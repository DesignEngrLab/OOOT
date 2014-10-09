using System;
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
	public class MultiSwarm: abstractOptMethod
	{

		private readonly double max=25.0;
		private readonly double min=-25.0;
		private readonly int numParticles = 10;
		private readonly int numSwarms = 20;
		private readonly int maxLoop = 150;
		private readonly int dim = 8;
		private readonly Random ran = new Random(0);
		private readonly double minx=0.0;
		private readonly double miny=0.0;
		private readonly double maxx=0.0;
		private readonly double maxy=0.0;
		public MultiSwarm ()
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

		public MultiSwarm(int numberParticles, int numberIterations, double maxX, double minX,int dimension,double maxx,double maxy,
			double minx,double miny)
			: this()
		{
			this.maxLoop = numberIterations;
			this.numParticles = numberParticles;
			this.max = maxX;
			this.min = minX;
			this.dim = dimension;
			this.maxx = maxx;
			this.minx = minx;
			this.maxy = maxy;
			this.miny = miny;
		}




		protected override double run (out double[] xStar)
		{
			double[] bestMultiPos = new double[dim];
			double bestMultiCost = double.MaxValue;
			Swarm[] swarms = new Swarm[numSwarms];
			List<double> minValues = new List<double> ();
			List<double> maxValues = new List<double> ();
//			for (int i = 0; i < numSwarms; i++) {
//				if (i < 4) {
//					minValues.Add (minx-50.0);
//					maxValues.Add (minx);
//				}
//				if (i>3  && i<8) {
//					minValues.Add (miny-50.0);
//					maxValues.Add (miny);
//				}
//				if (i>7  && i<12) {
//					minValues.Add (maxx);
//					maxValues.Add (maxx+50.0);
//				}
//				if (i>11  && i<16) {
//					minValues.Add (maxy);
//					maxValues.Add (maxy+50.0);
//				}
//				if (i>15  && i<numSwarms) {
//					minValues.Add (min);
//					maxValues.Add (max);
//				}
//			}



			for (int i = 0; i < numSwarms; ++i)
			{
				swarms [i] = new Swarm (numParticles, dim, min,max);
				//now compute the costs
				for (int j = 0; j < swarms [i].particles.Length; ++j) {

					//swarms [i].particles [j] = new Particle_MSO (dim, min, max);
					swarms[i].particles[j].cost=calc_f (swarms [i].particles [j].position);
					swarms [i].particles [j].bestPartCost = swarms [i].particles [j].cost;
					//	Array.Copy (swarms [i].particles [j].position, swarms [i].particles [j].bestPartPos, dim);
					swarms [i].particles [j].bestPartPos = swarms [i].particles [j].position;
					if (swarms[i].particles[j].cost < swarms[i].bestSwarmCost)
										{
						swarms[i].bestSwarmCost = swarms[i].particles[j].cost;
						//Array.Copy(swarms[i].particles[j].position, swarms[i].bestSwarmPos, dim);
						swarms [i].bestSwarmPos = swarms [i].particles [j].position;
										}

				}

				if (swarms[i].bestSwarmCost < bestMultiCost)
				{
					bestMultiCost = swarms[i].bestSwarmCost;
					//Array.Copy(swarms[i].bestSwarmPos, bestMultiPos, dim);
					bestMultiPos = swarms [i].bestSwarmPos;
					bestMultiPos = swarms [i].bestSwarmPos;
				}
			}

			int ct = 0;
			double w = 0.729; // inertia
			double c1 = 1.49445; // particle / cogntive 1.49445
			double c2 = 1.49445; // swarm / social
			double c3 = 0.3645; // multiswarm / global 0.3645
			double death = 0.05; ; // prob of particle death
			double immigrate = 0.05;  // prob of particle immigration

			while (ct < maxLoop)
			{
				++ct;
				for (int i = 0; i < swarms.Length; ++i) // each swarm
				{
					for (int j = 0; j < swarms[i].particles.Length; ++j) // each particle
					{
						double p = ran.NextDouble();
						if (p < death)
						{
							swarms[i].particles[j] = new Particle_MSO(dim, min, max);
							swarms[i].particles[j].cost=calc_f (swarms [i].particles [j].position);
							swarms [i].particles [j].bestPartCost = swarms [i].particles [j].cost;
							//Array.Copy (swarms [i].particles [j].position, swarms [i].particles [j].bestPartPos, dim);
							swarms [i].particles [j].bestPartPos = swarms [i].particles [j].position;
						}

						double q = ran.NextDouble();
						if (q < immigrate)
						{
//						Immigration(i, j); // swap curr particle with a random particle in diff swarm
							int otheri = ran.Next(0, swarms.Length);
							int otherj = ran.Next(0, swarms[0].particles.Length);
							Particle_MSO tmp = swarms[i].particles[j];
							swarms[i].particles[j] = swarms[otheri].particles[otherj];
							swarms[otheri].particles[otherj] = tmp;

						}

						for (int k = 0; k < dim; ++k) // update velocity. each x position component
						{
							double r1 = ran.NextDouble();
							double r2 = ran.NextDouble();
							double r3 = ran.NextDouble();

							swarms [i].particles [j].velocity [k] = w * (swarms [i].particles [j].velocity [k]) +
							(c1 * r1 * (swarms [i].particles [j].bestPartPos [k] - swarms [i].particles [j].position [k])) +
							(c2 * r2 * (swarms [i].bestSwarmPos [k] - swarms [i].particles [j].position [k]))  +
								(c3 * r3 * (bestMultiPos[k] - swarms[i].particles[j].position[k]));

//							if (swarms[i].particles[j].velocity[k] < min)
//								swarms[i].particles[j].velocity[k] = min;
//							else if (swarms[i].particles[j].velocity[k] >max)
//								swarms[i].particles[j].velocity[k] = max;

						}

						for (int k1 = 0; k1 < dim; ++k1) // update position
						{
							swarms[i].particles[j].position[k1] += swarms[i].particles[j].velocity[k1];
						}

						// update cost
						swarms[i].particles[j].cost = calc_f(swarms[i].particles[j].position);

						// check if new best cost
						if (swarms[i].particles[j].cost < swarms[i].particles[j].bestPartCost)
						{
							swarms[i].particles[j].bestPartCost = swarms[i].particles[j].cost;
							//Array.Copy(swarms[i].particles[j].position, swarms[i].particles[j].bestPartPos, dim);
							swarms [i].particles [j].bestPartPos = swarms [i].particles [j].position;
							//Console.WriteLine ("bestPartCost : " + swarms[i].particles[j].bestPartCost);
						}

						if (swarms[i].particles[j].cost < swarms[i].bestSwarmCost)
						{
							swarms[i].bestSwarmCost = swarms[i].particles[j].cost;
							Array.Copy(swarms[i].particles[j].position, swarms[i].bestSwarmPos, dim);
							swarms [i].bestSwarmPos = swarms [i].particles [j].position;
							//Console.WriteLine ("bestSwarmCost : " + swarms[i].bestSwarmCost);
						}

						if (swarms[i].particles[j].cost < bestMultiCost)
						{
							bestMultiCost = swarms[i].particles[j].cost;
							//Array.Copy(swarms[i].particles[j].position, bestMultiPos, dim);
							bestMultiPos = swarms [i].particles [j].position;
							//Console.WriteLine ("bestMultiCost : " + bestMultiCost);
						}
//						else
//						{
//							var conjugateDirections1 = new List<double[]> ();
//
//							for (int ij = 0; ij < n; ij++) {
//								var direction = new double[n];
//								direction [ij] = 1;
//								conjugateDirections1.Add (direction);
//
//							}
//							var xinner11 = (double[])swarms[i].particles[j].position;
//							for (int ij = 0; ij < n; ij++) {
//
//								var dk1 = conjugateDirections1 [ij];
//								var xinner1 = xinner11;
//								var alphaStar = lineSearchMethod.findAlphaStar (xinner1, dk1, true);
//								xinner1 = StarMath.add (xinner1, StarMath.multiply (alphaStar, dk1));
//
//								var fNew = calc_f (xinner1);
//
////								xinner1.CopyTo (swarms[i].particles[j].position, 0);
////								swarms[i].particles[j].cost = fNew;
//
//								if (fNew < swarms[i].particles[j].bestPartCost){
//									xinner1.CopyTo (swarms [i].particles [j].bestPartPos, 0);
//									swarms [i].particles [j].bestPartCost = fNew;
//									SearchIO.output ("Last after golden sectiom new fitness obtained : " + fNew);
//								}
//
//								if (fNew < swarms[i].bestSwarmCost) {
//									xinner1.CopyTo (swarms[i].bestSwarmPos, 0);
//									swarms[i].bestSwarmCost = fNew;
//									SearchIO.output ("Last after golden sectiom new fitness obtained : " + fNew);
//								}
//
//								if (fNew < bestMultiCost) {
//									xinner1.CopyTo (bestMultiPos, 0);
//									bestMultiCost = fNew;
//									SearchIO.output ("Last after golden sectiom new fitness obtained : " + fNew);
//
//
//								}
//
//							}
//
//
//
//
//
//						}
					}
				}
				Console.WriteLine ("bestMultiCost : " + bestMultiCost);

			}

			xStar = bestMultiPos;
			return bestMultiCost;

		}
	}
}

