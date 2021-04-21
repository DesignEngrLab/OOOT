using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
	public class PatternSearch:abstractOptMethod
	{
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
		private readonly double max;
		private readonly double min;
		public PatternSearch(double max, double min)
			: this()
		{
			this.max = max;
			this.min = min;
		}

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

