using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationToolbox
{
	public class NelderMead_Alt: abstractOptMethod
	{
		private readonly int dim = 2;  // problem dimension (number of variables to solve for)
		private readonly int amoebaSize = 3;  // number of potential solutions in the amoeba
		private readonly double minX = -10.0;
		private readonly double maxX = 10.0;
		private readonly int maxLoop = 50;
		private readonly SortedList<double, double[]> vertices = 
			new SortedList<double, double[]>(new optimizeSort(optimize.minimize));

		private static Random random = new Random(1); 
		private Solution[] solutions;  // potential solutions (vector + value)


		private  double alpha;  // reflection
		private double beta;   // contraction
		private double gamma;  // expansion


		public NelderMead_Alt (int amoebaSize, int dim, double minX, double maxX, int maxLoop)
		{
			this.amoebaSize = amoebaSize;
			this.dim = dim;
			this.minX = minX;
			this.maxX = maxX;
			this.alpha = 1.0;  // hard-coded values from theory
			this.beta = 0.5;
			this.gamma = 2.0;

			this.maxLoop = maxLoop;

//			this.solutions = new Solution[amoebaSize];
//			for (int i = 0; i < solutions.Length; ++i) {
//				solutions [i] = new Solution (dim, minX, maxX);  // the Solution ctor calls the objective function to compute value
//				solutions [i].value = calc_f (solutions [i].vector);
//				vertices.Add (solutions [i].value, solutions [i].vector);//adding those three into vertices - autosort
//			}

			//Array.Sort(solutions);




		}

		public NelderMead_Alt()
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

		public double[] generaterandomvalues (int dim, double minX, double maxX)
		{

			double[] x_st = new double[dim];

						for (int i = 0; i < dim; ++i)

				x_st[i] = (maxX - minX) * random.NextDouble() + minX;


			return x_st;

		}

		protected override double run(out double[] xStar)
		{

			double[] f_values = new double[amoebaSize];
			List<double[]> x_values = new List<double[]>();


			for (int i = 0; i < amoebaSize; i++) {
				x_values.Add(generaterandomvalues(dim,minX,maxX));
				f_values [i] = calc_f (x_values [i]);
				vertices.Add (f_values[i], x_values[i]);

			}

//			List<double> fstarX = f_values.ToList();
//			var sorted = fstarX
//				.Select((g, i) => new KeyValuePair<double, int>(g, i))
//				.OrderBy(g => g.Key)
//				.ToList();
//			List<double> f_star = sorted.Select(g => g.Key).ToList();
//			List<int> f_index = sorted.Select(g => g.Value).ToList();

			for (int i = 0; i < amoebaSize; i++) {
				x_values [i] = vertices.Values [i];
				f_values [i] = vertices.Keys [i];

			}





			int t = 0;  // loop counter
			while (t < maxLoop)
			{
				++t;

//				if (t % 10 == 0)
//				{
//					Console.WriteLine("At t = " + t + " curr best solution = " + this.solutions[0]); 
//				}

				//Solution centroid = Centroid();  // compute centroid

				#region compute centroid
				// return the centroid of all solution vectors except for the worst (highest index) vector
				double[] c = new double[dim];
				for (int i = 0; i < amoebaSize - 1; ++i)
					for (int j = 0; j < dim; ++j)
						c[j] += x_values[i][j];  // accumulate sum of each vector component

				for (int j = 0; j < dim; ++j)
					c[j] = c[j] / (amoebaSize - 1);

				double centroid = calc_f(c);

				#endregion 

				//Solution reflected = Reflected(centroid);  // compute reflected
				#region compute reflected

				// the reflected solution extends from the worst (lowest index) solution through the centroid
				double[] r = new double[dim];
				double[] worst = x_values[amoebaSize - 1];  // convenience only
				for (int j = 0; j < dim; ++j)
					r[j] = ((1 + alpha) * c[j]) - (alpha * worst[j]);
				double reflected = calc_f(r);

				#endregion 

				


				if (reflected < vertices.Keys[0])  // reflected is better than the curr best
				{
					//Solution expanded = Expanded(reflected, centroid);  // can we do even better??
					#region expanded
					double[] e = new double[dim];
					for (int j = 0; j < dim; ++j)
						e[j] = (gamma * r[j]) + ((1 - gamma) * c[j]);

					double expanded = calc_f(e);


					#endregion 

					#region if expanded is better or not
					if (expanded < vertices.Keys [0]) {  // winner! expanded is better than curr best
						//ReplaceWorst (expanded);  // replace curr worst solution with expanded
						for (int j = 0; j < dim; ++j)
							x_values[amoebaSize-1][j] = e[j];
						f_values[amoebaSize-1] = expanded;
						vertices.RemoveAt(amoebaSize-1);
						vertices.Add(expanded,e);

						//Array.Sort(solutions);
						for (int i = 0; i < amoebaSize; i++) {
							x_values [i] = vertices.Values [i];
							f_values [i] = vertices.Keys [i];

						}


					} else {
						//ReplaceWorst (reflected);  // it was worth a try . . .
						for (int j = 0; j < dim; ++j)
							x_values[amoebaSize-1][j] = r[j];
						f_values[amoebaSize-1] = reflected;
//						vertices.Values [amoebaSize-1] = r;
//						vertices.Keys [amoebaSize-1] = reflected;
						vertices.RemoveAt(amoebaSize-1);
						vertices.Add(reflected,r);
						//Array.Sort(solutions);
						for (int i = 0; i < amoebaSize; i++) {
							x_values [i] = vertices.Values [i];
							f_values [i] = vertices.Keys [i];

						}



					}
					#endregion
					continue;
				}

				if (IsWorseThanAllButWorst(reflected) == true)  // reflected is worse (larger value) than all solution vectors (except possibly the worst one)
				{
					if (reflected <= f_values [amoebaSize - 1]) {// reflected is better (smaller) than the curr worst (last index) vector
						//ReplaceWorst (reflected);
						for (int j = 0; j < dim; ++j)
							x_values[amoebaSize-1][j] = r[j];
						f_values[amoebaSize-1] = reflected;
//						vertices.Values [amoebaSize-1] = r;
//						vertices.Keys [amoebaSize-1] = reflected;
						vertices.RemoveAt(amoebaSize-1);
						vertices.Add(reflected,r);
						//Array.Sort(solutions);
						for (int i = 0; i < amoebaSize; i++) {
							x_values [i] = vertices.Values [i];
							f_values [i] = vertices.Keys [i];

						}

					}

					//Solution contracted = Contracted(centroid);  // compute a point 'inside' the amoeba

					#region contracted
					// contracted extends from worst (lowest index) towards centoid, but not past centroid
					double[] v = new double[dim];  // didn't want to reuse 'c' from centoid routine
					double[] worst1 = x_values[amoebaSize - 1];  // convenience only
					for (int j = 0; j < dim; ++j)
						v[j] = (beta * worst1[j]) + ((1 - beta) * c[j]);

					double contracted = calc_f(v);

					#endregion

					if (contracted > f_values [amoebaSize - 1]) {  // contracted is worse (larger value) than curr worst (last index) solution vector
						//Shrink ();
						for (int i = 1; i < amoebaSize; ++i) {  // note we don't start at [0]
							for (int j = 0; j < dim; ++j) {
								x_values [i] [j] = (x_values [i] [j] + x_values [0] [j]) / 2.0;
								f_values[i] = calc_f(x_values[i]);
								vertices.RemoveAt(i);
								vertices.Add(f_values[i],x_values[i]);
							}
							//f_values [i] = calc_f (x_values [i]);
//							vertices.Keys [i] = solutions [i].value;
//							vertices.Values [i] = solutions [i].vector;
//							vertices.RemoveAt(i);
//							vertices.Add(f_values[i],x_values[i]);
						}

						for (int i = 0; i < amoebaSize; i++) {
							x_values [i] = vertices.Values [i];
							f_values [i] = vertices.Keys [i];

						}


					} 
					else {
						//ReplaceWorst (contracted);

						for (int j = 0; j < dim; ++j)
							x_values[amoebaSize-1][j] = v[j];
						f_values[amoebaSize-1] = contracted;
//						vertices.Values [amoebaSize-1] = v;
//						vertices.Keys [amoebaSize-1] = contracted;
						vertices.RemoveAt(amoebaSize-1);
						vertices.Add(contracted,v);
						//Array.Sort(solutions);
						for (int i = 0; i < amoebaSize; i++) {
							x_values [i] = vertices.Values [i];
							f_values [i] = vertices.Keys [i];

						}

					}

					continue;
				}

				//ReplaceWorst(reflected);

				for (int j = 0; j < dim; ++j)
					x_values[amoebaSize-1][j] = r[j];
				f_values[amoebaSize-1] = reflected;
//				vertices.Values [amoebaSize-1] = r;
//				vertices.Keys [amoebaSize-1] = reflected;
				vertices.RemoveAt(amoebaSize-1);
				vertices.Add(reflected,r);
				//Array.Sort(solutions);
				for (int i = 0; i < amoebaSize; i++) {
					x_values [i] = vertices.Values [i];
					f_values [i] = vertices.Keys [i];

				}

				//if (IsSorted() == false)
				//  throw new Exception("Unsorted at k = " + k);

				Console.WriteLine ("current best fstar: " + vertices.Keys [0]);

			}  // solve loop

			xStar = vertices.Values[0];
			return vertices.Keys [0];

		}

		public Solution Centroid()
		{
			// return the centroid of all solution vectors except for the worst (highest index) vector
			double[] c = new double[dim];
			for (int i = 0; i < amoebaSize - 1; ++i)
				for (int j = 0; j < dim; ++j)
					c[j] += solutions[i].vector[j];  // accumulate sum of each vector component

			for (int j = 0; j < dim; ++j)
				c[j] = c[j] / (amoebaSize - 1);

			Solution s = new Solution(c);// feed vector to ctor which calls objective function to compute value
			s.value = calc_f (s.vector);
			//vertices.Add (s.value, s.vector);
			return s;
		}

		public Solution Reflected(Solution centroid)
		{
			// the reflected solution extends from the worst (lowest index) solution through the centroid
			double[] r = new double[dim];
			double[] worst = this.solutions[amoebaSize - 1].vector;  // convenience only
			for (int j = 0; j < dim; ++j)
				r[j] = ((1 + alpha) * centroid.vector[j]) - (alpha * worst[j]);
			Solution s = new Solution(r);
			s.value = calc_f (s.vector);
			//vertices.Add (s.value, s.vector);
			return s;
		}

		public Solution Expanded(Solution reflected, Solution centroid)
		{
			// expanded extends even more, from centroid, thru reflected
			double[] e = new double[dim];
			for (int j = 0; j < dim; ++j)
				e[j] = (gamma * reflected.vector[j]) + ((1 - gamma) * centroid.vector[j]);
			Solution s = new Solution(e);
			s.value = calc_f (s.vector);
			//vertices.Add (s.value, s.vector);
			return s;
		}

		public Solution Contracted(Solution centroid)
		{
			// contracted extends from worst (lowest index) towards centoid, but not past centroid
			double[] v = new double[dim];  // didn't want to reuse 'c' from centoid routine
			double[] worst = this.solutions[amoebaSize - 1].vector;  // convenience only
			for (int j = 0; j < dim; ++j)
				v[j] = (beta * worst[j]) + ((1 - beta) * centroid.vector[j]);
			Solution s = new Solution(v);
			s.value = calc_f (s.vector);
			//vertices.Add (s.value, s.vector);
			return s;
		}

		public void Shrink()
		{
			// move all vectors, except for the best vector (at index 0), halfway to the best vector
			// compute new objective function values and sort result
			for (int i = 1; i < amoebaSize; ++i)  // note we don't start at [0]
			{
				for (int j = 0; j < dim; ++j)
				{
					solutions[i].vector[j] = (solutions[i].vector[j] + solutions[0].vector[j]) / 2.0;
					solutions[i].value = calc_f(solutions[i].vector);

				}

				vertices.Keys [i] = solutions [i].value;
				vertices.Values [i] = solutions [i].vector;
			}
			//Array.Sort(solutions);

			//sort the solutions with respect to vertices
			double[] f_values = new double[amoebaSize];
			List<double[]> x_values = new List<double[]>();

			for (int i = 0; i < solutions.Length; i++) {
				f_values[i] = solutions [i].value;
				x_values.Add (solutions [i].vector);

			}

			for (int j = 0; j < solutions.Length; j++) {
				for (int i = 0; i < solutions.Length; i++) {
					if (vertices.Keys [j] == f_values [i]) {
						solutions [i].value = vertices.Keys [j];
						solutions [i].vector = vertices.Values [j];
					}
				}
			}

		}

		public void ReplaceWorst(Solution newSolution)
		{
			// replace the worst solution (at index size-1) with contents of parameter newSolution's vector
			for (int j = 0; j < dim; ++j)
				solutions[amoebaSize-1].vector[j] = newSolution.vector[j];
			solutions[amoebaSize - 1].value = newSolution.value;
			vertices.Add (solutions[amoebaSize-1].value, solutions[amoebaSize-1].vector);
			//Array.Sort(solutions);

			//sort the solutions with respect to vertices
			double[] f_values = new double[amoebaSize];
			List<double[]> x_values = new List<double[]>();

			for (int i = 0; i < solutions.Length; i++) {
				f_values[i] = solutions [i].value;
				x_values.Add (solutions [i].vector);

			}

			for (int j = 0; j < solutions.Length; j++) {
				for (int i = 0; i < solutions.Length; i++) {
					if (vertices.Keys [j] == f_values [i]) {
						solutions [i].value = vertices.Keys [j];
						solutions [i].vector = vertices.Values [j];
					}
				}
			}
		}

		public bool IsWorseThanAllButWorst(double reflected)
		{
			// Solve needs to know if the reflected vector is worse (greater value) than every vector in the amoeba, except for the worst vector (highest index)
			for (int i = 0; i < amoebaSize - 1; ++i)  // not the highest index (worst)
			{
				if (reflected <= vertices.Keys[i])  // reflected is better (smaller value) than at least one of the non-worst solution vectors
					return false;
			}
			return true;
		}


	}
}

