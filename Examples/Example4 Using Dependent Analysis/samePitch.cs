using OptimizationToolbox;

namespace Example4_Using_Dependent_Analysis
{
    class samePitch : IEquality, IDifferentiable
    {
        int pitchIndex1;
        int pitchIndex2;

        #region Constructor
        public samePitch(int pI1, int pI2)
        {
            this.pitchIndex1 = pI1;
            this.pitchIndex2 = pI2;
        }
        #endregion



        #region Implementation of IOptFunction

        public double calculate(double[] x)
        {
            return x[pitchIndex1] - x[pitchIndex2];
        }

        #endregion

        #region Implementation of IDifferentiable

        public double deriv_wrt_xi(double[] x, int i)
        {
            if (i == pitchIndex1)
                return 1.0;
            else if (i == pitchIndex2)
                return -1.0;
            return 0.0;
        }

        #endregion
    }
}
