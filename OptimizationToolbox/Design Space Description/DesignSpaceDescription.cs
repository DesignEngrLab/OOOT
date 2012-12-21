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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OptimizationToolbox
{
    public class DesignSpaceDescription : IList<VariableDescriptor>
    {
        private readonly List<VariableDescriptor> variableDescriptors;
        #region Constructor

        public DesignSpaceDescription()
        {
            variableDescriptors = new List<VariableDescriptor>();
        }

        public DesignSpaceDescription(IEnumerable<VariableDescriptor> VariableDescriptors)
        {
            variableDescriptors = new List<VariableDescriptor>(VariableDescriptors);
            UpdateCharacteristics();
        }

        public DesignSpaceDescription(int n)
        {
            variableDescriptors = new List<VariableDescriptor>(n);
            for (var i = 0; i < n; i++) variableDescriptors.Add(new VariableDescriptor());

            UpdateCharacteristics();
        }

        void UpdateCharacteristics()
        {
            AllDiscrete = true;
            DiscreteVarIndices = new List<int>();
            MaxVariableSizes = new long[n];

            for (var i = 0; i < n; i++)
            {
                if (!variableDescriptors[i].Discrete)
                {
                    AllDiscrete = false;
                    MaxVariableSizes[i] = 0;
                }
                else
                {
                    DiscreteVarIndices.Add(i);
                    MaxVariableSizes[i] = variableDescriptors[i].Size;
                }
            }

            SizeOfSpace = 1;
            foreach (var varIndex in DiscreteVarIndices)
                SizeOfSpace *= variableDescriptors[varIndex].Size;
        }

        #endregion

        #region Properties
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
        ///   Gets the max variable sizes.
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
                result[i] = variableDescriptors[i][Indices[i]];
            return result;
        }

        public int[][] CreateNeighborChangeVectorsHack(int minimumNeighbors)
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
            var transitions = new List<int[]> {new[] {-1, 0}, new[] {0, -1}, new[] {+1, 0}, new[] {0, +1},
            new[] {+1, +1}, new[] {-1, -1}, new[] {+1, -1}, new[] {-1, +1}};
            return transitions.ToArray();
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
                    if (transitions.Count <= tempExp)
                    {
                        transitions.Add(new List<List<int[]>>());
                        transitions[tempExp].Add(CreateNewChangeVectorsBasedOnLast(stepSize));
                    }
                    else
                        transitions[tempExp].Add(CreateNewChangeVectorsBasedOnLast(transitions[tempExp].Last(), stepSize));
                    minimumNeighbors -= transitions[tempExp][transitions[tempExp].Count - 1].Count;
                } while ((--tempExp >= 0) && (minimumNeighbors > 0));
                exponent++;
            } while (minimumNeighbors > 0);
            var transitionsCombined = new List<int[]>();
            foreach (var degreeLists in transitions.SelectMany(sizeLists => sizeLists))
                transitionsCombined.AddRange(degreeLists);
            return transitionsCombined.ToArray();
        }

        private List<int[]> CreateNewChangeVectorsBasedOnLast(IEnumerable<int[]> lastChanges, int stepSize)
        {
            var changes = new List<int[]>();
            foreach (var baseVector in lastChanges)
            {
                var firstEffectiveIndex = Array.FindLastIndex(baseVector, (a => (a != 0)));
                foreach (var i in DiscreteVarIndices)
                    if (i > firstEffectiveIndex && baseVector[i] == 0)
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


        private List<int[]> CreateNewChangeVectorsBasedOnLast(int stepSize)
        {
            var changes = new List<int[]>();
            foreach (var i in DiscreteVarIndices)
            {
                var changeVector = new int[n];
                changeVector[i] = -stepSize;
                changes.Add(changeVector);
                changeVector = new int[n];
                changeVector[i] = stepSize;
                changes.Add(changeVector);
            }
            return changes;
        }


        public List<int> FindValidChanges(double[] candidate, int[][] changeVectors)
        {
            var result = Enumerable.Range(0, changeVectors.GetLength(0)).ToList();
            for (var i = 0; i < n; i++)
            {
                result.RemoveAll(a => ((variableDescriptors[i].PositionOf(candidate[i]) + changeVectors[a][i]) < 0));
                result.RemoveAll(a => ((variableDescriptors[i].PositionOf(candidate[i]) + changeVectors[a][i]) >= variableDescriptors[i].Size));
            }
            return result;
        }

        #endregion

        #region Implementation of IEnumerable
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<VariableDescriptor> GetEnumerator()
        {
            return new VariableDescriptorEnum(variableDescriptors.ToArray());
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<VariableDescriptor>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///                 </exception>
        public void Add(VariableDescriptor item)
        {
            variableDescriptors.Add(item);
            UpdateCharacteristics();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. 
        ///                 </exception>
        public void Clear()
        {
            variableDescriptors.Clear();
            UpdateCharacteristics();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param>
        public bool Contains(VariableDescriptor item)
        {
            return variableDescriptors.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(VariableDescriptor[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///                 </exception>
        public bool Remove(VariableDescriptor item)
        {
            var result = variableDescriptors.Remove(item);
            UpdateCharacteristics();
            return result;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count { get { return variableDescriptors.Count; } }

        /// <summary>
        /// Gets the number of dimensions in the space, it is the same as DesignSpaceDescription.Count.
        /// </summary>
        /// <value>The number of dimensions, the number of variable descriptors.</value>
        public int n { get { return variableDescriptors.Count; } }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly { get { return false; } }

        #endregion

        #region Implementation of IList<VariableDescriptor>
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.
        ///                 </param>
        public int IndexOf(VariableDescriptor item)
        {
            return variableDescriptors.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.
        ///                 </param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.
        ///                 </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        ///                 </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        ///                 </exception>
        public void Insert(int index, VariableDescriptor item)
        {
            variableDescriptors.Insert(index, item);
            UpdateCharacteristics();
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.
        ///                 </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        ///                 </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        ///                 </exception>
        public void RemoveAt(int index)
        {
            variableDescriptors.RemoveAt(index);
            UpdateCharacteristics();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.
        ///                 </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        ///                 </exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        ///                 </exception>
        public VariableDescriptor this[int index]
        {
            get { return variableDescriptors[index]; }
            set
            {
                variableDescriptors[index] = value;
                UpdateCharacteristics();
            }
        }

        #endregion
    }
}