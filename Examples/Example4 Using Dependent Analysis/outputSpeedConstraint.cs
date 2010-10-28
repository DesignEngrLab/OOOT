using System;
using OptimizationToolbox;

namespace Example4_Using_Dependent_Analysis
{
    class outputSpeedConstraint : IEquality
    {
        private readonly ForceVelocityPositionAnalysis FVPAnalysis;

        private readonly double targetSpeed;
        private readonly double tolerance;

        public outputSpeedConstraint(ForceVelocityPositionAnalysis fvpAnalysis, double targetSpeed, double tolerance)
        {
            FVPAnalysis = fvpAnalysis;
            this.targetSpeed = targetSpeed;
            this.tolerance = tolerance;
        }

        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            var hVal = Math.Pow((FVPAnalysis.speeds[FVPAnalysis.numGears-1]-targetSpeed),2);

            if (hVal < tolerance) return 0.0;
            return hVal;
        }

        #endregion
    }
}