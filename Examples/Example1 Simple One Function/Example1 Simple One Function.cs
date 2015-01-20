/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using OptimizationToolbox;
using StarMathLib;
using tester;

namespace Example1_Simple_One_Function
{
    class Program
    {

        private static void Main()
        {
            Parameters.Verbosity = VerbosityLevels.AboveNormal;
            // this next line is to set the Debug statements from OOOT to the Console.
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            /* first a new optimization method in the form of Nelder-Mead method
             * (http://comjnl.oxfordjournals.org/content/7/4/308.abstract). This 
             * is the method also at the heart of MatLab's fsolve function. It's a
             * clever and wholly unique method that works well for low-dimensions
             * problems with no constraints (if constraints are used, you will need
             * to specify a merit/penalty function. */
            //var optMethod = new NelderMead();
            //var optMethod = new PowellsOptimization();
            var optMethod = new HookeAndJeeves(1,0.5, 1, 1e-10);
            //var optMethod = new Rosenbrock(3, -0.5, 1, 1e-10);
            optMethod.Add(new DSCPowell(0.01, 0.1, 50));
            /* The objective function is Rosenbrock's banana function
             * (http://en.wikipedia.org/wiki/Rosenbrock_function). 
             * All objective functions must a object from classes that inherit
             * from the IObjectiveFunction interface. Within the toolbox there
             * is one such method called polynomialObjFn. Like polynomialInequality,
             * and polynomialInequality, this class store a list of polynomial
             * terms which are summed together. An individual term can be comprised
             * of a constant and any (or all) variables raised to any power.
             * These methods cannot handle functions where there are individual
             * sums that are multiplied or raised to a power. Therefore, the equation
             * for Rosenbrock's shown at the top of the wikipedia page must be
             * broken down into individual terms like that shown below. */
            // optMethod.Add(new efficiencyMeasurement());
            optMethod.Add(new BragasFruitTruck());
            //optMethod.Add(new polynomialObjFn
            //                  {
            //                      Terms = new List<string>
            //    {
            //        "100*x1^4",
            //        "-200*x1^2*x2",
            //        "x1^2",
            //        "-2*x1",
            //        "100*x2^2",
            //        "1",
            //    }
            //                  });
            /* At least one convergence method is required for NelderMead.
             * Since we know the optimal is 0 (@ {1, 1}) we can use the 
             * "ToKnownBestFConvergence" with a tolerance of 0.0001. */
            //optMethod.Add(new ToKnownBestFConvergence(0, 0.0001));

              optMethod.Add(new ToKnownBestFConvergence(0, 1076.4951108)); 
            //optMethod.Add(new DeltaXConvergence(1e-6));
            /* Let us start the search from a specific point. */
            double[] xInit = new[] { 1.0, 5 };//,100};
            double[] xStar;

            /* the next line is where the optimization actually occurs. 
             * Note that the Run command requires the "out double[]" as it's
             * first argument. In this way, the optmization can return a single double
             * equal to the value of the optimal objective function, and an optimizer, 
             * a vector of optimal decision variables. There are two other overloads
             * for the Run command. You can simply specify the out vector and nothing
             * else, or you can provide the out vector and the number of decision 
             * variables. */
            var fStar = optMethod.Run(out xStar, xInit);

            /* output is provided from the optimization. Since the optMethod is an object
             * we can probe it following the run to get at important data like how the 
             * process converged. */
            Console.WriteLine("Convergence Declared by " + optMethod.ConvergenceDeclaredByTypeString);
            Console.WriteLine("X* = " + xStar.MakePrintString());
            Console.WriteLine("F* = " + fStar, 1);
            Console.WriteLine("NumEvals = " + optMethod.numEvals);
            /* Since there is no randomness in the process, you should get the following
             * response:
             * No inequalities specified.
             * Convergence Declared by ToKnownBestFConvergence
             * X* = {   1.079   ,  1.159    }
             * F* = 0.00772036716239199
             * NumEvals = 245
             */
            Console.ReadKey();
        }
    }
}