/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT X11 License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT X11 License for more details.
 *  


 *     
 *     Please find further details and contact information on OOOT
 *     at http://designengrlab.github.io/OOOT/.
 *************************************************************************/

namespace OptimizationToolbox
{
    public interface IDifferentiable
    {
        double deriv_wrt_xi(double[] x, int i);
    }

    public interface ITwiceDifferentiable
    {
        double second_deriv_wrt_ij(double[] x, int i, int j);
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
