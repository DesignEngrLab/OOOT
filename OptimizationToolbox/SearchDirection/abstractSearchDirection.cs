using System;


namespace OptimizationToolbox
{
    public abstract class abstractSearchDirection
    {
        public abstract double[] find(double[] x, double[] gradf, double f);
        public virtual double[] find(double[] x, double[] gradf, double f, Boolean reset)
        {
            return find(x, gradf, f);
        }
        public virtual double[] find(double[] x, double[] gradf, double f, ref double initAlpha)
        {
            return find(x, gradf, f);
        }
        public virtual double[] find(double[] x, double[] gradf, double f, ref double initAlpha,
            Boolean reset)
        {
            return find(x, gradf, f);
        }
    }
}
