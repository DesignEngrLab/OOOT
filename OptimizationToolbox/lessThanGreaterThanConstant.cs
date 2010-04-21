using System;


namespace OptimizationToolbox
{
    public class lessThanConstant : inequality
    {
        public double constant;
        public int index;

        #region Constructor
        public lessThanConstant() { }
        public lessThanConstant(double constant, int index)
        {
            this.constant = constant;
            this.index = index;
        }
        #endregion

        protected override double calc(double[] x)
        {
            return x[index] - constant;
        }
        public override double deriv_wrt_xi(double[] x, int i)
        {
            if (i == index) return 1.0;
            else return 0.0;
        }
    }

    public class greaterThanConstant : inequality
    {
       public double constant;
       public int index;

       #region Constructor
       public greaterThanConstant() { }
        public greaterThanConstant(double constant, int index)
        {
            this.constant = constant;
            this.index = index;
        }
        #endregion

        protected override double calc(double[] x)
        {
            return constant - x[index];
        }
        public override double deriv_wrt_xi(double[] x, int i)
        {
            if (i == index) return -1.0;
            else return 0.0;
        }
    }
}
