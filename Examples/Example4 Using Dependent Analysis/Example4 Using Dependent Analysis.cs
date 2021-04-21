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
using OptimizationToolbox;
using StarMathLib;

namespace Example4_Using_Dependent_Analysis
{
    internal static class Program
    {
        /* In this final example, we show a real complex engineering application:
         * the optimization of 24 parameters for a six-gear geartrain. One of the 
         * main things to learn from this example is the use of a dependent
         * analysis which is needed by nearly all the constraints and the objective
         * function. Many times, one needs to run a simulation to get data which
         * other functions depend on. This is below. The example, is more about the
         * set-up as opposed to the optimization. I tried Gradient Based, Randon
         * Hill Climbing and Genetic Algorithm. It is unclear what is best to solve this
         * problem. Experience tells me that since there are discrete and continuous
         * variables and the space should be fairly smooth, if not uni-modal - that 
         * some Branch-n-bound combined with GRG or SQP would be best. */

        /* Actually, this constant could be changed to change the whole size of the problem. */
        private const int NumGearPairs = 3;
        /* there are four design variables per gear (and, obviously, two gears per pair):
         * 0. number of teeth (N)
         * 1. pitch (P) or module (m)...gear tooth size
         * 2. face width (F)
         * 3. location variable, z
         * by setting the NumGearPairs to 3, we are create
         * 3 * (2* 4) = 24 variables. */

        /* the following constants are needed to describe the problem completely. */

        private const double gearDensity = 100.0;
        // gearDensity is used for finding the objective function (minimize mass)
        private const double inputSpeed = 100.0;
        private const double outputSpeed = 1000.0;
        private const double speedTol = 0.5;
        // one of the equality constraints is based on assuming a fixed input speed
        // and finding if the candidate's output is equal (or at least within the 
        // allotted tolerance, speedTol) to this outputSpeed value.

        private const double outputTorque = 100.0;
        // the outputTorque is used to find all the forces, torques, and stresses
        // in the gear train. 

        private const double outputX = 2.2;
        private const double outputY = 2.2;
        private const double outputZ = 2.2;
        private const double locationTol = 0.25;
        // an equality constriants seeks to place the output at the 3D location specified
        // by outputX, outputY, and outputZ. It is zero if one is within this tolerance
        // (in inches).

        private const double boxMinX = -5;
        private const double boxMinY = -5;
        private const double boxMinZ = -5;
        private const double boxMaxX = 5;
        private const double boxMaxY = 5;
        private const double boxMaxZ = 5;
        // the entire gear train must fit with a 10x10x10 box


        private const double Nf = 2;
        // Safety Factor
        private const double SFB = 10000;
        // Bending Fatique Strength
        private const double SFC = 20000;
        // Surface Fatigue (wear) Strength)


        private static readonly double[,] inputPosition = new double[,]
                                                              {
                                                                  {1, 0, 0, 0},
                                                                  {0, 1, 0, 0},
                                                                  {0, 0, 1, 0},
                                                                  {0, 0, 0, 1}
                                                              };
        // the input is located at 0,0,0.

        private static readonly double[] ValidPitches = new[]
                                                            {
                                                                2, 2.25, 2.5, 3, 4, 6, 8, 10, 12, 16,
                                                                20, 24, 32, 40, 48, 64, 80, 96, 120,
                                                                150, 200
                                                            };
        // This is the set of valid AGMA gear pitches (in unit of inch^-1).

        private static void Main()
        {
            //var opty = new GradientBasedOptimization();
            //var opty = new HillClimbing();
            var opty = new GeneticAlgorithm(100);

            var numGears = 2 * NumGearPairs;

            /* here is the Dependent Analysis. Take a look at the file/class ForceVelocityPositionAnalysis.cs
             * and notice that it inherits from IDependent Analysis. By adding this to the optimization method 
             * (line 122), we are ensuring that it is called for any new decision variables found in the process.*/
            var FVPAnalysis = new ForceVelocityPositionAnalysis(numGears, outputTorque, inputSpeed, inputPosition);
            opty.Add(FVPAnalysis);

            /* here is the objective function, minimize mass. Note that it will hold a reference to the 
             * ForceVelocityPositionAnalysis so that it can reference it for exact values of diamter. */
            opty.Add(new massObjective(FVPAnalysis, gearDensity));

            /* here is an inequality constraint for fitting within the box described above. Again, it
             * needs to position and diameter information stored in ForceVelocityPositionAnalysis */
            opty.Add(new boundingboxConstraint(FVPAnalysis, boxMinX, boxMaxX, boxMinY, boxMaxY, boxMinZ,
                                               boxMaxZ));

            /* on and on: stress inequality, output Location, output Speed equalities. Details can be found in 
             * http://dx.doi.org/10.1115/DETC2009-86780 */
            opty.Add(new stressConstraint(FVPAnalysis, Nf, SFB, SFC));
            opty.Add(new outputLocationConstraint(FVPAnalysis, locationTol, outputX, outputY, outputZ));
            opty.Add(new outputSpeedConstraint(FVPAnalysis, speedTol, outputSpeed));
            for (var i = 0; i < NumGearPairs - 1; i++)
                // each mating gear pair must have the same pitch. 
                opty.Add(new samePitch(i * 4 + 1, (i + 1) * 4 + 1));

            /******** Set up Design Space *************/
            /* for the GA and the Hill Climbing, a compete discrete space is needed. Face width and
             * location parameters should be continuous though. Consider removing the 800's below
             * when using a mixed optimization method. */
            var dsd = new DesignSpaceDescription(numGears * 4);
            for (var i = 0; i < numGears; i++)
            {
                dsd[4 * i] = new VariableDescriptor(5, 1000, 1.0); // number of teeth: integers between 5 and 1000
                dsd[4 * i + 1] = new VariableDescriptor(ValidPitches); // pitches from AGMA standard 
                dsd[4 * i + 2] = new VariableDescriptor(0, 50, 800); // face width is between 0 and 50 inches
                dsd[4 * i + 3] = new VariableDescriptor(0, 500, 800);//location is either an angle or a length
                // a max of 500 inches is generous
            }
            opty.Add(dsd);
            /******** Set up Optimization *************/
            /* the following mish-mash is similiar to previous project - just trying to find a 
             * combination of methods that'll lead to the optimial optimization algorithm. */
            //abstractSearchDirection searchDirMethod = new SteepestDescent();
            //opty.Add(searchDirMethod);
            //abstractLineSearch lineSearchMethod = new ArithmeticMean(0.0001, 1, 100);
            //opty.Add(lineSearchMethod);
            opty.Add(new LatinHyperCube(dsd, VariablesInScope.BothDiscreteAndReal));
            opty.Add(new GACrossoverBitString(dsd));
            opty.Add(new GAMutationBitString(dsd));
            opty.Add(new PNormProportionalSelection(optimize.minimize, true, 0.7));
            //opty.Add(new RandomNeighborGenerator(dsd,3000));
            //opty.Add(new KeepSingleBest(optimize.minimize));
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(new MaxAgeConvergence(40, 0.001));
            opty.Add(new MaxFnEvalsConvergence(10000));
            opty.Add(new MaxSpanInPopulationConvergence(15));
            double[] xStar;
            Parameters.Verbosity = VerbosityLevels.AboveNormal;
            // this next line is to set the Debug statements from OOOT to the Console.
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            var timer = Stopwatch.StartNew();
            var fStar = opty.Run(out xStar, numGears * 4);
            printResults(opty, xStar, fStar, timer);
            Console.ReadKey();
        }

        private static void printResults(abstractOptMethod opty, double[] xStar, double f, Stopwatch timer)
        {
            Console.WriteLine("Completed running " + opty.GetType());
            Console.WriteLine("Convergence Declared by " + opty.ConvergenceDeclaredByTypeString);
            Console.WriteLine("X* = " + xStar.MakePrintString());
            Console.WriteLine("F* = " + f, 1);
            Console.WriteLine("NumEvals = " + opty.numEvals);
            Console.WriteLine("The time taken by the process = " + timer.Elapsed + ".\n\n\n");
        }
    }
}