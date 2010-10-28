using System;
using OptimizationToolbox;

namespace Example4_Using_Dependent_Analysis
{
    internal class stressConstraint : IInequality
    {
        const int Qv = 9;
        const double J = .25;
        //var   J = .30;   //table value? 
        const double Km = 1.6;
        const int Ka = 1;
        const int Ks = 1;
        const int Kb = 1;
        const int Ki = 1;
        const double I = 0.1;
        const int Cf = 1;
        const int Cp = 1000;

        private readonly ForceVelocityPositionAnalysis FVPAnalysis;
        private readonly double Nf, SFB, SFC;

        #region Constructor

        public stressConstraint(ForceVelocityPositionAnalysis fvpAnalysis, double Nf, double SFB, double SFC)
        {
            FVPAnalysis = fvpAnalysis;
            this.Nf = Nf;
            this.SFB = SFB;
            this.SFC = SFC;
        }

        #endregion

        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            var fatique = new double[FVPAnalysis.numGears];
            var wear = new double[FVPAnalysis.numGears];
            
            for (var i = 0; i < FVPAnalysis.numGears; i++)
            {
                //var N = x[i*4]; /*number of teeth is not needed, well it could
                /*be used to better estimate J, but simplified here. */

                var pd = x[i * 4 + 1];
                var F = x[i * 4 + 2];
                //var Z = x[i*4 + 3]; position variable is not needed here
                var Wt = FVPAnalysis.forces[i];
                var diameter = FVPAnalysis.diameters[i];
                var speed = FVPAnalysis.speeds[i];

                #region 1) calc bending stress

                var B = Math.Pow((12 - Qv), (2 / 3)) / 4;
                var A = 50 + 56 * (1 - B);
                var pitchlineVelocity = speed * (Math.PI / 60) * diameter;
                var Kv = Math.Pow((A / (A + Math.Sqrt(pitchlineVelocity))), B);
                fatique[i] = (Wt * pd * Ka * Km * Ks * Kb * Ki) / (F * J * Kv);

                #endregion

                #region 2) calc wear stress

                var Ca = Ka;
                var Cm = Km;
                var Cv = Kv;
                var Cs = Ks;

                wear[i] = Cp * Math.Sqrt(Wt * Ca * Cm * Cs * Cf / (F * I * diameter * Cv));

                #endregion
            }
            var gVal = double.NegativeInfinity;
            for (var i = 0; i < FVPAnalysis.numGears; i++)
            {
                gVal = Math.Max(gVal, Nf * fatique[i] - SFB);
                gVal = Math.Max(gVal, Nf * wear[i] - SFC);
            }
            return gVal;
        }

        #endregion
    }
}