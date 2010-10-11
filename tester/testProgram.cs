using System;
using System.Collections.Generic;
using StarMathLib;
using OptimizationToolbox;

namespace testerNameSpace
{
    partial class testProgram
    {


        static void Main(string[] args)
        {
           // makeAndSaveProblemDefinition();
            readInAndRunTest();
        }

        private static void makeAndSaveProblemDefinition()
        {
            var dsd = new DesignSpaceDescription(64);
            for (int i = 0; i < 64; i++)
                dsd[i] = new VariableDescriptor(-10000, 10000, 0.01);
            var pd = new ProblemDefinition()
            {
                ConvergenceMethods = new List<abstractConvergence>()
                                                      {
            new MaxAgeConvergence(20, 0.000000001),
            new MaxIterationsConvergence(500),
            new MaxDistanceInPopulationConvergence(1)},
                SpaceDescriptor = dsd

            };
            pd.saveProbToXml("../../testPD.xml");
        }
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        static void readInAndRunTest()
        {
            double[] xStar;
            double f;
            //SET TEST PROBLEM HERE----------------
            //These are unconstrained with a single minima of 0 at the origin, 
            //make sure to set  the penalty weight to zero
           // ProblemDefinition pd = ProblemDefinition.openprobFromXml("../../test16variables.xml");
            // ProblemDefinition pd = ProblemDefinition.openprobFromXml("../../test64variables.xml");
            //This is constrained 2-d problem
           ProblemDefinition pd = ProblemDefinition.openprobFromXml("../../test1.xml");

            Console.WriteLine("setup...");
            abstractOptMethod opty;
            //opty = TestPowellsMethod(pd);
            //opty = TestNelderMeadsMethod(pd);
            //opty = TestSQPMethod(pd);
            //opty = TestGRGMethod(pd);
            // opty = TestGeneticAlgorithm(pd);
           // opty = TestRHC(pd);
            //opty = TestXHC(pd);
            opty = TextExhaustiveSearch(pd);
            f = opty.Run(out xStar);
            Console.WriteLine("Convergence Declared by " + opty.ConvergenceDeclaredBy);
            Console.WriteLine("X* = " + StarMath.MakePrintString(xStar));
            Console.WriteLine("F* = " + f.ToString(), 1);
            Console.WriteLine("NumEvals = " + pd.f.numEvals);

            Console.ReadKey();

        }

        private static abstractOptMethod TestGeneticAlgorithm(ProblemDefinition pd)
        {
            SearchIO.verbosity = 5;
            var opty = new GeneticAlgorithm();
            opty.Add(pd);
            opty.Add(new squaredExteriorPenalty(opty, 5));
            opty.Add(new RandomSampling(pd.SpaceDescriptor));
            opty.Add(new GAMutationBitString(pd.SpaceDescriptor,0.4));
            opty.Add(new GACrossoverBitString(pd.SpaceDescriptor));
            opty.Add(new RandomPairwiseCompare(optimize.minimize));
          //  opty.Add(new Elitism(optimize.minimize));

            return opty;
        }

        private static abstractOptMethod TestGRGMethod(ProblemDefinition pd)
        {
            GeneralizedReducedGradientActiveSet opty = new GeneralizedReducedGradientActiveSet();
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(pd);
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));

            SearchIO.verbosity = 5;
            Console.WriteLine("run...");

            return opty;
        }

        private static abstractOptMethod TestSQPMethod(ProblemDefinition pd)
        {
            SequentialQuadraticProgramming opty = new SequentialQuadraticProgramming(true);
            opty.Add(pd);
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));

            SearchIO.verbosity = 5;
            Console.WriteLine("run...");

            return opty;
        }

        private static abstractOptMethod TestNelderMeadsMethod(ProblemDefinition pd)
        {
            NelderMead opty = new NelderMead();
            opty.Add(pd);
            opty.Add(new squaredExteriorPenalty(opty, 10));

            SearchIO.verbosity = 5;
            Console.WriteLine("run...");
            return opty;
        }

        private static abstractOptMethod TestPowellsMethod(ProblemDefinition pd)
        {
            PowellsMethodOptimization opty = new PowellsMethodOptimization();
            opty.Add(pd);
            opty.Add(new ArithmeticMean(0.00001, 5, 500));
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));

            SearchIO.verbosity = 5;
            Console.WriteLine("run...");
            //f = opty.run(pd.xStart, out xStar);
            return opty;
        }

        private static abstractOptMethod TestRHC(ProblemDefinition pd)
        {
            var opty = new HillClimbing();
            opty.Add(pd);
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(new RandomNeighborGenerator(pd.SpaceDescriptor));
            opty.Add(new KeepSingleBest(optimize.minimize));
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));
            SearchIO.verbosity = 5;
            Console.WriteLine("run...");
            //f = opty.run(pd.xStart, out xStar);
            return opty;
        }


        private static abstractOptMethod TestXHC(ProblemDefinition pd)
        {
            var opty = new HillClimbing();
            opty.Add(pd);
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(new ExhaustiveNeighborGenerator(pd.SpaceDescriptor));
            opty.Add(new KeepSingleBest(optimize.minimize));
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));
            SearchIO.verbosity = 5;
            Console.WriteLine("run...");
            //f = opty.run(pd.xStart, out xStar);
            return opty;
        }
        private static abstractOptMethod TextExhaustiveSearch(ProblemDefinition pd)
        {
            var opty = new ExhaustiveSearch(pd.SpaceDescriptor, optimize.minimize);
            opty.Add(pd);
            opty.ConvergenceMethods.Clear();
            return opty;
        }

    }
}
