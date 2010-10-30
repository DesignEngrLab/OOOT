using System;
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    public abstract partial class abstractOptMethod
    {

        public int m { get; set; }

        public int p { get; set; }

        public int q { get; set; }

        public long numEvals
        {
            get
            {
                var numEvalList = new List<long>(functionData.Values.Select(a => a.numEvals));
                return numEvalList.Max();
            }
        }
        internal List<IObjectiveFunction> f { get; private set; }
        internal List<IEquality> h { get; private set; }
        internal List<IInequality> g { get; private set; }
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



        public void ResetFunctionEvaluationDatabase()
        {
            foreach (var fd in functionData)
                fd.Value.Clear();
        }


        #region Calculate f, g, h helper functions
        internal double calculate(IOptFunction function, double[] point)
        {
            double fValue;
            if (functionData[function].TryGetValue(point, out fValue))
                return fValue;

            calc_dependent_Analysis(point);
            /**************************************************/
            /*** This is the only function that should call ***/
            /**********IOptFunction.calculate(x)***************/
            fValue = function.calculate(point);
            /**************************************************/
            functionData[function].Add((double[])point.Clone(), fValue);
            functionData[function].numEvals++;
            return fValue;
        }


        // the reason this function is public but the remainder are not, is because
        // this is called from other classes. Most notably, the line search methods, 
        // and the initial sampling in SA to get a temperature.
        public double calc_f(double[] point, Boolean includeMeritPenalty = false)
        {
            var penalty = ((g.Count + h.Count > 0) && (ConstraintsSolvedWithPenalties || includeMeritPenalty))
                ? meritFunction.calcPenalty(point) : 0.0;
            return calculate(f[0], point) + penalty;
        }


        public double[] calc_f_vector(double[] point, Boolean includeMeritPenalty = false)
        { return f.Select(fi => calculate(fi, point)).ToArray(); }
        protected double[] calc_h_vector(double[] point)
        { return h.Select(h0 => calculate(h0, point)).ToArray(); }
        protected double[] calc_g_vector(double[] point)
        { return g.Select(g0 => calculate(g0, point)).ToArray(); }
        protected double[] calc_active_vector(double[] point)
        { return active.Select(a => calculate(a, point)).ToArray(); }

        protected double[] calc_f_gradient(double[] point, Boolean includeMeritPenalty = false)
        {
            var grad = new double[n];
            for (var i = 0; i != n; i++)
                grad[i] = deriv_wrt_xi(f[0], point, i);
            if (ConstraintsSolvedWithPenalties || includeMeritPenalty)
                return StarMath.add(grad, meritFunction.calcGradientOfPenalty(point));
            return grad;
        }




        protected double[,] calc_h_gradient(double[] point)
        {
            var result = new double[p, n];
            for (var i = 0; i != p; i++)
                for (var j = 0; j != n; j++)
                    result[i, j] = deriv_wrt_xi(h[i], point, j);
            return result;
        }

        protected double[,] calc_g_gradient(double[] point)
        {
            var result = new double[q, n];
            for (var i = 0; i != q; i++)
                for (var j = 0; j != n; j++)
                    result[i, j] = deriv_wrt_xi(g[i], point, j);
            return result;
        }

        protected double[,] calc_active_gradient(double[] point)
        {
            var result = new double[m, n];
            for (var i = 0; i != m; i++)
                for (var j = 0; j != n; j++)
                    result[i, j] = deriv_wrt_xi(active[i], point, j);
            return result;
        }

        protected double[,] calc_h_gradient(double[] point, List<int> Indices)
        {
            var size = Indices.Count;
            var result = new double[p, size];
            for (var i = 0; i != p; i++)
                for (var j = 0; j != size; j++)
                    result[i, j] = deriv_wrt_xi(h[i], point, Indices[j]);
            return result;
        }
        protected double[,] calc_g_gradient(double[] point, List<int> Indices)
        {
            var size = Indices.Count;
            var result = new double[q, size];
            for (var i = 0; i != q; i++)
                for (var j = 0; j != size; j++)
                    result[i, j] = deriv_wrt_xi(g[i], point, Indices[j]);
            return result;
        }
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

        internal Boolean feasible(double[] point)
        {
            if (h.Any(a => !feasible(a, point)))
                return false;

            if (g.Any(a => !feasible(a, point)))
                return false;
            return true;
        }

        internal bool feasible(IInequality c, double[] point)
        {
            return (calculate(c, point) <= 0);
        }
        internal bool feasible(IEquality c, double[] point)
        {
            return (calculate(c, point) == 0);
        }
        internal bool feasible(IConstraint c, double[] point)
        {
            if (typeof(IEquality).IsInstanceOfType(c))
                return feasible((IEquality)c, point);
            if (typeof(IInequality).IsInstanceOfType(c))
                return feasible((IInequality)c, point);
            throw new Exception("IConstraint is neither IInequality or IEquality?!?");
        }


        internal double deriv_wrt_xi(IOptFunction function, double[] point, int i)
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
