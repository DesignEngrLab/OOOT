using System;
using System.Linq;

namespace OptimizationToolbox
{
	public class Particle_MSO
	{
		static Random ran = new Random(0);
		public double[] position;
		public double[] velocity;
		public double cost;
		public  double[] bestPartPos;
		public  double bestPartCost;
		public Particle_MSO (double[] initPoints)
		{
			this.position = new double[initPoints.GetLength(0)];
			this.velocity = new double[initPoints.GetLength(0)];

			var maxX = initPoints.Max ();
			var minX = initPoints.Min ();

			//ran.NextDouble()
			for (int i = 0; i < initPoints.GetLength(0); ++i)
			{
				this.position[i] = initPoints[i];
				this.velocity[i] = ran.NextDouble() * ran.NextDouble()+ ran.NextDouble();
			}


		}
		public Particle_MSO (int  dim, double min, double max)
		{
			this.position = new double[dim];
			this.velocity = new double[dim];

		

			//ran.NextDouble()
			for (int i = 0; i < dim; ++i)
			{
				this.position[i] = (max - min) *ran.NextDouble()+min;
				this.velocity[i] = (max - min) *ran.NextDouble()+min;
			}


		}
	}
}

