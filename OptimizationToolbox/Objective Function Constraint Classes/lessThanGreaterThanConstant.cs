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
    public abstract class inequalityWithConstant : IDifferentiable, IInequality
    {
        public double constant { get; set; }
        public int index { get; set; }

        #region Constructor

        protected inequalityWithConstant()
        {
        }

        protected inequalityWithConstant(double constant, int index)
        {
            this.constant = constant;
            this.index = index;
        }

        #endregion

        #region Implementation of IDifferentiable

        public abstract double deriv_wrt_xi(double[] x, int i);

        #endregion

        #region Implementation of IOptFunction
        public abstract double calculate(double[] x);

        #endregion
    }

    public class lessThanConstant : inequalityWithConstant
    {

        public override double deriv_wrt_xi(double[] x, int i)
        {
            if (i == index) return 1.0;
            return 0.0;
        }

        public override double calculate(double[] x)
        {
            return x[index] - constant;
        }
    }

    public class greaterThanConstant : inequalityWithConstant
    {
        public override double deriv_wrt_xi(double[] x, int i)
        {
            if (i == index) return -1.0;
            return 0.0;
        }

        public override double calculate(double[] x)
        {
            return constant - x[index];
        }
    }
}