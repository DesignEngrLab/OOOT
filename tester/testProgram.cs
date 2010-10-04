using System;
using OptimizationToolbox;

namespace testerNameSpace
{
    class testProgram
    {
        static void Main(string[] args)
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
            opty.Add(new DSCPowell(opty, 0.0001, 5, 100));
            opty.Add(new ArithmeticMean(opty, 0.001, 5, 100));
            opty.Add(new SteepestDescent());
            //opty.Add(new squaredExteriorPenalty(opty, 1.0));
            opty.Add(new squaredExteriorPenalty(opty, 1000.0));
            opty.Add(new DeltaXConvergence(0.001));
            opty.Add(new NelderMeadConvergence(1000, 50, 0.00000001, 0.00000000));
            var pd = ProblemDefinition.openprobFromXml("../../test01.xml");
            opty.Add(pd);
            SearchIO.verbosity = 10;
            Console.WriteLine("run...");
            //double f = opty.run(x0, out xStar);
            double f = opty.run(2, out xStar);
            Console.WriteLine("X* = " + DoubleCollectionConverter.convert(xStar));
            Console.WriteLine("F* = " + f.ToString(), 2);
            Console.WriteLine("NumEvals = " + pd.f.numEvals);

            Console.ReadKey();

        }
    }
}
