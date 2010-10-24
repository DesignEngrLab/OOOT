using System;
using OptimizationToolbox;

namespace tester.G01
{
    class go1constraint1 : IInequality
    {
        public double calculate(double[] x)
        {
            return 2 * x[0] + 2 * x[1] + x[9] + x[10] - 10;
        }

    }
    class go1constraint2 : IInequality
    {
        public double calculate(double[] x)
        {
            return 2 * x[0] + 2 * x[2] + x[9] + x[11] - 10;
        }
    }
    class go1constraint3 : IInequality
    {
        public double calculate(double[] x)
        {
            return 2 * x[1] + 2 * x[2] + x[10] + x[11] - 10;
        }
    }
    class go1constraint4 : IInequality
    {
        public double calculate(double[] x)
        {
            return -8 * x[0] + x[9];
        }
    }
    class go1constraint5 : IInequality
    {
        public double calculate(double[] x)
        {
            return -8 * x[1] + x[10];
        }
    }
    class go1constraint6 : IInequality
    {
        public double calculate(double[] x)
        {
            return -8 * x[2] + x[11];
        }
    }
    class go1constraint7 : IInequality
    {
        public double calculate(double[] x)
        {
            return -2 * x[3] - x[4] + x[9];
        }
    }
    class go1constraint8 : IInequality
    {
        public double calculate(double[] x)
        {
            return -2 * x[5] - x[6] + x[10];
        }
    }
    class go1constraint9 : IInequality
    {
        public double calculate(double[] x)
        {
            return -2 * x[7] - x[8] + x[11];
        }
    }
}
