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
            //GeneralizedReducedGradientActiveSet opty = new GeneralizedReducedGradientActiveSet(true);
            //SequentialQuadraticProgramming opty = new SequentialQuadraticProgramming(true);
            //generalizedReducedGradientSlack opty = new generalizedReducedGradientSlack();
            //var opty = new GradientBasedOptimization(10);
            NelderMead opty = new NelderMead(0.0001, 100, true);
            //NelderMead opty = new NelderMead();

            //opty.Add(new GoldenSection(0.0001, 5)); 
            //opty.Add(new DSCPowell(0.0001, 5, 100));
            opty.Add(new ArithmeticMean(opty,0.001, 5, 100));
            opty.Add(new SteepestDescent());
            //opty.Add(new squaredExteriorPenalty(opty, 1.0));
            opty.Add(new squaredExteriorPenalty(opty, 10.0));

            ProblemDefinition pd = ProblemDefinition.openprobFromXml("../../test1.xml");
            opty.Add(pd);
            SearchIO.verbosity = 1;
            Console.WriteLine("run...");
            //double f = opty.run(x0, out xStar);
            double f = opty.run(2,out xStar);
            Console.WriteLine("X* = " + DoubleCollectionConverter.convert(xStar));
            Console.WriteLine("F* = " + f.ToString(), 2);
            Console.WriteLine("NumEvals = " + pd.f.numEvals);

            Console.ReadKey();

        }
    }
}
