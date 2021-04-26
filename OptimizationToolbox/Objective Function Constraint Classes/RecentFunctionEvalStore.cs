// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="RecentFunctionEvalStore.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    /// <summary>
    /// Class RecentFunctionEvalStore.
    /// Implements the <see cref="System.Collections.Generic.IDictionary{System.Double[], System.Double}" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IDictionary{System.Double[], System.Double}" />
    internal class RecentFunctionEvalStore : IDictionary<double[], double>
    {
        /// <summary>
        /// The old evaluations
        /// </summary>
        readonly Dictionary<double[], double> oldEvaluations;
        /// <summary>
        /// The queue
        /// </summary>
        readonly Queue<double[]> queue;

        /// <summary>
        /// Gets or sets the number evals.
        /// </summary>
        /// <value>The number evals.</value>
        internal long numEvals { get; set; }
        /// <summary>
        /// Gets or sets the size of the finite difference step.
        /// </summary>
        /// <value>The size of the finite difference step.</value>
        internal double finiteDiffStepSize { get; set; }
        /// <summary>
        /// Gets or sets the find deriv by.
        /// </summary>
        /// <value>The find deriv by.</value>
        internal differentiate findDerivBy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentFunctionEvalStore"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="comparer">The comparer.</param>
        /// <param name="finiteDiffStepSize">Size of the finite difference step.</param>
        /// <param name="findDerivBy">The find deriv by.</param>
        internal RecentFunctionEvalStore(IOptFunction function, IEqualityComparer<double[]> comparer,
            double finiteDiffStepSize, differentiate findDerivBy)
        {
            if (function is IDifferentiable)
                this.findDerivBy = differentiate.Analytic;
            else
            {
                this.finiteDiffStepSize = finiteDiffStepSize;
                this.findDerivBy = findDerivBy;
            }
            oldEvaluations = new Dictionary<double[], double>(comparer);
            queue = new Queue<double[]>();
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<double[], double>> GetEnumerator()
        {
            return oldEvaluations.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<KeyValuePair<double[],double>>
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(KeyValuePair<double[], double> item)
        {
            Add(item.Key, item.Value);
        }
        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            oldEvaluations.Clear();
            numEvals = 0;
        }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns><see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(KeyValuePair<double[], double> item)
        {
            return ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void CopyTo(KeyValuePair<double[], double>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns><see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public bool Remove(KeyValuePair<double[], double> item)
        {
            return Remove(item.Key);
        }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return oldEvaluations.Count; }
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly { get { return false; } }

        #endregion

        #region Implementation of IDictionary<double[],double>

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// <returns>true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool ContainsKey(double[] key)
        {
            return oldEvaluations.ContainsKey(key);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public void Add(double[] key, double value)
        {
            if (Parameters.MaxFunctionDataStore == 0) return;
            oldEvaluations.Add(key, value);
            if (queue.Count >= Parameters.MaxFunctionDataStore)
            {
                SearchIO.output("reducing queue...", 4);
                for (int i = 0; i < Parameters.FunctionStoreCleanOutStepDown; i++)
                    Remove(queue.Dequeue());}
            queue.Enqueue(key);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public bool Remove(double[] key)
        {
            return oldEvaluations.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool TryGetValue(double[] key, out double value)
        {
            value = double.NaN;
            if (!oldEvaluations.ContainsKey(key)) return false;
            value = oldEvaluations[key];
            return true;
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> is not found.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public double this[double[] key]
        {
            get { return oldEvaluations[key]; }
            set { oldEvaluations[key] = value; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>The keys.</value>
        public ICollection<double[]> Keys { get { return oldEvaluations.Keys; } }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>The values.</value>
        public ICollection<double> Values { get { return oldEvaluations.Values; } }

        #endregion
    }
}
