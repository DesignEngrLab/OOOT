using System;
using OptimizationToolbox;
using TVGL;

namespace SphericalOptimization
{
    internal class FloorAndWallFaceScore :IObjectiveFunction
    {
        private readonly TessellatedSolid tessellatedSolid;
        private const double b = 50; //practical values between 5 to 100
        private static double denomProx = 1 + Math.Exp(-b);

        public FloorAndWallFaceScore(TessellatedSolid tessellatedSolid)
        {
            this.tessellatedSolid = tessellatedSolid;
        }

        public double calculate(double[] x)
        {
            Vector3 d = new Vector3(x);
            //Vector3 d = MakeUnitVectorFromSpherical(x[0], x[1]);
            var total = 0.0;
            foreach (var face in tessellatedSolid.Faces)
            {
                var dot = d.Dot(face.Normal);
                total += face.Area * CuttingProximityScore(dot);
            }
            return total;
        }

        private double CuttingProximityScore(double dot)
        {
            return -(Math.Exp(b * (-dot - 1)) + Math.Exp(-b * Math.Abs(dot))) / denomProx;
        }

        private Vector3 MakeUnitVectorFromSpherical(double inclinationAngle, double azimuthalAngle )
        {
            var sinInclination = Math.Sin(inclinationAngle);
            var x = Math.Cos(azimuthalAngle) * sinInclination;
            var y = Math.Sin(azimuthalAngle) * sinInclination;
            var z = Math.Cos(inclinationAngle);
            return (new Vector3(x, y, z)).Normalize();
        }
    }
}