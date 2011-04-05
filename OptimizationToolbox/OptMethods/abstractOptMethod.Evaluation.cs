using System;
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    public abstract partial class abstractOptMethod
    {

        /// <summary>
        /// Gets or sets the number of active constraints.
        /// </summary>
        /// <value>The number of active constraints, m.</value>
        public int m { get; set; }

        /// <summary>
        /// Gets or sets the number of equality constraints.
        /// </summary>
        /// <value>The number of equality constraints, p.</value>
        public int p { get; set; }

        /// <summary>
        /// Gets or sets the number of inequality constraints.
        /// </summary>
        /// <value>The number of inequality constraints, q.</value>
        public int q { get; set; }

        /// <summary>
        /// Gets the number of function evaluations. This is actually the max of
        /// all functions (objective functions, equalities and inequalities) from
        /// the optimization run.
        /// </summary>
        /// <value>The num evals.</value>
        public long numEvals
        {
            get
            {
                var numEvalList = new List<long>(functionData.Values.Select(a => a.numEvals));
                return numEvalList.Max();
            }
        }
        public List<IObjectiveFunction> f { get; private set; }
        public List<IEquality> h { get; private set; }
        public List<IInequality> g { get; private set; }
        internal List<IConstraint> active { get; private set; }
        private readonly Dictionary<IOptFunction, optFunctionData> functionData;
        private sameCandidate sameCandComparer = new sameCandidate(sameTolerance);

        internal IDependentAnalysis dependentAnalysis { get; private set; }
        private double[] lastDependentAnalysis;


        private void calc_dependent_Analysis(double[] point)
        {
            if (dependentAnalysis == null) return;
            if (sameCandComparer.Equals(point, lastDependentAnalysis)) return;
            dependentAnalysis.calculate(point);
            lastDependentAnalysis = point;
        }



        /// <summary>
        /// Resets the function evaluation database.
        /// </summary>
        public void ResetFunctionEvaluationDatabase()
        {
            foreach (var fd in functionData)
                fd.Value.Clear();
        }


        #region Calculate f, g, h helper functions
        internal double calculate(IOptFunction function, double[] point)
        {
            double fValue;
            double[] pointClone = (double[])point.Clone();
            if (functionData[function].TryGetValue(pointClone, out fValue))
                return fValue;

            calc_dependent_Analysis(point);
            /**************************************************/
            /*** This is the only function that should call ***/
            /**********IOptFunction.calculate(x)***************/
            fValue = function.calculate(point);
            /**************************************************/
            functionData[function].Add(pointClone, fValue);
            functionData[function].numEvals++;
            return fValue;
        }


        // the reason this function is public but the remainder are not, is because
        // this is called from other classes. Most notably, the line search methods, 
        // and the initial sampling in SA to get a temperature.
        /// <summary>
        /// Calculates the value of f at the specified point (assuming single-objective).
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="includeMeritPenalty">if set to <c>true</c> [include merit penalty].</param>
        /// <returns></returns>
        public double calc_f(double[] point, Boolean includeMeritPenalty = false)
        {
            var penalty = ((g.Count + h.Count > 0) && (ConstraintsSolvedWithPenalties || includeMeritPenalty))
                ? meritFunction.calcPenalty(point) : 0.0;
            return calculate(f[0], point) + penalty;
        }


        /// <summary>
        /// Calculates the f vector (multi-objective) at the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="includeMeritPenalty">if set to <c>true</c> [include merit penalty].</param>
        /// <returns></returns>
        public double[] calc_f_vector(double[] point, Boolean includeMeritPenalty = false)
        { return f.Select(fi => calculate(fi, point)).ToArray(); }
        /// <summary>
        /// Calculates the h vector at the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        protected double[] calc_h_vector(double[] point)
        { return h.Select(h0 => calculate(h0, point)).ToArray(); }
        /// <summary>
        /// Calculates the g vector at the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        protected double[] calc_g_vector(double[] point)
        { return g.Select(g0 => calculate(g0, point)).ToArray(); }
        /// <summary>
        /// Calculates the active vector at the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        protected double[] calc_active_vector(double[] point)
        { return active.Select(a => calculate(a, point)).ToArray(); }

        /// <summary>
        /// Calculates the gradient of f vector at the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="includeMeritPenalty">if set to <c>true</c> [include merit penalty].</param>
        /// <returns></returns>
        protected double[] calc_f_gradient(double[] point, Boolean includeMeritPenalty = false)
        {
            var grad = new double[n];
            for (var i = 0; i != n; i++)
                grad[i] = deriv_wrt_xi(f[0], point, i);
            if (ConstraintsSolvedWithPenalties || includeMeritPenalty)
                return StarMath.add(grad, meritFunction.calcGradientOfPenalty(point));
            return grad;
        }




        /// <summary>
        ///Calculates the gradient of h vector at the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        protected double[,] calc_h_gradient(double[] point)
        {
            var result = new double[p, n];
            for (var i = 0; i != p; i++)
                for (var j = 0; j != n; j++)
                    result[i, j] = deriv_wrt_xi(h[i], point, j);
            return result;
        }

        /// <summary>
        /// Calculates the gradient of g vector at the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        protected double[,] calc_g_gradient(double[] point)
        {
            var result = new double[q, n];
            for (var i = 0; i != q; i++)
                for (var j = 0; j != n; j++)
                    result[i, j] = deriv_wrt_xi(g[i], point, j);
            return result;
        }

        /// <summary>
        /// Calculates the gradient of active vector at  the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        protected double[,] calc_active_gradient(double[] point)
        {
            var result = new double[m, n];
            for (var i = 0; i != m; i++)
                for (var j = 0; j != n; j++)
                    result[i, j] = deriv_wrt_xi(active[i], point, j);
            return result;
        }

        /// <summary>
        /// Calculates the gradient of h vector at the specified point
        /// at the specified indices.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="Indices">The indices.</param>
        /// <returns></returns>
        protected double[,] calc_h_gradient(double[] point, List<int> Indices)
        {
            var size = Indices.Count;
            var result = new double[p, size];
            for (var i = 0; i != p; i++)
                for (var j = 0; j != size; j++)
                    result[i, j] = deriv_wrt_xi(h[i], point, Indices[j]);
            return result;
        }
        /// <summary>
        /// Calculates the gradient of g vector at the specified point
        /// at the specified indices.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="Indices">The indices.</param>
        /// <returns></returns>
        protected double[,] calc_g_gradient(double[] point, List<int> Indices)
        {
            var size = Indices.Count;
            var result = new double[q, size];
            for (var i = 0; i != q; i++)
                for (var j = 0; j != size; j++)
                    result[i, j] = deriv_wrt_xi(g[i], point, Indices[j]);
            return result;
        }
        /// <summary>
        /// Calculates the gradient of active vector at the specified point
        /// at the specified indices.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="Indices">The indices.</param>
        /// <returns></returns>
        protected double[,] calc_active_gradient(double[] point, List<int> Indices)
        {
            var size = Indices.Count;
            var result = new double[m, size];
            for (var i = 0; i != m; i++)
                for (var j = 0; j != size; j++)
                    result[i, j] = deriv_wrt_xi(active[i], point, Indices[j]);
            return result;
        }


        #endregion

        /// <summary>
        /// Determines if the specified point is feasible.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public Boolean feasible(double[] point)
        {
            if (h.Any(a => !feasible(a, point)))
                return false;

            if (g.Any(a => !feasible(a, point)))
                return false;
            return true;
        }

        /// <summary>
        /// Determines if the specified point is feasible
        /// for the inequality, c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        private bool feasible(IInequality c, double[] point)
        {
            return (calculate(c, point) <= 0);
        }
        /// <summary>
        /// Determines if the specified point is feasible
        /// for the equality, c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        private bool feasible(IEquality c, double[] point)
        {
            return (calculate(c, point) == 0);
        }
        /// <summary>
        /// Determines if the specified point is feasible
        /// for the constraint, c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public bool feasible(IConstraint c, double[] point)
        {
            if (typeof(IEquality).IsInstanceOfType(c))
                return feasible((IEquality)c, point);
            if (typeof(IInequality).IsInstanceOfType(c))
                return feasible((IInequality)c, point);
            throw new Exception("IConstraint is neither IInequality or IEquality?!?");
        }


        /// <summary>
        /// Calculates the derivative with respect to variable xi
        /// for the specified function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="point">The point.</param>
        /// <param name="i">The index of the variable in x.</param>
        /// <returns></returns>
        public double deriv_wrt_xi(IOptFunction function, double[] point, int i)
        {
            switch (functionData[function].findDerivBy)
            {
                case differentiate.Analytic:
                    return ((IDifferentiable)function).deriv_wrt_xi(point, i);
                case differentiate.Back1:
                    return calcBack1(function, functionData[function].finiteDiffStepSize, point, i);
                case differentiate.Forward1:
                    return calcForward1(function, functionData[function].finiteDiffStepSize, point, i);
                case differentiate.Central2:
                    return calcCentral2(function, functionData[function].finiteDiffStepSize, point, i);
                case differentiate.Back2:
                    return calcBack2(function, functionData[function].finiteDiffStepSize, point, i);
                case differentiate.Forward2:
                    return calcForward2(function, functionData[function].finiteDiffStepSize, point, i);
                case differentiate.Central4:
                    return calcCentral4(function, functionData[function].finiteDiffStepSize, point, i);
            }
            return double.NaN;
        }


        #region finite difference

        private double calcBack1(IOptFunction function, double stepSize, double[] point, int i)
        {
            var backStep = (double[])point.Clone();
            backStep[i] -= stepSize;
            return (calculate(function, point) - calculate(function, backStep)) / stepSize;
        }
        private double calcForward1(IOptFunction function, double stepSize, double[] point, int i)
        {
            var forStep = (double[])point.Clone();
            forStep[i] += stepSize;
            return (calculate(function, forStep) - calculate(function, point)) / stepSize;
        }
        private double calcCentral2(IOptFunction function, double stepSize, double[] point, int i)
        {
            var forStep = (double[])point.Clone();
            var backStep = (double[])point.Clone();
            forStep[i] += stepSize;
            backStep[i] -= stepSize;
            return (calculate(function, forStep) - calculate(function, backStep)) / (2 * stepSize);
        }



        private double calcBack2(IOptFunction function, double stepSize, double[] point, int i)
        {
            var backStep1 = (double[])point.Clone();
            backStep1[i] -= stepSize;

            var backStep2 = (double[])point.Clone();
            backStep2[i] -= 2 * stepSize;
            return (calculate(function, backStep2) - 4 * calculate(function, backStep1) + 3 * calculate(function, point))
                / (2 * stepSize);
        }

        private double calcForward2(IOptFunction function, double stepSize, double[] point, int i)
        {
            var forStep1 = (double[])point.Clone();
            forStep1[i] += stepSize;

            var forStep2 = (double[])point.Clone();
            forStep2[i] += 2 * stepSize;
            return (-3 * calculate(function, point) + 4 * calculate(function, forStep1) - calculate(function, forStep2))
                / (2 * stepSize);
        }

        private double calcCentral4(IOptFunction function, double stepSize, double[] point, int i)
        {
            var forStep1 = (double[])point.Clone();
            forStep1[i] += stepSize;
            var forStep2 = (double[])point.Clone();
            forStep2[i] += 2 * stepSize;
            var backStep1 = (double[])point.Clone();
            backStep1[i] -= stepSize;
            var backStep2 = (double[])point.Clone();
            backStep2[i] -= 2 * stepSize;
            return (calculate(function, backStep2) - 8 * calculate(function, backStep1)
                    + 8 * calculate(function, forStep1) - calculate(function, forStep2)) / (12 * stepSize);
        }

        #endregion
    }
}
