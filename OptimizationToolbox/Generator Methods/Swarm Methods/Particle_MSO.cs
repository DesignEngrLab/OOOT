// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="Particle_MSO.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class Particle_MSO.
    /// </summary>
    public class Particle_MSO
	{
        /// <summary>
        /// The ran
        /// </summary>
        static Random ran = new Random(0);
        /// <summary>
        /// The position
        /// </summary>
        public double[] position;
        /// <summary>
        /// The velocity
        /// </summary>
        public double[] velocity;
        /// <summary>
        /// The cost
        /// </summary>
        public double cost;
        /// <summary>
        /// The best part position
        /// </summary>
        public double[] bestPartPos;
        /// <summary>
        /// The best part cost
        /// </summary>
        public double bestPartCost;
        /// <summary>
        /// Initializes a new instance of the <see cref="Particle_MSO"/> class.
        /// </summary>
        /// <param name="initPoints">The initialize points.</param>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="Particle_MSO"/> class.
        /// </summary>
        /// <param name="dim">The dim.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
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

