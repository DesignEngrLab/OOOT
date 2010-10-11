using System;
using System.Collections.Generic;


namespace OptimizationToolbox
{
    public abstract class abstractGenerator
    {
        protected readonly DesignSpaceDescription discreteSpaceDescriptor;
        protected int n { get { return discreteSpaceDescriptor.n; } }
        protected List<VariableDescriptor> VariableDescriptors { get { return discreteSpaceDescriptor.VariableDescriptors; } }
        protected List<int> DiscreteVarIndices { get { return discreteSpaceDescriptor.DiscreteVarIndices; } }
        protected long[] MaxVariableSizes { get { return discreteSpaceDescriptor.MaxVariableSizes; } }
        /// <summary>
        /// Initializes a new instance of the <see cref="abstractGenerator"/> class.
        /// </summary>
        /// <param name="discreteSpaceDescriptor">The discrete space descriptor.</param>
        protected abstractGenerator(DesignSpaceDescription discreteSpaceDescriptor)
        {
            this.discreteSpaceDescriptor = discreteSpaceDescriptor;
        }

        public virtual void GenerateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int control = -1)
        { throw new NotImplementedException("An override of GenerateOneCandidate (which takes a list of KeyValuePairs) was not created in class, " + this.GetType()); }
        public virtual List<double[]> GenerateCandidates(double[] candidate, int control = -1)
        { throw new NotImplementedException("An override of GenerateCandidates (which takes a single  candidate's double array and return a list of candidates)"+
            " was not created in class, " + this.GetType()); }
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
