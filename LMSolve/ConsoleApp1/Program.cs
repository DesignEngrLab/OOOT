// See https://aka.ms/new-console-template for more information
Console.WriteLine("How to use Levenberg-Marquadt Optimization");

var p = new NonlinearProblemToSolve(1, 15); //make an object from your class that inherit from 
var minError = 0.0001; // the solving is accopmlished by an optimization to zero. This value (the
                       // first argument in the RunOptimization) is your allowable amount of error
var minErrorDiff = 0.00001; // this convergence criteria is to stop the iteration if the improvement
                            // is less than this amount. This should probably be one or two orders
                            // of magnitude smaller than minError
var maxIterations = 1000; // this is a good upper value for this. It's possible that it finishes in 10 iterations
                          // so a value at 1000 is pretty conservative.

// now run the optimization
p.RunOptimization(minError,minErrorDiff,maxIterations);

// if you want to see the final error, you can check the value of the optimum at the end.
// just know that this is the squared-error, so we need to take the square root
var finalError = Math.Sqrt(p.f);

// the optimal/best solution is stored in x.
var solution = p.x;

Console.WriteLine("here is the answer: " + String.Join(", ", p.x));


internal class NonlinearProblemToSolve : LMOpt.LevenbergMarquadtOptimization
{
    public NonlinearProblemToSolve(int numResidualTerms, int xLength = -1) : base(numResidualTerms, xLength)
    {
    }

    protected override double[,] solveGradientOfResiduals(double[] x)
    {
        throw new NotImplementedException();
    }

    protected override double[] solveResiduals(double[] x)
    {
        throw new NotImplementedException();
    }
}