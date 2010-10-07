using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    public class DiscreteSpaceDescriptor
    {
        #region Constructor
        public DiscreteSpaceDescriptor() { }
        public DiscreteSpaceDescriptor(IList<VariableDescriptor> VariableDescriptors)
        {
            n = VariableDescriptors.Count;
            this.VariableDescriptors = new List<VariableDescriptor>(VariableDescriptors);
        }
        public DiscreteSpaceDescriptor(int n)
        {
            this.n = n;
            if (variableDescriptors == null)
            {
                variableDescriptors = new List<VariableDescriptor>(n);
                for (int i = 0; i < n; i++) variableDescriptors.Add(new VariableDescriptor());
            }
            updateCharacteristics();
        }

        private void updateCharacteristics()
        {
            AllDiscrete = true;
            DiscreteVarIndices = new List<int>();
            CurrentIndices = new long[n];
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
        public int n { get; private set; }

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
                updateCharacteristics();
            }
        }
        public VariableDescriptor this[int i]
        {
            get { return VariableDescriptors[i]; }
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
        /// Gets the current indices.
        /// </summary>
        /// <value>The current indices.</value>
        [XmlIgnore]
        public long[] CurrentIndices { get; private set; }
        /// <summary>
        /// Getsthe max variable sizes.
        /// </summary>
        /// <value>The max variable sizes.</value>
        [XmlIgnore]
        public long[] MaxVariableSizes { get; private set; }
        #endregion

        #region Getting Discrete Vectors with Indices
        public double[] GetVariableVector(IList<int> Indices = null)
        {
            if (Indices != null)
            {
                if (Indices.Count == n)
                    for (var i = 0; i < n; i++)
                        CurrentIndices[i] = Indices[i];
                else if (Indices.Count == DiscreteVarIndices.Count)
                {
                    CurrentIndices = new long[n];
                    foreach (var i in DiscreteVarIndices)
                        CurrentIndices[i] = Indices[i];
                }
                else throw new Exception("Input Indices not of proper length.");
            }
            var result = new double[n];
            for (var i = 0; i < n; i++)
                result[i] = VariableDescriptors[i][CurrentIndices[i]];
            return result;
        }

        private Boolean IncrementIndices(int IndicesIndex = 0)
        {
            if (IndicesIndex == n) return false;
            CurrentIndices[IndicesIndex]++;
            if (CurrentIndices[IndicesIndex] >= MaxVariableSizes[IndicesIndex])
            {
                CurrentIndices[IndicesIndex] = 0;
                return IncrementIndices(IndicesIndex + 1);
            }
            else return true;
        }

        private Boolean DecrementIndices(int IndicesIndex = 0)
        {
            if (IndicesIndex == n) return false;
            CurrentIndices[IndicesIndex]--;
            if (CurrentIndices[IndicesIndex] < 0)
            {
                CurrentIndices[IndicesIndex] = MaxVariableSizes[IndicesIndex] - 1;
                return DecrementIndices(IndicesIndex + 1);
            }
            else return true;
        }
        #endregion
    }
}