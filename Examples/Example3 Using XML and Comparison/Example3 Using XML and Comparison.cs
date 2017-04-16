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
using System.Diagnostics;
using System.IO;
using OptimizationToolbox;
using StarMathLib;

namespace Example3_Using_XML_and_Comparison
{
    class Program
    {
        private const string filename = "../../../test1.xml";
        private static void Main()
        {
            Parameters.Verbosity = VerbosityLevels.AboveNormal;
            // this next line is to set the Debug statements from OOOT to the Console.
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            /* In this example, we first present how the details of an optimzation
             * problem can be saved to an XML-file so that it can be read in 
             * and solved as opposed to defining all the details in an imperative
             * (code line by code line) way. In the first function, the xml file
             * name "test1.xml" is created. */
            makeAndSaveProblemDefinition();

            /* now we create a series of different optimization methods and test
             * them on the problem. The problem is now opened from the file and
             * the details are stored in an object of class "Problem Definition".*/
            var stream = new FileStream(filename, FileMode.Open);


            ProblemDefinition probTest1 = ProblemDefinition.OpenprobFromXml(stream);
            abstractOptMethod opty;


            /***********Gradient Based Optimization with Steepest Descent****************/
            SearchIO.output("***********Gradient Based Optimization with Steepest Descent****************");
            opty = new GradientBasedOptimization();
            opty.Add(probTest1);
            abstractSearchDirection searchDirMethod = new SteepestDescent();
            opty.Add(searchDirMethod);
            //abstractLineSearch lineSearchMethod = new ArithmeticMean(0.0001, 1, 100);
            //abstractLineSearch lineSearchMethod = new DSCPowell(0.0001, 1, 100);
            abstractLineSearch lineSearchMethod = new GoldenSection(0.0001, 1);
            opty.Add(lineSearchMethod);
            opty.Add(new squaredExteriorPenalty(opty, 10));
            /* since this is not a population-based optimization method, we need to remove the MaxSpan criteria. */
            opty.ConvergenceMethods.RemoveAll(a => a is MaxSpanInPopulationConvergence);

            double[] xStar;
            var timer = Stopwatch.StartNew();
            var fStar = opty.Run(out xStar);
            printResults(opty, xStar, fStar, timer);

            /***********Gradient Based Optimization with Fletcher-Reeves****************/
            SearchIO.output("***********Gradient Based Optimization with Fletcher-Reeves****************");
            /* we don't need to reset (invoke the constructor) for GradientBasedOptimization since we are only 
             * change the seaach direction method. */
            searchDirMethod = new FletcherReevesDirection();
            /* you could also try the remaining 3 search direction methods. */
            //searchDirMethod = new CyclicCoordinates();
            //searchDirMethod = new BFGSDirection();
            //searchDirMethod = new PowellMethod(0.001, 6);
            opty.Add(searchDirMethod);

            timer = Stopwatch.StartNew();
            opty.ResetFunctionEvaluationDatabase();
            fStar = opty.Run(out xStar);
            printResults(opty, xStar, fStar, timer);
            /******************Generalized Reduced Gradient***********************/
            SearchIO.output("******************Generalized Reduced Gradient***********************");
            opty = new GeneralizedReducedGradientActiveSet();
            opty.Add(probTest1);
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.ConvergenceMethods.RemoveAll(a => a is MaxSpanInPopulationConvergence);

            timer = Stopwatch.StartNew();
            fStar = opty.Run(out xStar);
            printResults(opty, xStar, fStar, timer);
            /* GRG is the ONLY one here that handles constraints explicity. It find the
             * optimal very quickly and accurately. However, many of the other show a 
             * better value of f*, this is because they are using an imperfect penalty
             * function (new squaredExteriorPenalty(opty, 10)). While it seems that GRG
             * includes it as well, it is only used in the the line search method. */


            /******************Random Hill Climbing ***********************/
            SearchIO.output("******************Random Hill Climbing ***********************");
            opty = new HillClimbing();
            opty.Add(probTest1);
            opty.Add(new squaredExteriorPenalty(opty, 8));
            opty.Add(new RandomNeighborGenerator(probTest1.SpaceDescriptor));
            opty.Add(new KeepSingleBest(optimize.minimize));
            opty.ConvergenceMethods.RemoveAll(a => a is MaxSpanInPopulationConvergence);
            /* the deltaX convergence needs to be removed as well, since RHC will end many iterations
             * at the same point it started. */
            opty.ConvergenceMethods.RemoveAll(a => a is DeltaXConvergence);

            timer = Stopwatch.StartNew();
            fStar = opty.Run(out xStar);
            printResults(opty, xStar, fStar, timer);




            /******************Exhaustive Hill Climbing ***********************/
            SearchIO.output("******************Exhaustive Hill Climbing ***********************");
            /* Everything else about the Random Hill Climbing stays the same. */
            opty.Add(new ExhaustiveNeighborGenerator(probTest1.SpaceDescriptor));

            timer = Stopwatch.StartNew();
            fStar = opty.Run(out xStar);
            printResults(opty, xStar, fStar, timer);



            /******************Simulated Annealing***********************/
            SearchIO.output("******************Simulated Annealing***********************");
            opty = new SimulatedAnnealing(optimize.minimize);
            opty.Add(probTest1);
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(new RandomNeighborGenerator(probTest1.SpaceDescriptor, 100));
            opty.Add(new SACoolingSangiovanniVincentelli(100));
            opty.ConvergenceMethods.RemoveAll(a => a is MaxSpanInPopulationConvergence);
            /* the deltaX convergence needs to be removed as well, since RHC will end many iterations
             * at the same point it started. */
            opty.ConvergenceMethods.RemoveAll(a => a is DeltaXConvergence);


            timer = Stopwatch.StartNew();
            fStar = opty.Run(out xStar);
            printResults(opty, xStar, fStar, timer);


            /******************Exhaustive Search ***********************/
            // SearchIO.output("******************Exhaustive Search ***********************");
            //opty = new ExhaustiveSearch(probTest1.SpaceDescriptor, optimize.minimize);
            //opty.Add(probTest1);
            /* No convergence criteria is needed as the process concludes when all
             * states have been visited but for this problem that is 4 trillion states.*/
            //opty.ConvergenceMethods.Clear();
            /* if you DID KNOW the best, you can include a criteria like...*/
            //opty.ConvergenceMethods.Add(new ToKnownBestXConvergence(new[] { 3.0, 3.0 }, 0.0000001));
            //timer = Stopwatch.StartNew();
            //fStar = opty.Run(out xStar);

            /* you probably will never see this process complete. Even with the added
             * convergence criteria (which is not factored into the estimated time of
             * completion), you are probably looking at 1 to 2 years. */
            //printResults(opty, xStar, fStar, timer);

        }

        private static void printResults(abstractOptMethod opty, double[] xStar, double f, Stopwatch timer)
        {
            Console.WriteLine("Completed running " + opty.GetType());
            Console.WriteLine("Convergence Declared by " + opty.ConvergenceDeclaredByTypeString);
            Console.WriteLine("X* = " + xStar.MakePrintString());
            Console.WriteLine("F* = " + f, 1);
            Console.WriteLine("NumEvals = " + opty.numEvals);
            Console.WriteLine("The time taken by the process = " + timer.Elapsed + ".\n\n\n");
            Console.ReadLine();
        }

        private static void makeAndSaveProblemDefinition()
        {
            var pd = new ProblemDefinition();

            /* Add a design space descriptor so that optimizatoin
             * methods for discrete variables can be used. Here we 
             * make a very generous discretization, which amounts
             * to 2 million steps in each of the 2 design variables. */
            var dsd = new DesignSpaceDescription(2);
            for (var i = 0; i < 2; i++)
                dsd[i] = new VariableDescriptor(-5000, 5000, 1.0);
            pd.Add(dsd);

            /* Add three convergence criteria */
            pd.Add(new DeltaXConvergence(0.0001));
            pd.Add(new MaxAgeConvergence(100, 0.000000001));
            pd.Add(new MaxFnEvalsConvergence(50000));
            pd.Add(new MaxSpanInPopulationConvergence(1));

            /* setting the number of convergence criteria needed is not necessary
             * since we will be using the default value of 1. Interesting to un-
             * comment the next line and see how it affects the process. */
            //pd.NumConvergeCriteriaNeeded = 2;

            /* Add the objective function. */
            var objfn = new polynomialObjFn();
            objfn.Add("x1^2");
            objfn.Add("x2^2");
            objfn.Add("-2*x1");
            objfn.Add("-10*x2");
            objfn.Add("26");
            /* this is a simple parabola center at {1, 5} */
            pd.Add(objfn);

            var g1 = new polynomialInequality();
            g1.Add("-x1");
            g1.Add("x2"); /* this inequality translates to x2 - x1 < 0
                           * of simply x1 > x2. */
            pd.Add(g1);

            pd.Add(new double[] { 1500.0, 700.0 });
            var stream = new FileStream(filename, FileMode.Create);
            pd.SaveProbToXml(stream);
        }
    }
}