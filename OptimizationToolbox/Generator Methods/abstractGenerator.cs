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
    /// <summary>
    /// The abstract generator class is used for all discrete problems. The generator
    /// creates new solutions. Either by adding to the input list or by simply writing over
    /// it.
    /// </summary>
    public abstract class abstractGenerator
    {
        /// <summary>
        /// The discreteSpaceDescriptor is of type DesignSpaceDescription and includes the
        /// details (VariableDescriptors) for all variables in the system.
        /// </summary>
        protected readonly DesignSpaceDescription discreteSpaceDescriptor;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "abstractGenerator" /> class.
        /// </summary>
        /// <param name = "discreteSpaceDescriptor">The discrete space descriptor.</param>
        protected abstractGenerator(DesignSpaceDescription discreteSpaceDescriptor)
        {
            this.discreteSpaceDescriptor = discreteSpaceDescriptor;
        }

        /// <summary>
        /// Gets the number of dimensions (length of the decision vector, x).
        /// </summary>
        /// <value>The n.</value>
        protected int n
        {
            get { return discreteSpaceDescriptor.n; }
        }


        /// <summary>
        /// Gets the indices for the discrete variables in x.
        /// </summary>
        /// <value>The discrete var indices.</value>
        protected List<int> DiscreteVarIndices
        {
            get { return discreteSpaceDescriptor.DiscreteVarIndices; }
        }

        /// <summary>
        /// Gets the maximum variable sizes.
        /// </summary>
        /// <value>The max variable sizes.</value>
        protected long[] MaxVariableSizes
        {
            get { return discreteSpaceDescriptor.MaxVariableSizes; }
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="control">The control.</param>
        public virtual void GenerateCandidates(ref List<Candidate> candidates, int control = -1)
        {
            throw new NotImplementedException(
                "An override of GenerateOneCandidate (which takes a list of KeyValuePairs) was not created in class, " +
                GetType());
        }

        /// <summary>
        /// Generates the candidates.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        public virtual List<double[]> GenerateCandidates(double[] candidate, int control = -1)
        {
            throw new NotImplementedException(
                "An override of GenerateCandidates (which takes a single  candidate's double array and return a list of candidates)" +
                " was not created in class, " + GetType());
        }
    }


    /// <summary>
    /// The Sampling Generator abstract class is used to indicate which generators are used for initial creation of points.
    /// These could also be used for simple design space exploration as in design of experiments. One may consider writing
    /// additional types like OFAT (one factor at a time), Full-Factorial, Fractional-Factorial, Box-Bencken, etc.
    /// </summary>
    public abstract class SamplingGenerator : abstractGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SamplingGenerator"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        protected SamplingGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }
    }


    /// <summary>
    /// The crossover abstract class is simply used by the genetic algorithm to recognize which generators are to be
    /// used for crossover (as opposed to for initial sampling or mutation).
    /// </summary>
    public abstract class GeneticCrossoverGenerator : abstractGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticCrossoverGenerator"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        protected GeneticCrossoverGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }
    }

    /// <summary>
    /// The mutation abstract class is simply used by the genetic algorithm to recognize which generators are to be
    /// used for mutation (as opposed to for initial sampling or crossover). 
    /// </summary>
    public abstract class GeneticMutationGenerator : abstractGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticMutationGenerator"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        protected GeneticMutationGenerator(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }
    }
}