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
        private const int NumGearPairs = 3;
        /* there are four design variables per gear:
         * 0. number of teeth (N)
         * 1. pitch (P) or module (m)...gear tooth size
         * 2. face width (F)
         * 3. location variable, z
         * by setting the NumGearPairs to 3, we are create
         * 3 * (2* 4) = 24 variables. */

        private const double gearDensity = 100.0;
        private const double inputSpeed = 100.0;
        private const double outputSpeed = 100.0;
        private const double speedTol = 0.5;
        private const double outputTorque = 100.0;

        private const double outputX = 2.2;
        private const double outputY = 2.2;
        private const double outputZ = 2.2;
        private const double locationTol = 0.0001;

        private const double boxMinX = 0;
        private const double boxMinY = 0;
        private const double boxMinZ = 0;
        private const double boxMaxX = 10;
        private const double boxMaxY = 10;
        private const double boxMaxZ = 10;


        private const double Nf = 2;
        private const double SFB = 10000;
        private const double SFC = 20000;


        private static readonly double[,] inputPosition = new double[,]
                                                              {
                                                                  {1, 0, 0, 0},
                                                                  {0, 1,0,0},
                                                                  {0,0,1,0},
                                                                  {0,0,0,1}
                                                              };

        private static double[] ValidPitches = new[]
                                            {
                                                2, 2.25, 2.5, 3, 4, 6, 8, 10, 12, 16, 
                                                20, 24, 32, 40, 48, 64, 80, 96, 120,
                                                150, 200
                                            };

        private static void Main()
        {
            var opty = new GradientBasedOptimization();
            var numGears = 2 * NumGearPairs;
            var FVPAnalysis = new ForceVelocityPositionAnalysis(numGears, outputTorque, inputSpeed, inputPosition);
            opty.Add(FVPAnalysis);
            opty.Add(new massObjective(FVPAnalysis, gearDensity));
            opty.Add(new boundingboxConstraint(FVPAnalysis, boxMinX, boxMaxX, boxMinY, boxMaxY, boxMinZ,
                                               boxMaxZ));
            opty.Add(new stressConstraint(FVPAnalysis, Nf, SFB, SFC));
            opty.Add(new outputLocationConstraint(FVPAnalysis, locationTol, outputX, outputY, outputZ));
            opty.Add(new outputSpeedConstraint(FVPAnalysis, speedTol, outputSpeed));
            for (var i = 0; i < NumGearPairs - 1; i++)
                opty.Add(new samePitch(i * 4 + 1, (i + 1) * 4 + 1));

            /******** Set up Design Space *************/
            var dsd = new DesignSpaceDescription(numGears*4);
            for(int i=0; i<numGears;i++)
            {
                dsd[4*i] = new VariableDescriptor(5, 1000, 1.0);// number of teeth: integers between 5 and 1000
                dsd[4 * i + 1] = new VariableDescriptor(ValidPitches); // pitches from AGMA standard 
                dsd[4*i + 2] = new VariableDescriptor(0, 50); // face width is between 0 and 50 inches
              }
            opty.Add(dsd);
            /******** Set up Optimization *************/
            abstractSearchDirection searchDirMethod = new SteepestDescent();
            opty.Add(searchDirMethod);
            abstractLineSearch lineSearchMethod = new ArithmeticMean(0.0001, 1, 100);
            opty.Add(lineSearchMethod);
            opty.Add(new squaredExteriorPenalty(opty, 10));
            opty.Add(new DeltaXConvergence(0.0001));
            opty.Add(new MaxIterationsConvergence(50000));
            double[] xStar;
            SearchIO.verbosity = 4;
            var timer = Stopwatch.StartNew();
            var fStar = opty.Run(out xStar,numGears*4);
            printResults(opty, xStar, fStar, timer);
            Console.ReadKey();
        }

        private static void printResults(abstractOptMethod opty, double[] xStar, double f, Stopwatch timer)
        {
            Console.WriteLine("Completed running " + opty.GetType());
            Console.WriteLine("Convergence Declared by " + opty.ConvergenceDeclaredByTypeString);
            Console.WriteLine("X* = " + StarMath.MakePrintString(xStar));
            Console.WriteLine("F* = " + f, 1);
            Console.WriteLine("NumEvals = " + opty.numEvals);
            Console.WriteLine("The time taken by the process = " + timer.Elapsed + ".\n\n\n");

        }
    }
}