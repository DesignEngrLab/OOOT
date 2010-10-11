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
        public List<abstractConvergence> ConvergenceMethods = new List<abstractConvergence>();
        protected DesignSpaceDescription spaceDescriptor;

        /* The following Booleans should be set in the constructor of every optimization method. 
         * Even if it seems redundant to do so, it is better to have them clearly indicated for each
         * method. */
        protected Boolean RequiresObjectiveFunction;
        protected Boolean ConstraintsSolvedWithPenalties;
        protected Boolean RequiresMeritFunction;
        protected Boolean InequalitiesConvertedToEqualities;
        protected Boolean RequiresSearchDirectionMethod;
        protected Boolean RequiresLineSearchMethod;
        protected Boolean RequiresAnInitialPoint;
        protected Boolean RequiresConvergenceCriteria;
        protected Boolean RequiresFeasibleStartPoint;
        protected Boolean RequiresDiscreteSpaceDescriptor;
        protected double[] xStart;
        protected double[] x;
        protected int feasibleOuterLoopMax;
        protected int feasibleInnerLoopMax;
        protected double epsilon;
        #endregion

        #region Set-up function, Add. */
        public virtual void Add(object function)
        {
            if (typeof(ProblemDefinition).IsInstanceOfType(function))
                readInProblemDefinition((ProblemDefinition)function);
            else if (typeof(inequality).IsInstanceOfType(function))
                g.Add((inequality)function);
            else if (typeof(equality).IsInstanceOfType(function))
                h.Add((equality)function);
            else if (typeof(objectiveFunction).IsInstanceOfType(function))
                objfn = (objectiveFunction)function;
            else if (typeof(abstractLineSearch).IsInstanceOfType(function))
            {
                lineSearchMethod = (abstractLineSearch)function;
                lineSearchMethod.SetOptimizationDetails(this);
            }
            else if (typeof(abstractSearchDirection).IsInstanceOfType(function))
                searchDirMethod = (abstractSearchDirection)function;
            else if (typeof(abstractMeritFunction).IsInstanceOfType(function))
                meritFunction = (abstractMeritFunction)function;
            else if (typeof(abstractConvergence).IsInstanceOfType(function))
                if (ConvergenceMethods.Exists(a => a.GetType() == function.GetType()))
                    throw new Exception("You are adding a convergence method of type " + function.GetType() +
                                        "to the optimization method but one already exists of this same type.");
                else ConvergenceMethods.Add((abstractConvergence)function);
            else if (typeof(double[]).IsInstanceOfType(function))
                xStart = (double[])function;
            else if (typeof(DesignSpaceDescription).IsInstanceOfType(function))
            {
                spaceDescriptor = (DesignSpaceDescription)function;
                n = spaceDescriptor.n;
            }
            else throw (new Exception("Function, " + function.ToString() + ", not of known type (needs "
                + "to inherit from inequality, equality, objectiveFunction, abstractLineSearch, " +
                "or abstractSearchDirection)."));
        }

        #endregion

        #region Initialize and Run funtions
        public double Run(out double[] xStar)
        {
            if (xStart != null) return Run(out xStar, xStart);
            if (n > 0) return run(out xStar, null);
            SearchIO.output("The number of variables was not set or determined from inputs.", 0);
            xStar = null;
            return double.PositiveInfinity;
        }
        public double Run(out double[] xStar, double[] xInit)
        {
            n = xInit.GetLength(0);
            return run(out xStar, xInit);
        }

        public double Run(out double[] xStar, int NumberOfVariables)
        {
            n = NumberOfVariables;
            return run(out xStar, null);
        }
        private double run(out double[] xStar, double[] xInit)
        {
            xStar = null;
            fStar = double.PositiveInfinity;
            // k = 0 --> iteration counter
            k = 0;

            if (RequiresObjectiveFunction && (objfn == null))
            {
                SearchIO.output("No objective function specified.", 0);
                return fStar;
            }
            if (RequiresSearchDirectionMethod && (searchDirMethod == null))
            {
                // it'd be cool to use reflection to find all the possible implementations
                // of the abstractSearchDirection class and present them at this time. 
                // e.g. "Consider using SteepestDescent, BFGS, etc."
            }
            if (RequiresLineSearchMethod && (lineSearchMethod == null))
            {
                SearchIO.output("No line search method specified.", 0);
                return fStar;
            }
            if (RequiresConvergenceCriteria && ConvergenceMethods.Count == 0)
            {
                SearchIO.output("No convergence method specified.", 0);
                return fStar;
            }
            if (RequiresMeritFunction && meritFunction == null)
            {
                SearchIO.output("No merit function specified.", 0);
                return fStar;
            }
            if (RequiresDiscreteSpaceDescriptor && (spaceDescriptor == null))
            {
                SearchIO.output("No description of the discrete space is specified.", 0);
                return fStar;
            }
            if (g.Count == 0) SearchIO.output("No inequalities specified.", 4);
            if (h.Count == 0) SearchIO.output("No equalities specified.", 4);
            if (ConstraintsSolvedWithPenalties && (h.Count + g.Count > 0))
                SearchIO.output("Constsraints will be solved with exterior penalty function.", 4);
            if (InequalitiesConvertedToEqualities && (g.Count > 0))
                SearchIO.output(g.Count + " inequality constsraints will be converted to equality" +
                    " constraints with the addition of " + g.Count + " slack variables.", 4);

            if (RequiresAnInitialPoint)
            {
                if (xInit != null)
                {
                    xStart = (double[])xInit.Clone();
                    x = (double[])xInit.Clone();
                }
                else if (xStart != null) x = (double[])xStart.Clone();
                else
                {
                    // no? need a random start
                    x = new double[n];
                    var randy = new Random();
                    for (int i = 0; i < n; i++)
                        x[i] = 100.0 * randy.NextDouble();
                }
                if (RequiresFeasibleStartPoint && !feasible(x))
                    if (!findFeasibleStartPoint()) return fStar;
            }
            p = h.Count;
            q = g.Count;
            m = p;

            if (InequalitiesConvertedToEqualities && (q > 0))
            {
                var xnew = new double[n + q];
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
                return fStar;
            }

            return run(out xStar);
        }


        protected abstract double run(out double[] xStar);


        private Boolean findFeasibleStartPoint()
        {
            double average = StarMath.norm1(x) / x.GetLength(0);
            Random randNum = new Random();
            // n-m variables can be changed

            double[,] gradH = calc_h_gradient(x);

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



        #endregion

        #region Calculate f, g, h helper functions
        protected Boolean feasible(double[] x)
        {
            foreach (equality a in h)
                if (!a.feasible(x)) return false;
            foreach (inequality a in g)
                if (!a.feasible(x)) return false;
            return true;
        }
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

        #region from/to Problem Definition
        private void readInProblemDefinition(ProblemDefinition pd)
        {
            if (pd.g != null)
                foreach (inequality gNew in pd.g)
                    Add(gNew);
            if (pd.h != null)
                foreach (equality hNew in pd.h)
                    Add(hNew);
            if (pd.f != null) Add(pd.f);
            if (pd.ConvergenceMethods != null)
                foreach (var cM in pd.ConvergenceMethods)
                    Add(cM);
            if ((pd.tolerance > double.Epsilon) && (pd.tolerance < double.PositiveInfinity))
                this.epsilon = pd.tolerance;
            if (pd.SpaceDescriptor != null) Add(pd.SpaceDescriptor);
            if ((pd.xStart != null) && (pd.xStart.GetLength(0) > 0))
            {
                xStart = (double[])pd.xStart.Clone();
                n = xStart.GetLength(0);
            }
        }

        public ProblemDefinition createProblemDefinition()
        {
            var pd = new ProblemDefinition();
            pd.ConvergenceMethods = this.ConvergenceMethods;
            pd.f = objfn;
            pd.g = new List<inequality>();
            foreach (inequality ineq in g)
                if (ineq.GetType() == typeof(lessThanConstant))
                {
                    double ub = ((lessThanConstant)ineq).constant;
                    int varIndex = ((lessThanConstant)ineq).index;
                    pd.SpaceDescriptor.VariableDescriptors[varIndex].UpperBound = ub;
                }
                else if (ineq.GetType() == typeof(greaterThanConstant))
                {
                    double lb = ((greaterThanConstant)ineq).constant;
                    int varIndex = ((greaterThanConstant)ineq).index;
                    pd.SpaceDescriptor.VariableDescriptors[varIndex].UpperBound = lb;
                }
                else pd.g.Add(ineq);
            pd.h = new List<equality>();
            foreach (equality eq in h)
                pd.h.Add(eq);
            pd.tolerance = this.epsilon;
            pd.xStart = this.xStart;
            return pd;
        }
        #endregion

        #region Convergence Main Function
        public string ConvergenceDeclaredBy { get { return ConvergenceMethods[indexConverged].GetType().ToString().Remove(0, 20); } }
        private int indexConverged;
        protected Boolean notConverged(int YInteger = int.MinValue, double YDouble = double.NaN,
               IList<double> YDoubleArray1 = null, IList<double> YDoubleArray2 = null, IList<double[]> YJaggedDoubleArray = null)
        {
            for (indexConverged = 0; indexConverged < ConvergenceMethods.Count; indexConverged++)
            {
                if (ConvergenceMethods[indexConverged].converged(YInteger, YDouble, YDoubleArray1, YDoubleArray2,
                                                                 YJaggedDoubleArray))
                    return false;
            }
            indexConverged = -1;
            return true;
        }
        #endregion
    }
}
