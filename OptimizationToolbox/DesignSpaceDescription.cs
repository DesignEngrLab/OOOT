using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace OptimizationToolbox
{
    public class DesignSpaceDescription
    {
        #region Constructor
        public DesignSpaceDescription() { }
        public DesignSpaceDescription(IEnumerable<VariableDescriptor> VariableDescriptors)
        {
            this.VariableDescriptors = new List<VariableDescriptor>(VariableDescriptors);
            UpdateCharacteristics();
        }
        public DesignSpaceDescription(int n)
        {
            variableDescriptors = new List<VariableDescriptor>(n);
            for (int i = 0; i < n; i++) variableDescriptors.Add(new VariableDescriptor());

            UpdateCharacteristics();
        }

        public void UpdateCharacteristics()
        {
            AllDiscrete = true;
            DiscreteVarIndices = new List<int>();
            MaxVariableSizes = new long[n];

            for (var i = 0; i < n; i++)
            {
                if (!VariableDescriptors[i].Discrete)
                {
                    AllDiscrete = false;
                    MaxVariableSizes[i] = 0;
                }
                else
                {
                    DiscreteVarIndices.Add(i);
                    MaxVariableSizes[i] = VariableDescriptors[i].Size;
                }
            }

            SizeOfSpace = 1;
            foreach (var varIndex in DiscreteVarIndices)
                SizeOfSpace *= VariableDescriptors[varIndex].Size;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of dimensions, the length of x.
        /// </summary>
        /// <value>The n.</value>
        [XmlIgnore]
        public int n { get { return VariableDescriptors.Count; } }

        private List<VariableDescriptor> variableDescriptors;
        /// <summary>
        /// Gets or sets the variable descriptors.
        /// </summary>
        /// <value>The variable descriptors.</value>
        public List<VariableDescriptor> VariableDescriptors
        {
            get { return variableDescriptors; }
            set
            {
                variableDescriptors = value;
                UpdateCharacteristics();
            }
        }
        public VariableDescriptor this[int i]
        {
            get { return VariableDescriptors[i]; }
            set { VariableDescriptors[i] = value; }
        }
        /// <summary>
        /// Gets a value indicating whether [all discrete].
        /// </summary>
        /// <value><c>true</c> if [all discrete]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public Boolean AllDiscrete { get; private set; }
        /// <summary>
        /// Gets the discrete var indices.
        /// </summary>
        /// <value>The discrete var indices.</value>
        [XmlIgnore]
        public List<int> DiscreteVarIndices { get; private set; }
        /// <summary>
        /// Gets the size of space.
        /// </summary>
        /// <value>The size of space.</value>
        [XmlIgnore]
        public long SizeOfSpace { get; private set; }
        /// <summary>
        /// Getsthe max variable sizes.
        /// </summary>
        /// <value>The max variable sizes.</value>
        [XmlIgnore]
        public long[] MaxVariableSizes { get; private set; }
        #endregion

        #region Getting Discrete Vectors with Indices
        public double[] GetVariableVector(IList<long> Indices)
        {
            var result = new double[n];
            for (var i = 0; i < n; i++)
                result[i] = VariableDescriptors[i][Indices[i]];
            return result;
        }

        public int[][] CreateNeighborChangeVectors()
        {
            var result = new int[2*DiscreteVarIndices.Count][];
            int k = 0;
            foreach (var i in DiscreteVarIndices)
            {
                var changeVector = new int[n];
                changeVector[i] = -1;
                result[k++]=changeVector;
                changeVector = new int[n];
                changeVector[i] = +1;
                result[k++] = changeVector;
            }
            return result;
        }

        public List<int> FindValidChanges(double[] candidate, int[][] changeVectors)
        {
            var result = Enumerable.Range(0, changeVectors.GetLength(0)).ToList();
            for (int i = 0; i < n; i++)
                if (candidate[i] == VariableDescriptors[i].LowerBound)
                    result.RemoveAll(a =>changeVectors[a][i] < 0);
                else if (candidate[i] == VariableDescriptors[i].UpperBound)
                    result.RemoveAll(a => changeVectors[a][i] > 0);
            return result;
        }
        #endregion
    }
}