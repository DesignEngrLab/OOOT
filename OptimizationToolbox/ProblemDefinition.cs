﻿/*************************************************************************
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
using System.Xml.Serialization;
using System.IO;

namespace OptimizationToolbox
{
    public class ProblemDefinition
    {
        public string name;
        public DesignSpaceDescription SpaceDescriptor;
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
            else if (function.GetType() == typeof(DesignSpaceDescription))
                SpaceDescriptor = (DesignSpaceDescription)function;
            else if (function.GetType() == typeof(double[]))
                xStart = (double[])function;
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
            SearchIO.output(Path.GetFileName(filename) + " successfully loaded.", 1);
            newDesignprob.SpaceDescriptor.UpdateCharacteristics();
            if (newDesignprob.name == null)
                newDesignprob.name = Path.GetFileNameWithoutExtension(filename);

            if (probReader != null) probReader.Close();

            return newDesignprob;
        }
    }
}
