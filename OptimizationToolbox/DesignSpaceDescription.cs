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
using System.Linq;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    public class DesignSpaceDescription
    {
        #region Constructor

        public DesignSpaceDescription()
        {
        }

        public DesignSpaceDescription(IEnumerable<VariableDescriptor> VariableDescriptors)
        {
            this.VariableDescriptors = new List<VariableDescriptor>(VariableDescriptors);
            UpdateCharacteristics();
        }

        public DesignSpaceDescription(int n)
        {
            variableDescriptors = new List<VariableDescriptor>(n);
            for (var i = 0; i < n; i++) variableDescriptors.Add(new VariableDescriptor());

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

        private List<VariableDescriptor> variableDescriptors;

        /// <summary>
        ///   Gets the number of dimensions, the length of x.
        /// </summary>
        /// <value>The n.</value>
        [XmlIgnore]
        public int n
        {
            get { return VariableDescriptors.Count; }
        }

        /// <summary>
        ///   Gets or sets the variable descriptors.
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
        ///   Gets a value indicating whether [all discrete].
        /// </summary>
        /// <value><c>true</c> if [all discrete]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public Boolean AllDiscrete { get; private set; }

        /// <summary>
        ///   Gets the discrete var indices.
        /// </summary>
        /// <value>The discrete var indices.</value>
        [XmlIgnore]
        public List<int> DiscreteVarIndices { get; private set; }

        /// <summary>
        ///   Gets the size of space.
        /// </summary>
        /// <value>The size of space.</value>
        [XmlIgnore]
        public long SizeOfSpace { get; private set; }

        /// <summary>
        ///   Getsthe max variable sizes.
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

        /// <summary>
        ///   Creates the neighbor change vectors. There will at least the minimum specified, 
        ///   and the process will stop after this max is reached although there may be significantly
        ///   more which are created to keep the changes symmetric. This is probably one of the
        ///   craziest little functions I've ever written but there is a method to it madness.
        ///   As opposed to the simplest approach which is +/-1 step in each direction, it seems
        ///   beneficial to have more transitions that can be made. And as opposed to increasing
        ///   these sizes linearly (e.g. +/-1, +/-2, +/-3, ...), it seems better to have them
        ///   increase logarithmically. Here the steps are 1,3,7,20,55,etc. The idea is to move
        ///   in the closest integers to the natural log. As if in base-e. This is shown to
        ///   be optimal from a simple paper I read in science some years ago:
        ///   http://www.americanscientist.org/issues/pub/third-base/3
        ///   The function starts at points e^0 (or 1) away and makes the primary changes,
        ///   {(-1,0), (+1,0), (0,-1), (0,+1)}, and then goes on to e^1 rounded to the closest
        ///   integer {(-3,0), (+3,0), (0,-3), (0,+3)}. But then it goes back to fill out the 
        ///   higher order changes at the lower levels {(-1,-1), (+1,-1), (-1,+1), (+1,+1)}.
        ///   It then jumps to the next exponent for a new set of primary changes, and then
        ///   again drops back to populate the higher level changes of lower levels.
        /// </summary>
        /// <param name = "minimumNeighbors">The minimum neighbors.</param>
        /// <returns></returns>
        public int[][] CreateNeighborChangeVectors(int minimumNeighbors)
        {
            /* the main data structure here is a List of Lists of Lists of arrays.
             * Why not just make it one List of arrays since, in the end, the whole thing
             * is condensed to this anyway? The reason is that we need to keep track of the 
             * transitions for a particular step size in order to create the next set. That
             * is, the CreateNewChangeVectorsBasedOnLast requires this. There may be an easier
             * way to write that function but this may be most efficent. So, the first list is
             * indexed by stepSize, the inner lists by the number of nonzeros in the transitions 
             * (e.g. (1,0,0) is a transition in the first of these lists, and (0,-1,1) is a trans-
             * tion in the second list. The third list is simply the number of symmetric transitions
             * found at that step size for that many non-zeros. There are two for each binary possibilty
             * to ensure symmetry - for every positive step there is a corresponding one negative step. */
            var transitions = new List<List<List<int[]>>>();
            var exponent = 0;
            do
            {
                var tempExp = exponent;
                do
                {
                    var stepSize = (int)(Math.Round(Math.Exp(tempExp)));
                    var lastChanges = new List<int[]> { new int[n] };
                    if (transitions.Count <= tempExp)
                        transitions.Add(new List<List<int[]>>());
                    else lastChanges = transitions[tempExp][transitions[tempExp].Count - 1];
                    transitions[tempExp].Add(CreateNewChangeVectorsBasedOnLast(lastChanges, stepSize));
                    minimumNeighbors -= transitions[tempExp][transitions[tempExp].Count - 1].Count;
                } while ((--tempExp >= 0) && (minimumNeighbors > 0));
                exponent++;
            } while (minimumNeighbors > 0);
            var transitionsCombined = new List<int[]>();
            foreach (var degreeLists in transitions.SelectMany(sizeLists => sizeLists))
            {
                transitionsCombined.AddRange(degreeLists);
            }
            return transitionsCombined.ToArray();
        }

        private List<int[]> CreateNewChangeVectorsBasedOnLast(IEnumerable<int[]> lastChanges, int stepSize)
        {
            var changes = new List<int[]>();
            foreach (var baseVector in lastChanges)
            {
                var firstAffectiveNewIndex = Array.FindLastIndex(baseVector, a => (a != 0)) + 1;
                foreach (var i in DiscreteVarIndices)
                    if (i >= firstAffectiveNewIndex)
                    {
                        var changeVector = (int[])baseVector.Clone();
                        changeVector[i] = -stepSize;
                        changes.Add(changeVector);
                        changeVector = (int[])baseVector.Clone();
                        changeVector[i] = stepSize;
                        changes.Add(changeVector);
                    }
            }
            return changes;
        }


        public List<int> FindValidChanges(double[] candidate, int[][] changeVectors)
        {
            var result = Enumerable.Range(0, changeVectors.GetLength(0)).ToList();
            for (var i = 0; i < n; i++)
            {
                result.RemoveAll(a => ((candidate[i] + changeVectors[a][i]) < VariableDescriptors[i].LowerBound));
                result.RemoveAll(a => ((candidate[i] + changeVectors[a][i]) > VariableDescriptors[i].UpperBound));
            }
            return result;
        }

        #endregion
    }
}