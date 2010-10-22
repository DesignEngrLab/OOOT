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
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    public class ProblemDefinition
    {
        public List<abstractConvergence> ConvergenceMethods;
        public DesignSpaceDescription SpaceDescriptor;
        public objectiveFunction f;
        public List<inequality> g;
        public List<equality> h;
        public string name;
        public double[] xStart;

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
            else if (function.GetType() == typeof(DesignSpaceDescription))
                SpaceDescriptor = (DesignSpaceDescription)function;
            else if (function.GetType() == typeof(double[]))
                xStart = (double[])function;
            else
                throw (new Exception("Function, " + function + ", not of known type (needs "
                                     + "to inherit from inequality, equality, or objectiveFunction."));
        }

        #endregion

        public void saveProbToXml(string filename)
        {
            var probWriter = new StreamWriter(filename);
            var probSerializer = new XmlSerializer(typeof(ProblemDefinition));
            probSerializer.Serialize(probWriter, this);
            probWriter.Close();
        }

        public static ProblemDefinition openprobFromXml(string filename)
        {
            var probReader = new StreamReader(filename);
            var probDeserializer = new XmlSerializer(typeof(ProblemDefinition));
            var newDesignprob = (ProblemDefinition)probDeserializer.Deserialize(probReader);
            SearchIO.output(Path.GetFileName(filename) + " successfully loaded.");
            //newDesignprob.SpaceDescriptor.UpdateCharacteristics();
            if (newDesignprob.name == null)
                newDesignprob.name = Path.GetFileNameWithoutExtension(filename);

            probReader.Close();
            return newDesignprob;
        }
    }
}