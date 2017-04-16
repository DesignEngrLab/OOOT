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
    internal class slackSquaredEqualityFromInequality : IEquality
    {
        private readonly IInequality formerIneq;
        private readonly int slackIndex;
        private readonly abstractOptMethod optMethod;

        public slackSquaredEqualityFromInequality(IInequality formerIneq, int slackIndex, abstractOptMethod optMethod)
        {
            this.formerIneq = formerIneq;
            this.slackIndex = slackIndex;
            this.optMethod = optMethod;
        }

        #region Implementation of IOptFunction

        public double h { get; set; }
        public differentiate findDerivBy { get; set; }
        public int numEvals { get; private set; }
        public double calculate(double[] x)
        {
            return optMethod.calculate(formerIneq,x) + x[slackIndex] * x[slackIndex];
        }
        #endregion
    }
}