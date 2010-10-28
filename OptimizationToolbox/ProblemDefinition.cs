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
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    ///    how many variables and what are there ranges (if specified) and which
    ///    are discrete, and which are continuous. Additionally, a starting
    ///    point, xStart, can also be provided s.t. the optimization methods
    ///    start at the same location.
    /// The evaluation of the problem is essentially described by the f's, g's and h's
    ///    functions (note that these employ a special class simply for proper
    ///    XML serialization, ListforIOptFunctions).
    /// Convergence methods are really a part of the optimization method, but these
    ///    can stored within the problem definition so that one can compare different
    ///    methods operating under the same convergence criteria.
    /// </summary>
    public class ProblemDefinition
    {
        #region Properties
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
        /// Initializes a new instance of the <see cref="ProblemDefinition"/> class.
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
        public void Add(object function)
        {
            if (typeof(IInequality).IsInstanceOfType(function))
                g.Add((IInequality)function);
            else if (typeof(IEquality).IsInstanceOfType(function))
                h.Add((IEquality)function);
            else if (typeof(IObjectiveFunction).IsInstanceOfType(function))
                f.Add((IObjectiveFunction)function);
            else if (typeof(abstractConvergence).IsInstanceOfType(function))
                ConvergenceMethods.Add((abstractConvergence)function);
            else if (typeof(DesignSpaceDescription).IsInstanceOfType(function))
                SpaceDescriptor = (DesignSpaceDescription)function;
            else if (typeof(double[]).IsInstanceOfType(function))
                xStart = (double[])function;
            else
                throw (new Exception("Function, " + function + ", not of known type (needs "
                                     + "to inherit from inequality, equality, or objectiveFunction."));
        }

        #endregion

        /// <summary>
        /// Saves the problem definition to XML.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void saveProbToXml(string filename)
        {
            var probWriter = new StreamWriter(filename);
            var probSerializer = new XmlSerializer(typeof(ProblemDefinition));
            probSerializer.Serialize(probWriter, this);
            probWriter.Close();
        }


        /// <summary>
        /// Open the problem definition from XML.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static ProblemDefinition openprobFromXml(string filename)
        {
            var probReader = new StreamReader(filename);
            var probDeserializer = new XmlSerializer(typeof(ProblemDefinition));
            var newDesignprob = (ProblemDefinition)probDeserializer.Deserialize(probReader);
            SearchIO.output(Path.GetFileName(filename) + " successfully loaded.");
            if (newDesignprob.name == null)
                newDesignprob.name = Path.GetFileNameWithoutExtension(filename);
            foreach (var item in newDesignprob.FunctionList.Items)
                if (typeof(IObjectiveFunction).IsInstanceOfType(item))
                    newDesignprob.f.Add((IObjectiveFunction)item);
                else if (typeof(IInequality).IsInstanceOfType(item))
                    newDesignprob.g.Add((IInequality)item);
                else if (typeof(IEquality).IsInstanceOfType(item))
                    newDesignprob.h.Add((IEquality)item);
            newDesignprob.FunctionList = new ListforIOptFunctions(newDesignprob.f, newDesignprob.g, newDesignprob.h);
            probReader.Close();
            return newDesignprob;
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
        private readonly List<IObjectiveFunction> _f;
        private readonly List<IInequality> _g;
        private readonly List<IEquality> _h;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListforIOptFunctions"/> class.
        /// </summary>
        public ListforIOptFunctions(List<IObjectiveFunction> f, List<IInequality> g, List<IEquality> h)
        {
            _f = f;
            _g = g;
            _h = h;
            Items = new List<IOptFunction>();
        }
        public ListforIOptFunctions()
        {
            _f = new List<IObjectiveFunction>();
            _g = new List<IInequality>();
            _h = new List<IEquality>();
            Items = new List<IOptFunction>();
        }

        public List<IOptFunction> Items { get; private set; }
        #region IXmlSerializable Members

        /// <summary>
        ///   This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref = "T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        ///   An <see cref = "T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref = "M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref = "M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

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
                Items.Add((IOptFunction)new XmlSerializer(type).Deserialize(subReader));
            }
        }

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