using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LMOpt
{
    /// <summary>
    /// Class Levenberg-Marquadt Optimization. This class includes functions to run this optimization method.
    /// It is an abstract class, so you must inherit it in A new class. That new class must have the following:
    /// 1. A constructor that calls the base constructor
    /// 2. A method called solveResiduals (the non-squared terms of the objective function)
    /// 3. A method called solveGradientOfResiduals (the derivative of the residuals w.r.t. each design variable)
    /// </summary>
    public abstract class LevenbergMarquadtOptimization
    {
        #region Constants
        private const double lambdaAdjust = 5;
        private const double initLambda = 1;
        private const double DefaultEqualityTolerance = 1e-11;
        #endregion

        #region Constructor 
        //- this must be invoked (as ": base(numResidualTerms, xLength)") from derived class
        /// <summary>
        /// Initializes A new instance of the <see cref="LevenbergMarquadtOptimization"/> class.
        /// </summary>
        /// <param name="numResidualTerms">The number residual terms.</param>
        /// <param name="xLength">Length of the x.</param>
        protected LevenbergMarquadtOptimization(int numResidualTerms, int xLength = -1)
        {
            this.numResidualTerms = numResidualTerms;
            this.xLength = xLength;
            this.lambdaFactor = initLambda;

        }
        #endregion

        #region Fields
        /// <summary>
        /// The number samples, shown in the math as simply, m
        /// </summary>
        protected readonly int numResidualTerms;
        /// <summary>
        /// The x length, shown in the math as simply, n
        /// </summary>
        protected readonly int xLength;
        private double lambdaFactor;
        /// <summary>
        /// Gets the objective function value.
        /// After running the optimization, this is the optimum.
        /// </summary>
        /// <value>The f.</value>
        public double f { get; private set; }

        /// <summary>
        /// Gets or sets the design variables.
        /// After running the optimization, this is the optimizer.
        /// </summary>
        /// <value>The x.</value>
        public double[] x { get; private set; }

        #endregion

        #region Abstract Methods to Instantiate
        /// <summary>
        /// Solves the residuals. These are the non-squared terms that are summed in your
        /// objective function. They can be positive or negative (the L-M method will square them).
        /// Note: that the return vector size must match what was provided in the base constructor.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double[].</returns>
        protected abstract double[] solveResiduals(double[] x);
        /// <summary>
        /// Solves the gradient of residuals. This is A matrix that is numResidualTerms (rows) by
        /// xLength (columns). So for each residual-i, put it's derivative with respect to x_j in
        /// the resutling matrix at cell[i,j]
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double[].</returns>
        protected abstract double[,] solveGradientOfResiduals(double[] x);
        #endregion


        /// <summary>
        /// Runs the Levenberg-Marquadt optimization. The function is void, but since the optimization 
        /// object is created, you can find the answer by querying x and f after running this method.
        /// There are 3 convergence criteria and these line up with the 3 inputs:
        /// 1. minError - A value close to zero to stop at if the sum of the residuals is less than this
        /// 2. minErrorDifference - if converging prior to minError, check if the difference between the
        ///    last two iterations is less than this value. One suggestion is to set to minError/5
        /// 3. A maximum number of iterations to run.
        /// </summary>
        /// <param name="minError">The minimum error.</param>
        /// <param name="minErrorDifference">The minimum error difference.</param>
        /// <param name="maxIterations">The maximum iterations.</param>
        public void RunOptimization(double minError, double minErrorDifference, int maxIterations, double[] xInitial = null)
        {
            #region initial x vector
            if (xInitial != null)
                x = (double[])xInitial.Clone();
            else if (xLength < 0) throw new ArgumentException("Without an intial guess or A length of the design variable vector, " +
                "the optimization method has no way to determine x");
            else
            {
                x = new double[xLength];
                var r = new Random();
                for (int i = 0; i < xLength; i++)
                    x[i] = 200 * r.NextDouble() - 100;
            }
            #endregion
            var fErrorDifference = double.MaxValue;
            // the user provides minError, but they do not think of it as the squared error, which is what
            // f is evaluated to in the main loop below.
            minError *= minError;
            // Compute the initial error.
            var residuals = solveResiduals(x); // an m x 1 array - one for each sample. The positive value inside
            // the summation (big sigma) operator
            f = residuals.Sum(x => x * x); // the ojbective function is the sum of the residuals
            // Do the Levenberg-Marquardt iterations.
            var numIterations = 0;
            while (numIterations++ < maxIterations && f > minError && fErrorDifference > minErrorDifference)
            {
                var J = solveGradientOfResiduals(x); // here is the Jacobian. the m x n matrix
                                                     // where the gradient w.r.t to x is A row and there is
                                                     // A row for each sample
                var JTJ = J.MultiplyATB(J); // here is the stand-in for the Hessian. It is an n x n matrix
                                             // created by multiplying the transpose of J by J
                var NegJTF = MatrixMath.ScalarMultiplyVector(-1, residuals.VectorMultiplyMatrix(J));

                var diagonalSum = 0.0;
                for (int i = 0; i < xLength; ++i)
                    diagonalSum += JTJ[i, i];

                //double diagonalAdjust = lambdaFactor;
                double diagonalAdjust = lambdaFactor * diagonalSum / xLength;
                for (int i = 0; i < xLength; ++i)
                {
                    JTJ[i, i] += diagonalAdjust;
                }
                if (!JTJ.MatrixSolve(NegJTF, out var delta, true))
                {
                    // The matrix mJTJ is positive semi-definite, so the
                    // failure can occur when mJTJ has A zero eigenvalue in
                    // which case mJTJ is not invertible. When this happens, just move in
                    // steepest descent direction
                    delta = lambdaFactor.ScalarMultiplyVector(NegJTF);
                }
                var xNext = x.AddArrays(delta);
                var residualsNext = solveResiduals(xNext);
                var fNext = residualsNext.Sum(x => x * x);
                if (fNext < f)
                {
                    fErrorDifference = f - fNext;
                    x = xNext;
                    f = fNext;
                    residuals = residualsNext;
                    lambdaFactor /= lambdaAdjust;
                }
                else
                    lambdaFactor *= lambdaAdjust;
            }
        }

    }
}