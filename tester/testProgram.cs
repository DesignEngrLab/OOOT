using System;
using System.Collections.Generic;
using OptimizationToolbox;
using StarMathLib;

namespace testerNameSpace
{
    class testProgram
    {
        static void Main(string[] args)
        {
            makeAndSaveProblemDefinition();
             readAndRunTest1();
        }

        private static void makeAndSaveProblemDefinition()
        {
            var pd = new ProblemDefinition()
                         {
                             ConvergenceMethods = new List<abstractConvergence>()
                                                      {
                                                          new MaxIterationsConvergence(1000),
                                                          new NelderMeadConvergence(9999,20, 0.0000000001, 0.000001)
                                                      }
                                                   
                         };
            pd.saveProbToXml("../../testPD.xml");
        }

        private static void readAndRunTest1()
        {
            double[] xStar;
            Console.WriteLine("setup...");
            abstractOptMethod opty;
            opty = new GeneralizedReducedGradientActiveSet(true);
            //opty = new SequentialQuadraticProgramming(true);
            //opty = new GeneralizedReducedGradientSlack();
            //opty = new GradientBasedOptimization();
            opty = new NelderMead();

            // opty.Add(new GoldenSection(0.0001, 5)); 
            opty.Add(new DSCPowell(0.0001, 5, 100));
            opty.Add(new ArithmeticMean(0.001, 5, 100));
            opty.Add(new SteepestDescent());
            //opty.Add(new squaredExteriorPenalty(opty, 1.0));
            opty.Add(new squaredExteriorPenalty(opty, 1000.0));
            opty.Add(new DeltaXConvergence(0.001));
            opty.Add(new NelderMeadConvergence(1000, 50, 0.00000001, 0.00000000));
            var pd = ProblemDefinition.openprobFromXml("../../test1.xml");
            opty.Add(pd);
            SearchIO.verbosity = 5;
            Console.WriteLine("run...");
            //double f = opty.run(x0, out xStar);
            double f = opty.Run(out xStar);
            Console.WriteLine("X* = " + StarMath.MakePrintString(xStar));
            Console.WriteLine("F* = " + f.ToString(), 2);
            Console.WriteLine("NumEvals = " + pd.f.numEvals);

            Console.ReadKey();
        }
    }
}
