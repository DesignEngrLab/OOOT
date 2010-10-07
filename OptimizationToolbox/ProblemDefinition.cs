using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace OptimizationToolbox
{
    public class ProblemDefinition
    {
        public string name;
        public double tolerance;
        public DiscreteSpaceDescriptor SpaceDescriptor;
        public double[] xStart;
        public objectiveFunction f;
        public List<equality> h;
        public List<inequality> g;

        public List<abstractConvergence> ConvergenceMethods;



        #region Set-up function, Add.
        public void Add(object function)
        {
            if (function.GetType().BaseType == typeof(inequality))
                g.Add((inequality)function);
            else if (function.GetType().BaseType == typeof(equality))
                h.Add((equality)function);
            else if (function.GetType().BaseType == typeof(objectiveFunction))
                f = (objectiveFunction)function;
            else if ((function.GetType().BaseType == typeof(abstractConvergence))
                && (!ConvergenceMethods.Exists(c => (c.GetType().Equals(function.GetType())))))
                ConvergenceMethods.Add((abstractConvergence)function);
            else if (function.GetType() == typeof(DiscreteSpaceDescriptor))
                SpaceDescriptor = (DiscreteSpaceDescriptor)function;
            else if (function.GetType() == typeof(double[]))
                xStart = (double[])function;
            else if (function.GetType() == typeof(double))
                tolerance = (double)function;
            else throw (new Exception("Function, " + function + ", not of known type (needs "
                + "to inherit from inequality, equality, or objectiveFunction."));
        }
        #endregion

        public void saveProbToXml(string filename)
        {
            StreamWriter probWriter = null;
            probWriter = new StreamWriter(filename);
            XmlSerializer probSerializer = new XmlSerializer(typeof(ProblemDefinition));
            probSerializer.Serialize(probWriter, this);
            if (probWriter != null) probWriter.Close();
        }

        public static ProblemDefinition openprobFromXml(string filename)
        {
            StreamReader probReader = null;
            ProblemDefinition newDesignprob = null;
            probReader = new StreamReader(filename);
            XmlSerializer probDeserializer = new XmlSerializer(typeof(ProblemDefinition));
            newDesignprob = (ProblemDefinition)probDeserializer.Deserialize(probReader);
            SearchIO.output(Path.GetFileName(filename) + " successfully loaded.");

            if (newDesignprob.name == null)
                newDesignprob.name = Path.GetFileNameWithoutExtension(filename);

            if (probReader != null) probReader.Close();

            return newDesignprob;
        }
    }
}
