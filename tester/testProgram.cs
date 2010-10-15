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
using OptimizationToolbox;
using StarMathLib;

namespace testerNameSpace
{
    internal class testProgram
    {
        private static void Main()
        {
            // makeAndSaveProblemDefinition();
            readInAndRunTest();
        }

        private static void makeAndSaveProblemDefinition()
        {
            var dsd = new DesignSpaceDescription(64);
            for (var i = 0; i < 64; i++)
                dsd[i] = new VariableDescriptor(-10000, 10000, 0.01);
            var pd = new ProblemDefinition
                         {
                             ConvergenceMethods = new List<abstractConvergence>
                                                      {
                                                          new MaxAgeConvergence(20, 0.000000001),
                                                          new MaxIterationsConvergence(500),
                                                          new MaxDistanceInPopulationConvergence(1)
                                                      },
                             SpaceDescriptor = dsd
                         };
            pd.saveProbToXml("../../testPD.xml");
        }

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        private static void readInAndRunTest()
        {
            double[] xStar;
            //SET TEST PROBLEM HERE----------------
            //These are unconstrained with a single minima of 0 at the origin, 
            //make sure to set  the penalty weight to zero
            //ProblemDefinition pd = ProblemDefinition.openprobFromXml("../../test16variables.xml");
            // ProblemDefinition pd = ProblemDefinition.openprobFromXml("../../test64variables.xml");
            //This is constrained 2-d problem
            var pd = ProblemDefinition.openprobFromXml("../../test1.xml");

            Console.WriteLine("setup...");
            abstractOptMethod opty;
            opty = TestNelderMeadsMethod(pd);
            //opty = TestSQPMethod(pd);
            //opty = TestGRGMethod(pd);
            //opty = TestGeneticAlgorithm(pd);
            //opty = TestRHC(pd);
            //opty = TestXHC(pd);
            // opty = TestExhaustiveSearch(pd);
            // opty = TestGradientBased(pd);
            // opty = TestSimulatedAnnealing(pd);
            SearchIO.verbosity = 1;
            var f = opty.Run(out xStar);
            Console.WriteLine("Convergence Declared by " + opty.ConvergenceDeclaredBy);
            Console.WriteLine("X* = " + StarMath.MakePrintString(xStar));
            Console.WriteLine("F* = " + f, 1);
            Console.WriteLine("NumEvals = " + pd.f.numEvals);

            Console.ReadKey();
        }

        private static abstractOptMethod TestSimulatedAnnealing(ProblemDefinition pd)
        {
            var opty = new SimulatedAnnealing(optimize.minimize);
            opty.Add(pd);
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(new RandomNeighborGenerator(pd.SpaceDescriptor, 100));
            opty.Add(new SACoolingSangiovanniVincentelli(100));
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));
            return opty;
        }

        private static abstractOptMethod TestGradientBased(ProblemDefinition pd)
        {
            var opty = new GradientBasedOptimization();
            opty.Add(pd);
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(new CyclicCoordinates());
            //opty.Add(new FletcherReevesDirection());
            //opty.Add(new SteepestDescent());
            //opty.Add(new BFGSDirection());
            //opty.Add(new PowellMethod(pd.SpaceDescriptor.n, 0.001, 6));
            opty.Add(new ArithmeticMean(0.0001, 0.1, 100));
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));
            return opty;
        }

        private static abstractOptMethod TestGeneticAlgorithm(ProblemDefinition pd)
        {
            var opty = new GeneticAlgorithm();
            opty.Add(pd);
            opty.Add(new squaredExteriorPenalty(opty, 50));
            opty.Add(new LatinHyperCube(pd.SpaceDescriptor, VariablesInScope.BothDiscreteAndReal));
            opty.Add(new GAMutationBitString(pd.SpaceDescriptor, 0.4));
            opty.Add(new GACrossoverBitString(pd.SpaceDescriptor));
            opty.Add(new RandomPairwiseCompare(optimize.minimize));
            //  opty.Add(new Elitism(optimize.minimize));
            return opty;
        }

        private static abstractOptMethod TestGRGMethod(ProblemDefinition pd)
        {
            var opty = new GeneralizedReducedGradientActiveSet();
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(pd);
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));
            return opty;
        }

        private static abstractOptMethod TestSQPMethod(ProblemDefinition pd)
        {
            var opty = new SequentialQuadraticProgramming();
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(pd);
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));
            return opty;
        }

        private static abstractOptMethod TestNelderMeadsMethod(ProblemDefinition pd)
        {
            var opty = new NelderMead();
            opty.Add(pd);
            opty.Add(new squaredExteriorPenalty(opty, 10));
            return opty;
        }

        private static abstractOptMethod TestPowellsMethod(ProblemDefinition pd)
        {
            var opty = new PowellsMethodOptimization();
            opty.Add(pd);
            opty.Add(new ArithmeticMean(0.00001, 5, 500));
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.ConvergenceMethods.RemoveAll(a => typeof(MaxDistanceInPopulationConvergence).IsInstanceOfType(a));
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
            return opty;
        }

        private static abstractOptMethod TestExhaustiveSearch(ProblemDefinition pd)
        {
            var opty = new ExhaustiveSearch(pd.SpaceDescriptor, optimize.minimize);
            opty.Add(pd);
            opty.ConvergenceMethods.Clear();
            return opty;
        }
    }
}