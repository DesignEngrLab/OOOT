using System;

using System.Collections.Generic;
using System.Xml.Serialization;


namespace OptimizationToolbox
{

    [XmlInclude(typeof(polynomialObjFn))]
    public abstract class objectiveFunction : abstractOptFunction
    { }

    public abstract class constraint : abstractOptFunction
    {
        abstract public Boolean feasible(double[] x);
    }

    [XmlInclude(typeof(lessThanConstant)),
    XmlInclude(typeof(greaterThanConstant)),
    XmlInclude(typeof(polynomialInequality))]
    public abstract class inequality : constraint
    {
        public override Boolean feasible(double[] x)
        {
            return (this.calculate(x) <= 0);
        }

    }

    [XmlInclude(typeof(polynomialEquality))]
    public abstract class equality : constraint
    {
        public override Boolean feasible(double[] x)
        {
            return (0 == this.calculate(x));
        }
    }
}