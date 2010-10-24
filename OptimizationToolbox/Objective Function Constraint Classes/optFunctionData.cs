using System;
using System.Collections;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    internal class optFunctionData
    {
        internal Dictionary<double[], double> oldEvaluations;
        internal long numEvals { get; set; }
        internal double finiteDiffStepSize { get; set; }
        internal differentiate findDerivBy { get; set; }

        internal optFunctionData(IOptFunction function, IEqualityComparer<double[]> comparer,
            double finiteDiffStepSize, differentiate findDerivBy)
        {
            if (typeof(IDifferentiable).IsInstanceOfType(function))
                this.findDerivBy = differentiate.Analytic;
            else
            {
                this.finiteDiffStepSize = finiteDiffStepSize;
                this.findDerivBy = findDerivBy;
            }
            oldEvaluations = new Dictionary<double[], double>(comparer);
        }
    }


    class sameCandidate : IEqualityComparer<double[]>
    {
        private readonly double toleranceSquared;
        public sameCandidate(double tolerance)
        {
            this.toleranceSquared = tolerance * tolerance;
        }

        #region Implementation of IEqualityComparer<double[]>

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.
        ///                 </param><param name="y">The second object of type <paramref name="T"/> to compare.
        ///                 </param>
        public bool Equals(double[] x, double[] y)
        {
            if ((x == null) || (y == null) || (x.GetLength(0) != y.GetLength(0))) return false;
            if (x.Equals(y)) return true;
            return (StarMath.norm2(x, y, true) <= toleranceSquared);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.
        ///                 </param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        ///                 </exception>
        public int GetHashCode(double[] obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}
