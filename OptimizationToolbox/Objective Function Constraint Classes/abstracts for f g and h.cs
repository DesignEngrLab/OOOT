/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 *  
 *     You should have received a copy of the GNU General Public License
 *     along with OOOT.  If not, see <http://www.gnu.org/licenses/>.
 *     
 *     Please find further details and contact information on OOOT
 *     at http://ooot.codeplex.com/.
 *************************************************************************/
using System;
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