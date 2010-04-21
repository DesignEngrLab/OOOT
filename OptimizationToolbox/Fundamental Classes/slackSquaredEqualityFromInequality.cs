using System;


namespace OptimizationToolbox
{
    class slackSquaredEqualityFromInequality : equality
    {
        int slackIndex;
        abstractOptFunction formerIneq;
        public slackSquaredEqualityFromInequality(abstractOptFunction formerIneq, int slackIndex)
        {
            this.formerIneq = formerIneq;
            this.slackIndex = slackIndex;
        }

        protected override double calc(double[] x)
        {
            return formerIneq.calculate(x) + x[slackIndex] * x[slackIndex];
        }

        public override double deriv_wrt_xi(double[] x, int i)
        {
            // the reason this returns 2xi is that the slack variable is squared, xi^2
            // (see calculate above), meaning the derivative is 2xi
            if (i == slackIndex) return 2.0 * x[i];

            else return formerIneq.deriv_wrt_xi(x, i);
        }
    }
}
