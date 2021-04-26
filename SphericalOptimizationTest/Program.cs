using System;
using System.Linq;
using StarMathLib;
using TVGL;
using OptimizationToolbox;
using System.Diagnostics;
using System.Collections.Generic;

namespace SphericalOptimization
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Parameters.Verbosity = OptimizationToolbox.VerbosityLevels.Everything;
            // this next line is to set the Debug statements from OOOT to the Console.
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            //var optMethod = new NelderMead();
            var optMethod = new SphericalOptimization();

            TVGL.IOFunctions.IO.Open(@"C:\Users\campmatt\source\repos\medemaMVP\TestFiles\TopOp\Part2.stl", out TessellatedSolid ts);
            //ts.Transform(TVGL.Numerics.Matrix4x4.CreateRotationY(1.5));
            //ts.Transform(TVGL.Numerics.Matrix4x4.CreateRotationZ(1.25));
            optMethod.Add(new FloorAndWallFaceScore(ts));
            optMethod.Add(new DeltaXConvergence(1e-6));
            /* Let us start the search from a specific point. */
            double[] xInit = new[] { 0.0, 0.0 };//,100};
            var fStar = optMethod.Run(out var xStar, xInit);
            PlotDirections(optMethod.vertices.Keys, optMethod.vertices.Values, ts);
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
        static void PlotDirections(IList<double> keys, IList<double[]> values, TessellatedSolid ts)
        {
            var numToShow = keys.Count;
            var max = keys[0];
            var rank = Enumerable.Range(0, keys.Count);
            var rankFraction = rank.Select(x => x / (double)numToShow).ToList();
            //rankFraction.Reverse();
            var plotVertices = new List<Vertex>();
            var colors = new List<TVGL.Color>();
            for (int i = 0; i < numToShow; i++)
            {
                plotVertices.Add(new Vertex(0.03*(19+(keys[i]/max)) * (new TVGL.Numerics.Vector3(values[i]))));
                colors.Add(TVGL.Color.HSVtoRGB((float)rankFraction[i], 1, 1));
                //colors.Add(TVGL.Color.HSVtoRGB((float)(keys[i]/max), 1, 1));
                Console.WriteLine(keys[i]);
            }
            Presenter.ShowGaussSphereWithIntensity(plotVertices, colors, ts);
        }
    }
}



