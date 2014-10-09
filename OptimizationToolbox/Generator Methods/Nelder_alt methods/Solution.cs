using System;

namespace OptimizationToolbox
{
	public class Solution/*: IComparable<Solution>*/
	{
		public double[] vector;
		public double value;

		static Random random = new Random(1);  // to allow creation of random solutions
		public Solution(int dim, double minX, double maxX)
		{
			// a random Solution
			this.vector = new double[dim];
			for (int i = 0; i < dim; ++i)
				this.vector[i] = (maxX - minX) * random.NextDouble() + minX;
			//this.value = AmoebaProgram.ObjectiveFunction(this.vector, null);
		}


		public Solution(double[] vector)
		{
			// a specifiede solution
			this.vector = new double[vector.Length];
			Array.Copy(vector, this.vector, vector.Length);
			//this.value = AmoebaProgram.ObjectiveFunction(this.vector, null);
		}

//		public int CompareTo(Solution other) // based on vector/solution value
//		{
//			if (this.value < other.value)
//				return -1;
//			else if (this.value > other.value)
//				return 1;
//			else
//				return 0;
//		}
	}
}

