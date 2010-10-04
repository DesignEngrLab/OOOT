using System;

using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
    public abstract class abstractOptMethod
    {
        #region Fields
        /* I usually object to such simple names for variables, but this 
         * follows the convention used in my course - ME392C at UT Austin. */
        protected int n; /* the total number of design variables - the length of x. */
        protected int m; /* the number of active constraints. */
        protected int p; /* the number of equality constraints - length of h. */
        protected int q; /* the number of inequality constraints - length of g. */
        protected int k; /* the iteration counter. */
        protected objectiveFunction objfn;
        protected double fStar = double.PositiveInfinity; /*fStar is the optimum that is returned at the end of run. */
        /* 'active' is the set of Active Constraints. For simplicity all equality constraints 
         * are assumed to be active, and any additional g's that come and go in this active
         * set strategy. More importantly we want the gradient of A which is a m by n matrix. 
         * m is the # of active constraints and n is the # of variables. */
        protected List<constraint> active = new List<constraint>();
        public List<constraint> h = new List<constraint>();
        public List<constraint> g = new List<constraint>();
        protected abstractSearchDirection searchDirMethod;
        protected abstractLineSearch lineSearchMethod;
        protected abstractMeritFunction meritFunction;
        protected IList<abstractConvergence> convergeMethods = new List<abstractConvergence>();
        protected DiscreteSpaceDescriptor discreteSpace;
        protected Boolean ObjectiveFunctionNeeded = true;
        protected Boolean ConstraintsSolvedWithPenalties = false;
        protected Boolean InequalitiesConvertedToEqualities = false;
        protected Boolean SearchDirectionMethodNeeded = true;
        protected Boolean LineSearchMethodNeeded = true;
        protected Boolean FeasibleStartPointRequired = false;
        protected Boolean DiscreteSpaceNeeded = false;
        protected double[] xStart;
        protected int feasibleOuterLoopMax;
        protected int feasibleInnerLoopMax;
        protected double epsilon;
        #endregion

        #region Set-up function, Add. */
        public void Add(object function)
        {
            if (function.GetType() == typeof(ProblemDefinition))
            {
                ProblemDefinition pd = (ProblemDefinition)function;
                
                if (pd.g != null)
                {
                    g.Clear();
                    foreach (inequality gNew in pd.g)
                        g.Add(gNew);
                }
                if (pd.h != null)
                {
                    h.Clear();
                    foreach (equality hNew in pd.h)
                        h.Add(hNew);
                }
                if (pd.f != null)
                {
                    this.objfn = pd.f;
                    if (lineSearchMethod != null) lineSearchMethod.optMethod = this;
                }
                if (pd.convergeMethod != null)
                    this.convergeMethod = pd.convergeMethod;
                if ((pd.tolerance > double.Epsilon) && (pd.tolerance < double.PositiveInfinity))
                    this.epsilon = pd.tolerance;
                if (pd.bounds.GetLength(0) > 0) //&& (pd.bounds.GetLength(1) == 2))
                    for (int i = 0; i != pd.bounds.GetLength(0); i++)
                    {
                        if (pd.bounds[i][0] > double.NegativeInfinity)
                            g.Add(new greaterThanConstant(pd.bounds[i][0], i));
                        if (pd.bounds[i][1] < double.PositiveInfinity)
                            g.Add(new lessThanConstant(pd.bounds[i][1], i));
                    }
                if ((pd.xStart != null) && (pd.xStart.GetLength(0) > 0))
                    xStart = (double[])pd.xStart.Clone();
            }
            //*Added in by Bill Patterson/////////////////////////////////////////
            //else if (function.GetType().BaseType == typeof(double[][]))
            //{
            //    double[][] boundsCopy = function;
            //    if (boundsCopy.GetLength(0) > 0) //&& (pd.bounds.GetLength(1) == 2))
            //    for (int i = 0; i != pd.bounds.GetLength(0); i++)
            //    {
            //        if (function[i][0] > double.NegativeInfinity)
            //            g.Add(new greaterThanConstant(pd.bounds[i][0], i));
            //        if (function[i][1] < double.PositiveInfinity)
            //            g.Add(new lessThanConstant(pd.bounds[i][1], i));
            //    }
            //}
            //*End of Add in////////////////////////////////////////////
            else if (function.GetType().BaseType == typeof(inequality))
                g.Add((inequality)function);
            else if (function.GetType().BaseType == typeof(equality))
                h.Add((equality)function);
            else if (function.GetType().BaseType == typeof(objectiveFunction))
            {
                objfn = (objectiveFunction)function;
            }
            else if (function.GetType().BaseType == typeof(abstractLineSearch))
            {
                lineSearchMethod = (abstractLineSearch)function;
                if (lineSearchMethod.optMethod != this)
                    lineSearchMethod.optMethod = this;
            }
            else if (function.GetType().BaseType == typeof(abstractSearchDirection))
                searchDirMethod = (abstractSearchDirection)function;
            else if (function.GetType().BaseType == typeof(abstractMeritFunction))
                meritFunction = (abstractMeritFunction)function;
            else if (function.GetType().BaseType == typeof(abstractConvergence))
                convergeMethods.Add((abstractConvergence)function);
            else if (function.GetType() == typeof(double[]))
                xStart = (double[])function;
            else if (function.GetType() == typeof(DiscreteSpaceDescriptor))
                discreteSpace = (DiscreteSpaceDescriptor)function;
            else throw (new Exception("Function, " + function.ToString() + ", not of known type (needs "
                + "to inherit from inequality, equality, objectiveFunction, abstractLineSearch, " +
                "or abstractSearchDirection)."));
        }
        #endregion

        #region Initialize funtions
        public Boolean initializeAndCheck(ref double[] x)
        {
            fStar = double.PositiveInfinity;
            if ((x == null) && (n > 0))
            {
                x = new double[n];
                Random randy = new Random();
                for (int i = 0; i < n; i++)
                    x[i] = 100.0 * randy.NextDouble();
            }
            else if (x==null) return false;
            if (!completelySetUp()) return false;
            n = x.GetLength(0);
            p = h.Count;
            q = g.Count;
            m = p;

            if (FeasibleStartPointRequired && (!feasible(x))) return findFeasibleStartPoint(x);

            if (InequalitiesConvertedToEqualities && (q > 0))
            {
                double[] xnew = new double[n + q];
                for (int i = 0; i != n; i++)
                    xnew[i] = x[i];
                for (int i = n; i != n + q; i++)
                {
                    double sSquared = g[i - n].calculate(x);
                    if (sSquared < 0) xnew[i] = Math.Sqrt(-sSquared);
                    else xnew[i] = epsilon;
                    h.Add(new slackSquaredEqualityFromInequality(g[i - n], i));
                }
                x = xnew;
                n = x.GetLength(0);
                m = h.Count;
                p = h.Count;
            }


            if (n <= m)
            {
                if (n == m)
                    SearchIO.output("There are as many equality constraints as design variables " +
                        "(m = size). Consider another approach. Optimization is not needed.");
                else
                    SearchIO.output("There are more equality constraints than design variables " +
                        "(m > size). Therefore the problem is overconstrained.");
                return false;
            }

            return true;
        }

        private Boolean findFeasibleStartPoint(double[] x)
        {
            double[] xlast = (double[])x.Clone();
            double average = StarMath.norm1(x) / x.GetLength(0);
            Random randNum = new Random();
            List<int> varsToChange = new List<int>(n - m);
            // n-m variables can be changed

            double[,] gradH = calc_h_gradient(x);
            double[,] invGradH = StarMath.inverse(gradH);

            for (int outerK = 0; outerK < feasibleOuterLoopMax; outerK++)
            {
                SearchIO.output("looking for feasible start point (attempt #" + outerK, 4);
                for (int innerK = 0; innerK < feasibleOuterLoopMax; innerK++)
                {
                    // gradA = calc_h_gradient(x, varsToChange);
                    // invGradH = StarMath.inverse(gradA);
                    //x = StarMath.subtract(x, StarMath.multiply(invGradH, calc_h_vector(x)));
                    if (feasible(x)) return true;
                }
                for (int i = 0; i < n; i++)
                    x[i] += 2 * average * (randNum.NextDouble() - 0.5);

                //gradA = calc_h_gradient(x);
                //invGradH = StarMath.inverse(gradA);
                if (feasible(x)) return true;
            }
            return false;
        }

        private Boolean feasible(double[] x)
        {
            foreach (equality a in h)
                if (!a.feasible(x)) return false;
            foreach (inequality a in g)
                if (!a.feasible(x)) return false;
            return true;
        }

        private Boolean completelySetUp()
        {
            Boolean complete = true;
            if (ObjectiveFunctionNeeded && (objfn == null))
            {
                SearchIO.output("No objective function specified.", 0);
                complete = false;
            }
            if (SearchDirectionMethodNeeded && (searchDirMethod == null))
            {
                // it'd be cool to use reflection to find all the possible implementations
                // of the abstractSearchDirection class and present them at this time. 
                // e.g. "Consider using SteepestDescent, BFGS, etc."
                SearchIO.output("No search direction method specified.", 0);
                complete = false;
            }
            if (LineSearchMethodNeeded && (lineSearchMethod == null))
            {
                SearchIO.output("No line search method specified.", 0);
                complete = false;
            }
            if (convergeMethod == null)
            {
                SearchIO.output("No convergence method specified.", 0);
                complete = false;
            } 
            if (meritFunction == null)
            {
                SearchIO.output("No merit function specified.", 0);
                complete = false;
            }
            if (DiscreteSpaceNeeded  && (discreteSpace == null))
            {
                SearchIO.output("No description of the discrete space is specified.", 0);
                complete = false;
            }
            if (g.Count == 0) SearchIO.output("No inequalities specified.", 4);
            if (h.Count == 0) SearchIO.output("No equalities specified.", 4);
            if (ConstraintsSolvedWithPenalties && (h.Count + g.Count > 0))
                SearchIO.output("Constsraints will be solved with exterior penalty function.", 4);
            if (InequalitiesConvertedToEqualities && (g.Count > 0))
                SearchIO.output(g.Count + " inequality constsraints will be converted to equality" +
                    " constraints with the addition of " + g.Count + " slack variables.", 4);
            return complete;
        }
        #endregion

        #region Calculate f, g, h helper functions
        protected double[] calc_h_vector(double[] x)
        {
            double[] vals = new double[p];
            for (int i = 0; i != p; i++)
                vals[i] = h[i].calculate(x);
            return vals;
        }
        protected double[] calc_h_vector(double[] x, List<int> workingSet)
        {
            int workSetLength = workingSet.Count;
            double[] vals = new double[workSetLength];
            for (int i = 0; i != workSetLength; i++)
                vals[i] = h[workingSet[i]].calculate(x);
            return vals;
        }
        protected double[,] calc_h_gradient(double[] x)
        {
            double[,] result = new double[p, n];
            for (int i = 0; i != p; i++)
                for (int j = 0; j != n; j++)
                    result[i, j] = h[i].deriv_wrt_xi(x, j);
            return result;
        }
        protected double[,] calc_h_gradient(double[] x, List<int> Indices)
        {
            int size = Indices.Count;
            double[,] result = new double[p, size];
            for (int i = 0; i != p; i++)
                for (int j = 0; j != size; j++)
                    result[i, j] = h[i].deriv_wrt_xi(x, Indices[j]);
            return result;
        }

        protected double[] calc_g_vector(double[] x)
        {
            double[] vals = new double[q];
            for (int i = 0; i != q; i++)
                vals[i] = g[i].calculate(x);
            return vals;
        }
        protected double[] calc_g_vector(double[] x, List<int> workingSet)
        {
            int workSetLength = workingSet.Count;
            double[] vals = new double[workSetLength];
            for (int i = 0; i != workSetLength; i++)
                vals[i] = g[workingSet[i]].calculate(x);
            return vals;
        }
        protected double[,] calc_g_gradient(double[] x)
        {
            double[,] result = new double[q, n];
            for (int i = 0; i != q; i++)
                for (int j = 0; j != n; j++)
                    result[i, j] = g[i].deriv_wrt_xi(x, j);
            return result;
        }
        protected double[,] calc_g_gradient(double[] x, List<int> Indices)
        {
            int size = Indices.Count;
            double[,] result = new double[q, size];
            for (int i = 0; i != q; i++)
                for (int j = 0; j != size; j++)
                    result[i, j] = g[Indices[i]].deriv_wrt_xi(x, Indices[j]);
            return result;
        }
        protected double[,] calc_active_gradient(double[] x)
        {
            double[,] result = new double[m, n];
            for (int i = 0; i != m; i++)
                for (int j = 0; j != n; j++)
                    result[i, j] = active[i].deriv_wrt_xi(x, j);
            return result;
        }
        protected double[,] calc_active_gradient(double[] x, List<int> Indices)
        {
            int size = Indices.Count;
            double[,] result = new double[m, size];
            for (int i = 0; i != m; i++)
                for (int j = 0; j != size; j++)
                    result[i, j] = active[i].deriv_wrt_xi(x, Indices[j]);
            return result;
        }
        protected double[] calc_active_vector(double[] x)
        {
            double[] vals = new double[m];
            for (int i = 0; i != m; i++)
                vals[i] = active[i].calculate(x);
            return vals;
        }

        public double[] calc_f_gradient(double[] x)
        { return calc_f_gradient(x, false); }
        public double[] calc_f_gradient(double[] x, Boolean includeMeritPenalty)
        {
            double[] grad = new double[n];
            for (int i = 0; i != n; i++)
                grad[i] = objfn.deriv_wrt_xi(x, i);
            if (this.ConstraintsSolvedWithPenalties || includeMeritPenalty)
                return StarMath.add(grad, meritFunction.calcGradientOfPenalty(x));
            else return grad;
        }

        public double calc_f(double[] point)
        { return calc_f(point, false); }
        public double calc_f(double[] point, Boolean includeMeritPenalty)
        {
            if (this.ConstraintsSolvedWithPenalties || includeMeritPenalty)
                return objfn.calculate(point) + meritFunction.calcPenalty(point);
            else return objfn.calculate(point);
        }
        #endregion

        #region Create Problem Definition
        public ProblemDefinition createProblemDefinition()
        {
            ProblemDefinition pd = new ProblemDefinition();
            pd.bounds = getBoundsFromLTandGTInequalities();
            pd.convergeMethod = this.convergeMethod;
            pd.f = this.objfn;
            pd.g = new List<inequality>();
            foreach (inequality ineq in g)
                if ((ineq.GetType() != typeof(lessThanConstant)) &&
                    (ineq.GetType() != typeof(greaterThanConstant)))
                    pd.g.Add(ineq);
            pd.h = new List<equality>();
            foreach (equality eq in h)
                pd.h.Add(eq);
            pd.tolerance = this.epsilon;
            pd.xStart = this.xStart;
            return pd;
        }

        private double[][] getBoundsFromLTandGTInequalities()
        {
            int numVar = Math.Max(n, this.xStart.GetLength(0));
            foreach (inequality ineq in g)
            {
                if (ineq.GetType() == typeof(lessThanConstant))
                {
                    lessThanConstant lt = (lessThanConstant)ineq;
                    numVar = Math.Max(numVar, lt.index);
                }
                else if (ineq.GetType() == typeof(greaterThanConstant))
                {
                    greaterThanConstant gt = (greaterThanConstant)ineq;
                    numVar = Math.Max(numVar, gt.index);
                }
            }
            double[][] bounds = new double[numVar][];
            for (int i = 0; i != numVar; i++)
            {
                bounds[i] = new double[2];
                bounds[i][0] = double.NegativeInfinity;
                bounds[i][1] = double.PositiveInfinity;
            }

            foreach (inequality ineq in g)
            {
                if (ineq.GetType() == typeof(lessThanConstant))
                {
                    lessThanConstant lt = (lessThanConstant)ineq;
                    bounds[lt.index][0] = lt.constant;
                }
                else if (ineq.GetType() == typeof(greaterThanConstant))
                {
                    greaterThanConstant gt = (greaterThanConstant)ineq;
                    bounds[gt.index][0] = gt.constant;
                }
            }
            return bounds;
        }

        #endregion

        #region Run
        public double run(out double[] xStar)
        { return this.run(xStart, out xStar); }
        public double run(int n, out double[] xStar)
        {
            this.n = n;
            return this.run(xStart, out xStar);
        }
        public abstract double run(double[] x0, out double[] xStar);
        #endregion
    }
}
