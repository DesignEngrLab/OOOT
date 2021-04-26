// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 04-21-2021
// ***********************************************************************
// <copyright file="PatternSearch.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class PatternSearch.
    /// Implements the <see cref="OptimizationToolbox.abstractOptMethod" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.abstractOptMethod" />
    public class PatternSearch:abstractOptMethod
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternSearch"/> class.
        /// </summary>
        public PatternSearch ()
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
        /// <summary>
        /// The maximum
        /// </summary>
        private readonly double max;
        /// <summary>
        /// The minimum
        /// </summary>
        private readonly double min;
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternSearch"/> class.
        /// </summary>
        /// <param name="max">The maximum.</param>
        /// <param name="min">The minimum.</param>
        public PatternSearch(double max, double min)
			: this()
		{
			this.max = max;
			this.min = min;
		}

        /// <summary>
        /// Runs the specified optimization method. This includes the details
        /// of the optimization method.
        /// </summary>
        /// <param name="xStar">The x star.</param>
        /// <returns>System.Double.</returns>
        protected override double run (out double[] xStar)
		{
			var xCopy = x;
			var meshSize = max-min;
			var iterations = 0;
			var stepsize = 10.0;
			var alpha = 2.0;
			var GlobalXstar = new double[n];
			while (iterations < 5) {
				iterations++;
				SomeFunctions.MyClass trialneighbors = new SomeFunctions.MyClass ();
				var neighbors = trialneighbors.generateNeighbors (xCopy, meshSize,stepsize);
				double minFstar_orig = calc_f (xCopy);
				double minFstar = minFstar_orig;
				int indexVal = 0;
				for (int i = 0; i < neighbors.Count; i++) {
					var fstar = calc_f (neighbors [i]);
					if (fstar <= minFstar) {
						minFstar = fstar;
						indexVal = i;
						SearchIO.output ("better obtained: " + fstar);
					}
				}

				SearchIO.output ("iterations:" + iterations);
				if (minFstar < minFstar_orig) {
					var xCopy1 = neighbors [indexVal];
					var xCopy2 = StarMath.add (xCopy1, StarMath.multiply (alpha, StarMath.subtract (xCopy1,xCopy)));
					var fStar = calc_f (xCopy2);
					if (fStar <= minFstar) {
						xCopy2.CopyTo (xCopy, 0);
						xCopy2.CopyTo (GlobalXstar, 0);
					} else {
						xCopy1.CopyTo(xCopy,0);
						xCopy1.CopyTo(GlobalXstar, 0);
					}
					meshSize = meshSize * 2;

				} else {
					meshSize = meshSize / 2;
					stepsize = stepsize*0.9;
				}


			}

			xStar = new double[8];
			xStar = GlobalXstar;
			return calc_f(xStar);



		}
	}
}

