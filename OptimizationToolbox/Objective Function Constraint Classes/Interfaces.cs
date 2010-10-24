using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    public interface IDifferentiable
    {
        double deriv_wrt_xi(double[] x, int i);
    }
    
    public interface IOptFunction
    {
        double calculate(double[] x);
    }
    public interface IObjectiveFunction : IOptFunction { }
    public interface IConstraint : IOptFunction{}
    public interface IEquality : IConstraint { }
    public interface IInequality : IConstraint { }

    public interface IDependentAnalysis
    {
        void calculate(double[] x);
    }
}
