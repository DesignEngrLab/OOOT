// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="equalToValue.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class equalToValue.
    /// Implements the <see cref="OptimizationToolbox.IDifferentiable" />
    /// Implements the <see cref="OptimizationToolbox.IEquality" />
    /// Implements the <see cref="System.Xml.Serialization.IXmlSerializable" />
    /// </summary>
    /// <seealso cref="OptimizationToolbox.IDifferentiable" />
    /// <seealso cref="OptimizationToolbox.IEquality" />
    /// <seealso cref="System.Xml.Serialization.IXmlSerializable" />
    public class equalToValue : IDifferentiable, IEquality, IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the constant.
        /// </summary>
        /// <value>The constant.</value>
        public double constant { get; set; }
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int index { get; set; }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="equalToValue"/> class.
        /// </summary>
        public equalToValue() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="equalToValue"/> class.
        /// </summary>
        /// <param name="constant">The constant.</param>
        /// <param name="index">The index.</param>
        public equalToValue(double constant, int index)
        {
            this.constant = constant;
            this.index = index;
        }
        #endregion


        #region Implementation of IOptFunction

        /// <summary>
        /// Gets or sets the h.
        /// </summary>
        /// <value>The h.</value>
        public double h { get; set; }
        /// <summary>
        /// Gets or sets the find deriv by.
        /// </summary>
        /// <value>The find deriv by.</value>
        public differentiate findDerivBy { get; set; }
        /// <summary>
        /// Gets the number evals.
        /// </summary>
        /// <value>The number evals.</value>
        public int numEvals { get; private set; }
        /// <summary>
        /// Calculates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public double calculate(double[] x)
        {
            return x[index] - constant;
        }

        #endregion

        #region Implementation of IDifferentiable

        /// <summary>
        /// Derivs the WRT xi.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Double.</returns>
        public double deriv_wrt_xi(double[] x, int i)
        {
            if (i == index) return 1.0;
            return 0.0;
        }

        #endregion

        #region Implementation of IXmlSerializable

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic)
        /// from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}