using System;
using System.Diagnostics;
using OptimizationToolbox;
using StarMathLib;

namespace Example2_Genetic_Algorithm
{
    class Program
    {
        private static void printResults(abstractOptMethod opty, double[] xStar, double f, Stopwatch timer)
        {
            Console.WriteLine("Completed running " + opty.GetType());
            Console.WriteLine("Convergence Declared by " + opty.ConvergenceDeclaredByTypeString);
            Console.WriteLine("X* = " + StarMath.MakePrintString(xStar));
            Console.WriteLine("F* = " + f, 1);
            Console.WriteLine("NumEvals = " + opty.numEvals);
            Console.WriteLine("The time taken by the process = " + timer.Elapsed + ".\n\n\n");
            Console.ReadLine();
        }
        static void Main()
        {
            Parameters.Verbosity = VerbosityLevels.AboveNormal;
            // this next line is to set the Debug statements from OOOT to the Console.
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            /* first a new optimization method in the form of a genetic algorithm is created. */
            var optMethod = new GeneticAlgorithm(100);
            /* then an objective function and constraints are added. Since these inherit from
             * type OptimizationToolbox.objectiveFunction and OptimizationToolbox.inequality
             * the Add function knows where to store them so that they will be invoked by the
             * fitness evaluation section of the GA. */
            optMethod.Add(new efficiencyMeasurement());

            //optMethod.Add(new lessThanManifoldVolume());
            /* GA's cannot explicitly handle inequalities, so a merit function must be added
             * so that the fitness evaluation knows how to combine the constraint with the 
             * objective function. */
            //optMethod.Add(new squaredExteriorPenalty(optMethod, 50));
            /* Now a number of convergerence criteria are added. Again, since these all 
             * inherit from the abstractConvergence class, the Add method knows to where 
             * to store them. */
            optMethod.Add(new ToKnownBestFConvergence(0.0, 0.5));
            optMethod.Add(new MaxIterationsConvergence(50000)); /* stop after 500 iteration (i.e. generations) */
            optMethod.Add(new MaxAgeConvergence(20, 0.000000001)); /*stop after 20 generations of the best not changing */
            optMethod.Add(new MaxSpanInPopulationConvergence(100)); /*stop if the largest distance is only one unit. */
            optMethod.NumConvergeCriteriaNeeded = 2; /* two of these three criteria are needed to stop the process. */

            /* The genetic algorithm is for discrete problems. Therefore we need to provide the optimization algorithm
             * and the subsequent generators with the details of the space. The first variable represents the number of
             * passes in our fictitious problem. We set the lower bound to 1 and the upper bound to 20. The third argument
             * is the delta and since only integers are possible we set this to 1. The second and third variables are
             * really continous, but for the purpose of the GA we set a discretization at one-ten-thousandth for the second
             * and one-hundredth in the third. Note that you can provide either the delta or the number of steps. Here
             * 36,001 steps will make increments of one-hundredth. */
            var SpaceDescriptor = new DesignSpaceDescription
                                      {
                                          new VariableDescriptor(1, 20, 1.0),
                                          new VariableDescriptor(0, 100, 0.0001),
                                          new VariableDescriptor(-180, 180, 36000)
                                      };
            optMethod.Add(SpaceDescriptor);
            /* the genetic algorithm requires some more values to be fully specified. These include initial, 
            * crossover, and mutation generators, as well as a selector. A Latin Hyper Cube initial sample is
            * first created to assure the population covers the space well. */
            optMethod.Add(new LatinHyperCube(SpaceDescriptor, VariablesInScope.BothDiscreteAndReal));
            /* the typical bit-string approach to mutation and crossover are adopted here. Note that the
             * mutation rate (per candidate) is increased to 0.4 from the default of 0.1. Which means that
             * 4 in 10 candidates should experience at least one mutation. No new crossover rate is provided
             * therefore the default of 1.7 will be used. This means that between two parents there will likely 
             * be 1.7 locations of crossover between them. */
            optMethod.Add(new GAMutationBitString(SpaceDescriptor, 0.4));
            optMethod.Add(new GACrossoverBitString(SpaceDescriptor));

            /* Finally, the selector is added to the population. This RandomPairwiseCompare is often referred to
             * as tournament selection wherein a random selection of two candidates results in the inferior one
             * being removed from the population. It requires the optimization direction: are lower values better
             * (minimize) or larger (maximize)? */
            optMethod.Add(new RandomPairwiseCompare(optimize.minimize));

            /* for output statements (points in the code where the SearchIO.output(...) function is called, the
             * verbosity is set to 4 which is high. Typical values are between 0 and 4 but higher values (>4)
             * may be used, but this will likely cut into the speed of the search process. */
            Parameters.Verbosity = VerbosityLevels.AboveNormal;
            // this next line is to set the Debug statements from OOOT to the Console.
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

            var timer = Stopwatch.StartNew();
            /* everything is set, we can now run the algorithm and retrieve the f* and x* values. */
            double[] xOptimal;
            var fOptimal = optMethod.Run(out xOptimal);

            printResults(optMethod, xOptimal, fOptimal, timer);

        }
    }
}
