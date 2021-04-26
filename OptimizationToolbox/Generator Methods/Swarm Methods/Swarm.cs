// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="Swarm.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class Swarm.
    /// </summary>
    public class Swarm
	{
        /// <summary>
        /// The particles
        /// </summary>
        public Particle_MSO[] particles;
        /// <summary>
        /// Gets or sets the best swarm position.
        /// </summary>
        /// <value>The best swarm position.</value>
        internal double[] bestSwarmPos{ get; set; }
        /// <summary>
        /// Gets or sets the best swarm cost.
        /// </summary>
        /// <value>The best swarm cost.</value>
        internal double bestSwarmCost{ get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Swarm"/> class.
        /// </summary>
        /// <param name="numParticles">The number particles.</param>
        /// <param name="dim">The dim.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
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

