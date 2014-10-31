using System;
using OptimizationToolbox;
using OptimizationToolbox.Selector_Methods;
using StarMathLib;
using System.Collections.Generic;

namespace Example5_MOGA
{
    class Program
    {
        static void Main()
        {
            /* first a new optimization method in the form of a genetic algorithm is created. */
            var optMethod = new MultiObjectiveGeneticAlgorithm();
            /* The objective function is Rosenbrock's banana function again. */
            optMethod.Add(new polynomialObjFn
                              {
                Terms = new List<string>
                {
                    "100*x1^4",
                    "-200*x1^2*x2",
                    "x1^2",
                    "-2*x1",
                    "100*x2^2",
                    "1"
                }
            });

            optMethod.Add(new RoyalRoads());

            /* Now a number of convergerence criteria are added. Again, since these all 
             * inherit from the abstractConvergence class, the Add method knows to where 
             * to store them. */
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
                                          new VariableDescriptor(-100, 100, 0.0001),
                                          new VariableDescriptor(-100, 100, 0.0001),
                                          new VariableDescriptor(-100, 100, 0.0001)
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
            optMethod.Add(new SkewboidDiversity( optimize.minimize, optimize.minimize ));
            optMethod.Add(new RandomPairwiseCompare(optimize.minimize));

            /* for output statements (points in the code where the SearchIO.output(...) function is called, the
             * verbosity is set to 4 which is high. Typical values are between 0 and 4 but higher values (>4)
             * may be used, but this will likely cut into the speed of the search process. */
            SearchIO.verbosity = 4;

            /* everything is set, we can now run the algorithm and retrieve the f* and x* values. */
            double[] xOptimal;
            var fOptimal = optMethod.Run(out xOptimal);

            /* since we are curious how the process completed we now output some details. */
            SearchIO.output("f* = " + fOptimal); /* the 0 indicates that this statement has high priority
                                                     * and shouldn't be skipped in printing to the console. */
            SearchIO.output("x* = " + StarMath.MakePrintString(xOptimal));
            SearchIO.output("The process converged by criteria: " + optMethod.ConvergenceDeclaredByTypeString);
            Console.ReadLine();
        }
    }
}
