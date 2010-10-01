using System;
using StarMathLib;
using System.Collections.Generic;


namespace OptimizationToolbox
{
    public class PowellsDirection : abstractSearchDirection
    {
        double[] dir = null;
        int k;
        int n;
        double[,] searchDirMatrix;
        public override double[] find(double[] x, double[] gradf, double f)
        {
            return find(x, gradf, f, false);
        }

        public override double[] find(double[] x, double[] gradf, double f, Boolean reset)
        {
            if (reset || !(searchDirMatrix.Length >= 1))
                PowellsReset(x);

            int column;

            //Coordinate Search First
            if (k < n)
            {
                column = k;
                for (int i = 0; i < n; i++)
                    dir[i] = searchDirMatrix[i,k];
            }
            //Average Directions
            else
            {
                column = k%n;  //Remainder
                for (int i = 0; i < n; i++)
                {
                    dir[i] = 0.0;
                    for (int j = 0; j < n; j++){
                        dir[i] += searchDirMatrix[i, j];
                    }
                }
                dir = StarMath.normalize(dir);
            }
            
            return dir;
            
        }
        private void PowellsReset(double[] x)
        {
            k = 0;
            n = x.GetLength(0);
            searchDirMatrix = StarMath.makeIdentity(n);
        }
    }
}
