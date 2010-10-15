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

namespace OptimizationToolbox
{
    public abstract class abstractGenerator
    {
        protected readonly DesignSpaceDescription discreteSpaceDescriptor;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "abstractGenerator" /> class.
        /// </summary>
        /// <param name = "discreteSpaceDescriptor">The discrete space descriptor.</param>
        protected abstractGenerator(DesignSpaceDescription discreteSpaceDescriptor)
        {
            this.discreteSpaceDescriptor = discreteSpaceDescriptor;
        }

        protected int n
        {
            get { return discreteSpaceDescriptor.n; }
        }

        protected List<VariableDescriptor> VariableDescriptors
        {
            get { return discreteSpaceDescriptor.VariableDescriptors; }
        }

        protected List<int> DiscreteVarIndices
        {
            get { return discreteSpaceDescriptor.DiscreteVarIndices; }
        }

        protected long[] MaxVariableSizes
        {
            get { return discreteSpaceDescriptor.MaxVariableSizes; }
        }

        public virtual void GenerateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int control = -1)
        {
            throw new NotImplementedException(
                "An override of GenerateOneCandidate (which takes a list of KeyValuePairs) was not created in class, " +
                GetType());
        }

        public virtual List<double[]> GenerateCandidates(double[] candidate, int control = -1)
        {
            throw new NotImplementedException(
                "An override of GenerateCandidates (which takes a single  candidate's double array and return a list of candidates)" +
                " was not created in class, " + GetType());
        }
    }


    public abstract class SamplingGenerator : abstractGenerator
    {
        protected SamplingGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }
    }


    public abstract class GeneticCrossoverGenerator : abstractGenerator
    {
        protected GeneticCrossoverGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }
    }

    public abstract class GeneticMutationGenerator : abstractGenerator
    {
        protected GeneticMutationGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }
    }
}