using System;
using System.Collections;
using System.Collections.Generic;

namespace OptimizationToolbox
{
    internal class optFunctionData : IDictionary<double[], double>
    {
        readonly Dictionary<double[], double> oldEvaluations;
        readonly Queue<double[]> queue;
        private const long maxSize = 100000;
        private const long sizeStepDown = 1000;

        internal long numEvals { get; set; }
        internal double finiteDiffStepSize { get; set; }
        internal differentiate findDerivBy { get; set; }

        internal optFunctionData(IOptFunction function, IEqualityComparer<double[]> comparer,
            double finiteDiffStepSize, differentiate findDerivBy)
        {
            if (typeof(IDifferentiable).IsInstanceOfType(function))
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
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<double[], double>> GetEnumerator()
        {
            return oldEvaluations.GetEnumerator();
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

        #region Implementation of ICollection<KeyValuePair<double[],double>>
        public void Add(KeyValuePair<double[], double> item)
        {
            Add(item.Key, item.Value);
        }
        public void Clear()
        {
            oldEvaluations.Clear();
            numEvals = 0;
        }
        public bool Contains(KeyValuePair<double[], double> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<double[], double>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public bool Remove(KeyValuePair<double[], double> item)
        {
            return Remove(item.Key);
        }
        public int Count
        {
            get { return oldEvaluations.Count; }
        }
        public bool IsReadOnly { get { return false; } }

        #endregion

        #region Implementation of IDictionary<double[],double>

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception>
        public bool ContainsKey(double[] key)
        {
            return oldEvaluations.ContainsKey(key);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.
        ///                 </param><param name="value">The object to use as the value of the element to add.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        ///                 </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///                 </exception>
        public void Add(double[] key, double value)
        {
            oldEvaluations.Add(key, value);
            if (queue.Count >= maxSize)
                for (int i = 0; i < sizeStepDown; i++)
                    Remove(queue.Dequeue());
            queue.Enqueue(key);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///                 </exception>
        public bool Remove(double[] key)
        {
            return oldEvaluations.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.
        ///                 </param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception>
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
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.
        ///                 </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.
        ///                 </exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.
        ///                 </exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///                 </exception>
        public double this[double[] key]
        {
            get { return oldEvaluations[key]; }
            set { oldEvaluations[key] = value; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<double[]> Keys { get { return oldEvaluations.Keys; } }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<double> Values { get { return oldEvaluations.Values; } }

        #endregion
    }
}
