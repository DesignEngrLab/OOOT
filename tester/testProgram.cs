using System;
using StarMathLib;
using OptimizationToolbox;

namespace testerNameSpace
{
    partial class testProgram
    {
        enum optMethodType { NelderMead, Powell, SQP, Default };
        static void Main(string[] args)
        {
            ///SET METHOD TO TEST HERE-------------
            optMethodType optMethod = optMethodType.Powell;

            double[] xStar;
            double f;
            //SET TEST PROBLEM HERE----------------
            //These are unconstrained with a single minima of 0 at the origin, 
            //make sure to set  the penalty weight to zero
            //ProblemDefinition pd = ProblemDefinition.openprobFromXml("../../test16variables.xml");
            ProblemDefinition pd = ProblemDefinition.openprobFromXml("../../test64variables.xml");
            //This is constrained 2-d problem
            //ProblemDefinition pd = ProblemDefinition.openprobFromXml("../../test1.xml");

            Console.WriteLine("setup...");
            if (optMethod == optMethodType.Powell)
            {
                PowellsMethodOptimization opty = new PowellsMethodOptimization(1);
                opty.Add(new MaxIterationsConvergence(100));
                opty.Add(new DeltaFConvergence(0.00001));
                opty.Add(new DeltaXConvergence(0.00001));

                //opty.Add(new GoldenSection(opty, 0.001, 5, 100)); 
                //opty.Add(new DSCPowell(0.0001, 5, 100));
                opty.Add(new ArithmeticMean(0.00001, 5, 500));

                opty.Add(new squaredExteriorPenalty(opty, 0 * Math.Pow(10, 5)));
                //opty.Add(new linearExteriorPenalty(opty, 1.0));

                opty.Add(pd);
                SearchIO.verbosity = 5;
                Console.WriteLine("run...");
                //f = opty.run(pd.xStart, out xStar);
                f = opty.Run(out xStar);
            }
            else if (optMethod == optMethodType.NelderMead)
            {
                NelderMead opty = new NelderMead();
                //NelderMead opty = new NelderMead();
                opty.Add(new MaxAgeConvergence(20,0.000001));
                opty.Add(new MaxIterationsConvergence(50000));
                // what is this for? System.Convert.ToInt32(Math.Pow(pd.xStart.GetLength(0), 1.15)), 1 * Math.Pow(10, -5));
                opty.Add(new squaredExteriorPenalty(opty, 0 * Math.Pow(10, 5)));
                //opty.Add(new linearExteriorPenalty(opty, 1.0));

                opty.Add(pd);
                SearchIO.verbosity = 1;
                Console.WriteLine("run...");
                //double f = opty.run(x0, out xStar);
                f = opty.Run(out xStar);
            }
            else if (optMethod == optMethodType.SQP)
            {
                SequentialQuadraticProgramming opty = new SequentialQuadraticProgramming(true);
                //opty.Add(new squaredExteriorPenalty(opty, 5 * Math.Pow(10, 5)));
                //opty.Add(new linearExteriorPenalty(opty, 1.0));

                opty.Add(new MaxIterationsConvergence(100));
                opty.Add(new DeltaFConvergence(0.00001));
                opty.Add(new DeltaXConvergence(0.00001));
                opty.Add(pd);
                SearchIO.verbosity = 5;
                Console.WriteLine("run...");
                //double f = opty.run(x0, out xStar);
                f = opty.Run(out xStar);
            }
            else
            {
                GeneralizedReducedGradientActiveSet opty = new GeneralizedReducedGradientActiveSet(true);
                //generalizedReducedGradientSlack opty = new generalizedReducedGradientSlack();
                //GradientBasedUnconstrained opty = new GradientBasedUnconstrained(10);
                opty.Add(new squaredExteriorPenalty(opty, 5 * Math.Pow(10, 5)));
                //opty.Add(new linearExteriorPenalty(opty, 1.0));

                opty.Add(pd);
                SearchIO.verbosity = 5;
                Console.WriteLine("run...");
                //double f = opty.run(x0, out xStar);
                f = opty.Run(out xStar);
            }
            //opty.Add(new SteepestDescent());
            Console.WriteLine("X* = " + StarMath.MakePrintString(xStar));
            Console.WriteLine("F* = " + f.ToString(), 1);
            Console.WriteLine("NumEvals = " + pd.f.numEvals);

            Console.ReadKey();

        }
    }
}
