using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
	public class Swarm
	{
		public Particle_MSO[] particles;
		internal double[] bestSwarmPos{ get; set; }
		internal double bestSwarmCost{ get; set; }

		public Swarm (int numParticles, int dim, double min, double max)
		{
			this.bestSwarmCost = double.MaxValue;
			this.bestSwarmPos = new double[dim];
			this.particles = new Particle_MSO[numParticles];


			for (int i = 0; i < particles.Length; ++i)
			{
				this.particles[i] = new Particle_MSO(dim,min,max);
//				if (particles[i].cost < bestSwarmCost)
//				{
//					bestSwarmCost = particles[i].cost;
//					Array.Copy(particles[i].position, bestSwarmPos, dim);
//				}
			}
		}
	}
}

