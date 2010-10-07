using System;


namespace OptimizationToolbox
{
    public class equalToValue : equality
    {
        public double constant;
        public int index;

        #region Constructor
        public equalToValue() { }
        public equalToValue(double constant, int index)
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
}
