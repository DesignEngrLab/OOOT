using System;
using OptimizationToolbox;
using StarMathLib;

namespace Example4_Using_Dependent_Analysis
{
    internal class outputLocationConstraint : IEquality
    {
        private readonly ForceVelocityPositionAnalysis FVPAnalysis;
        private readonly double tolerance;

        private readonly double thetaX;
        private readonly double thetaY;
        private readonly double thetaZ;
        private readonly double xtarget;
        private readonly double ytarget;
        private readonly double ztarget;

        #region Constructor

        public outputLocationConstraint(ForceVelocityPositionAnalysis fvpAnalysis, double tolerance,
            double xtarget, double ytarget, double ztarget)
        // double thetaX, double thetaY, double thetaZ)
        {
            FVPAnalysis = fvpAnalysis;
            this.tolerance = tolerance;
            this.xtarget = xtarget;
            this.ytarget = ytarget;
            this.ztarget = ztarget;
            //this.thetaX = thetaX;
            //this.thetaY = thetaY;
            //this.thetaZ = thetaZ;
        }

        #endregion

        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            var targetOut = new[] { xtarget, ytarget, ztarget, 1.0 };
            var candidateOut = StarMath.GetColumn(3, FVPAnalysis.positions[FVPAnalysis.numGears - 1]);
            var hVal = StarMath.norm2(targetOut, candidateOut, true);
            if (hVal < tolerance) return 0.0;
            return hVal;
        }

        #endregion
    }
}