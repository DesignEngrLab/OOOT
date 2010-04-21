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
        public double[][] bounds;
        public double[] xStart;
        public objectiveFunction f;
        public List<equality> h;
        public List<inequality> g;
        public abstractConvergence convergeMethod;



        #region Set-up function, Add.
        public void Add(object function)
        {
            if (function.GetType().BaseType == typeof(inequality))
                g.Add((inequality)function);
            else if (function.GetType().BaseType == typeof(equality))
                h.Add((equality)function);
            else if (function.GetType().BaseType == typeof(objectiveFunction))
                            f = (objectiveFunction)function;
                        else if (function.GetType().BaseType == typeof(abstractConvergence))
                convergeMethod = (abstractConvergence)function;
            else if (function.GetType() == typeof(double[][]))
                bounds = (double[][])function;
            else if (function.GetType() == typeof(double[]))
                xStart = (double[])function;
            else if (function.GetType() == typeof(double))
                tolerance = (double)function;
            else throw (new Exception("Function, " + function.ToString() + ", not of known type (needs "
                + "to inherit from inequality, equality, objectiveFunction, abstractLineSearch, " +
                "or abstractSearchDirection)."));
        }
        #endregion

        public static void saveProbToXml(string filename, ProblemDefinition prob1)
        {
            StreamWriter probWriter = null;
            try
            {
                probWriter = new StreamWriter(filename);
                XmlSerializer probSerializer = new XmlSerializer(typeof(ProblemDefinition));
                probSerializer.Serialize(probWriter, prob1);
            }
            catch (Exception ioe)
            {
                SearchIO.output("XML Serialization Error: " + ioe.ToString());
            }
            finally
            {
                if (probWriter != null) probWriter.Close();
            }
        }

        public static ProblemDefinition openprobFromXml(string filename)
        {
            StreamReader probReader = null;
            ProblemDefinition newDesignprob = null;
            try
            {
                probReader = new StreamReader(filename);
                XmlSerializer probDeserializer = new XmlSerializer(typeof(ProblemDefinition));
                newDesignprob = (ProblemDefinition)probDeserializer.Deserialize(probReader);
                SearchIO.output(Path.GetFileName(filename) + " successfully loaded.");

                if (newDesignprob.name == null)
                    newDesignprob.name = Path.GetFileNameWithoutExtension(filename);
            }
            catch (Exception ioe)
            { SearchIO.output("Error Opening prob: " + ioe.ToString()); }
            finally
            {
                if (probReader != null) probReader.Close();
            }
            return newDesignprob;
        }
    }
}
