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
        public abstract void generateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int control = -1);
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
