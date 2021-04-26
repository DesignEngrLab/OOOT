// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="ProblemDefinition.cs" company="OptimizationToolbox">
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    /// <summary>
    /// This class is used only for the XML serialization and deserialization of
    /// optimization problems. The saving and opening of problems allows one to
    /// compare the performance across multiple techniques without have to re-
    /// write a bunch of code. Note that types of elements stored in this file.
    /// The representation of the problem is captured by the SpaceDescriptor:
    /// how many variables and what are there ranges (if specified) and which
    /// are discrete, and which are continuous. Additionally, a starting
    /// point, xStart, can also be provided s.t. the optimization methods
    /// start at the same location.
    /// The evaluation of the problem is essentially described by the f's, g's and h's
    /// functions (note that these employ a special class simply for proper
    /// XML serialization, ListforIOptFunctions).
    /// Convergence methods are really a part of the optimization method, but these
    /// can stored within the problem definition so that one can compare different
    /// methods operating under the same convergence criteria.
    /// </summary>
    public class ProblemDefinition
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the x start.
        /// </summary>
        /// <value>The x start.</value>
        public double[] xStart { get; set; }

        /// <summary>
        /// Gets or sets the convergence methods.
        /// </summary>
        /// <value>The convergence methods.</value>
        public List<abstractConvergence> ConvergenceMethods { get; set; }


        /// <summary>
        /// The number converge criteria needed
        /// </summary>
        private int numConvergeCriteriaNeeded = 1;
        /// <summary>
        /// Gets or sets the num convergence criteria needed to stop the process.
        /// </summary>
        /// <value>The num converge criteria needed.</value>
        public int NumConvergeCriteriaNeeded
        {
            get { return numConvergeCriteriaNeeded; }
            set { numConvergeCriteriaNeeded = value; }
        }
        /// <summary>
        /// Gets or sets the space descriptor.
        /// </summary>
        /// <value>The space descriptor.</value>
        public DesignSpaceDescription SpaceDescriptor { get; set; }

        /// <summary>
        /// Gets or sets the f.
        /// </summary>
        /// <value>The f.</value>
        [XmlIgnore]
        public List<IObjectiveFunction> f { get; private set; }
        /// <summary>
        /// Gets or sets the g.
        /// </summary>
        /// <value>The g.</value>
        [XmlIgnore]
        public List<IInequality> g { get; private set; }
        /// <summary>
        /// Gets or sets the h.
        /// </summary>
        /// <value>The h.</value>
        [XmlIgnore]
        public List<IEquality> h { get; private set; }


        /// <summary>
        /// Gets or sets the function list.
        /// </summary>
        /// <value>The function list.</value>
        public ListforIOptFunctions FunctionList { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemDefinition" /> class.
        /// </summary>
        public ProblemDefinition()
        {
            ConvergenceMethods = new List<abstractConvergence>();
            SpaceDescriptor = new DesignSpaceDescription();
            f = new List<IObjectiveFunction>();
            g = new List<IInequality>();
            h = new List<IEquality>();
            FunctionList = new ListforIOptFunctions(f, g, h);
        }

        #endregion

        #region Set-up function, Add.

        /// <summary>
        /// Adds the specified object to the problem definition as is similiarly done
        /// for abstractOptMethod.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <exception cref="Exception">Function, " + function + ", not of known type (needs "
        ///                                      + "to inherit from inequality, equality, or objectiveFunction.</exception>
        public void Add(object function)
        {
            if (function is IInequality)
                g.Add((IInequality)function);
            else if (function is IEquality)
                h.Add((IEquality)function);
            else if (function is IObjectiveFunction)
                f.Add((IObjectiveFunction)function);
            else if (function is abstractConvergence)
                ConvergenceMethods.Add((abstractConvergence)function);
            else if (function is DesignSpaceDescription)
                SpaceDescriptor = (DesignSpaceDescription)function;
            else if (function is double[])
                xStart = (double[])function;
            else
                throw (new Exception("Function, " + function + ", not of known type (needs "
                                     + "to inherit from inequality, equality, or objectiveFunction."));
        }

        #endregion

        /// <summary>
        /// Saves the problem definition to XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SaveProbToXml(Stream stream)
        {
            var probWriter = new StreamWriter(stream);
            var probSerializer = new XmlSerializer(typeof(ProblemDefinition));
            probSerializer.Serialize(probWriter, this);
            probWriter.Dispose();
        }


        /// <summary>
        /// Open the problem definition from XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>ProblemDefinition.</returns>
        public static ProblemDefinition OpenprobFromXml(Stream stream)
        {
            var probReader = new StreamReader(stream);
            var probDeserializer = new XmlSerializer(typeof(ProblemDefinition));
            var newDesignprob = (ProblemDefinition)probDeserializer.Deserialize(probReader);
            string name = getNameFromStream(stream);
            SearchIO.output(name + " successfully loaded.");
            if (newDesignprob.name == null)
                newDesignprob.name = name;
            foreach (var item in newDesignprob.FunctionList.Items)
                if (item is IObjectiveFunction)
                    newDesignprob.f.Add((IObjectiveFunction)item);
                else if (item is IInequality)
                    newDesignprob.g.Add((IInequality)item);
                else if (item is IEquality)
                    newDesignprob.h.Add((IEquality)item);
            newDesignprob.FunctionList = new ListforIOptFunctions(newDesignprob.f, newDesignprob.g, newDesignprob.h);
            probReader.Dispose();
            return newDesignprob;
        }

        /// <summary>
        /// Gets the name from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>System.String.</returns>
        private static string getNameFromStream(Stream stream)
        {
            var type = stream.GetType();
            var namePropertyInfo = type.GetProperty("Name");
            var name = (string)namePropertyInfo.GetValue(stream, null);
            var lastDirectorySeparator = name.LastIndexOf("\\");
            var fileExtensionIndex = name.LastIndexOf(".");
            return (lastDirectorySeparator < fileExtensionIndex)
                ? name.Substring(lastDirectorySeparator + 1, fileExtensionIndex - lastDirectorySeparator - 1)
                : name.Substring(lastDirectorySeparator + 1, name.Length - lastDirectorySeparator - 1);
        }
    }

    /// <summary>
    /// This little class is a necessary evil once I removed the abstract classes. The reason being that
    /// interfaces are not (directly) serializable as abstract classes are. The problem definition
    /// class avoids any "discussion" of interfaces so that we can easily use the XmlSerializer
    /// automatically. However, for the list of functions (objective functions, inequalities and equalities)
    /// we use this, which overrides (thanks to the IXmlSerializable Members) how the lists are
    /// serialized.
    /// </summary>
    public class ListforIOptFunctions : IXmlSerializable
    {
        /// <summary>
        /// The f
        /// </summary>
        private readonly List<IObjectiveFunction> _f;
        /// <summary>
        /// The g
        /// </summary>
        private readonly List<IInequality> _g;
        /// <summary>
        /// The h
        /// </summary>
        private readonly List<IEquality> _h;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListforIOptFunctions" /> class.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <param name="h">The h.</param>
        public ListforIOptFunctions(List<IObjectiveFunction> f, List<IInequality> g, List<IEquality> h)
        {
            _f = f;
            _g = g;
            _h = h;
            Items = new List<IOptFunction>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ListforIOptFunctions"/> class.
        /// </summary>
        public ListforIOptFunctions()
        {
            _f = new List<IObjectiveFunction>();
            _g = new List<IInequality>();
            _h = new List<IEquality>();
            Items = new List<IOptFunction>();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public List<IOptFunction> Items { get; private set; }
        #region IXmlSerializable Members

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
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
        public void ReadXml(XmlReader reader)
        {
            //Skip whitespaces
            var settings = new XmlReaderSettings { IgnoreWhitespace = true };

            //Create a reader that will read the list's content
            var subReader = XmlReader.Create(reader.ReadSubtree(), settings);
            subReader.ReadStartElement();

            while (subReader.Depth > 0)
            {
                //turn the element name back to a fully-qualified concrete class name
                //NOTE: Assuming this class is sharing the same namespace. If not,
                // place your own logic here to reconstruct full class name
                var type = Type.GetType(GetType().Namespace + "." + subReader.Name);
                if (type != null)
                    Items.Add((IOptFunction)new XmlSerializer(type).Deserialize(subReader));
            }
        }

        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteXml(XmlWriter writer)
        {
            //avoid adding xsi attributes to keep the XMLs nice and clean
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            Items = _f.Cast<IOptFunction>().ToList();
            Items.AddRange(_g.Cast<IOptFunction>());
            Items.AddRange(_h.Cast<IOptFunction>());
            foreach (IOptFunction item in Items)
                new XmlSerializer(item.GetType()).Serialize(writer, item, ns);
        }

        #endregion

    }
}