// ***********************************************************************
// Assembly         : OptimizationToolbox
// Author           : campmatt
// Created          : 01-28-2021
//
// Last Modified By : campmatt
// Last Modified On : 01-28-2021
// ***********************************************************************
// <copyright file="SortedList.cs" company="OptimizationToolbox">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox PCL Version. It is a modified version from the source found 
 *     at:
 *     Type: System.Collections.Generic.SortedList`2
 *     Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
 *     MVID: 67296426-5FEC-4466-BD0C-69BBFD2659CF
 *     Original Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll
 *     
 *     Since SortedList is not included within the Portable .NET version, we
 *     are including it here.
 *************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace OptimizationToolbox
{
    /// <summary>
    /// Represents a collection of key/value pairs that are sorted by key based on the associated
    /// <see cref="T:System.Collections.Generic.IComparer`1" /> implementation.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TValue">The type of values in the collection.</typeparam>
    internal class SortedList<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        /// <summary>
        /// The empty keys
        /// </summary>
        private static readonly TKey[] emptyKeys = new TKey[0];
        /// <summary>
        /// The empty values
        /// </summary>
        private static readonly TValue[] emptyValues = new TValue[0];
        /// <summary>
        /// The comparer
        /// </summary>
        private readonly IComparer<TKey> comparer;
        /// <summary>
        /// The size
        /// </summary>
        private int _size;
        /// <summary>
        /// The synchronize root
        /// </summary>
        private object _syncRoot;
        /// <summary>
        /// The key list
        /// </summary>
        private KeyList keyList;
        /// <summary>
        /// The keys
        /// </summary>
        private TKey[] keys;
        /// <summary>
        /// The value list
        /// </summary>
        private ValueList valueList;
        /// <summary>
        /// The values
        /// </summary>
        private TValue[] values;
        /// <summary>
        /// The version
        /// </summary>
        private int version;

        /// <summary>
        /// Initializes static members of the <see cref="SortedList{TKey, TValue}"/> class.
        /// </summary>
        static SortedList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that is empty, has
        /// the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        internal SortedList()
        {
            keys = emptyKeys;
            values = emptyValues;
            _size = 0;
            comparer = Comparer<TKey>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that is empty, has
        /// the specified initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.SortedList`2" />
        /// can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity" /> is less than zero.</exception>
        internal SortedList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException();
            keys = new TKey[capacity];
            values = new TValue[capacity];
            comparer = Comparer<TKey>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that is empty, has
        /// the default initial capacity, and uses the specified <see cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing
        /// keys.-or-null to use the default <see cref="T:System.Collections.Generic.Comparer`1" /> for the type of the key.</param>
        internal SortedList(IComparer<TKey> comparer)
            : this()
        {
            if (comparer == null)
                return;
            this.comparer = comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that is empty, has
        /// the specified initial capacity, and uses the specified <see cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.SortedList`2" />
        /// can contain.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing
        /// keys.-or-null to use the default <see cref="T:System.Collections.Generic.Comparer`1" /> for the type of the key.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity" /> is less than zero.</exception>
        internal SortedList(int capacity, IComparer<TKey> comparer)
            : this(comparer)
        {
            Capacity = capacity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that contains
        /// elements copied from the specified <see cref="T:System.Collections.Generic.IDictionary`2" />, has sufficient
        /// capacity to accommodate the number of elements copied, and uses the default
        /// <see cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the
        /// new <see cref="T:System.Collections.Generic.SortedList`2" />.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="dictionary" /> contains one or more duplicate keys.</exception>
        internal SortedList(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2" /> class that contains
        /// elements copied from the specified <see cref="T:System.Collections.Generic.IDictionary`2" />, has sufficient
        /// capacity to accommodate the number of elements copied, and uses the specified
        /// <see cref="T:System.Collections.Generic.IComparer`1" />.
        /// </summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the
        /// new <see cref="T:System.Collections.Generic.SortedList`2" />.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing
        /// keys.-or-null to use the default <see cref="T:System.Collections.Generic.Comparer`1" /> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="dictionary" /> contains one or more duplicate keys.</exception>
        internal SortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
            : this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException();
            dictionary.Keys.CopyTo(keys, 0);
            dictionary.Values.CopyTo(values, 0);
            // Array.Sort<TKey, TValue>(this.keys, this.values, comparer);
            _size = dictionary.Count;
        }

        /// <summary>
        /// Gets or sets the number of elements that the <see cref="T:System.Collections.Generic.SortedList`2" /> can contain.
        /// </summary>
        /// <value>The capacity.</value>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><see cref="P:System.Collections.Generic.SortedList`2.Capacity" /> is set to a value that is less than
        /// <see cref="P:System.Collections.Generic.SortedList`2.Count" />.</exception>
        /// <exception cref="T:System.OutOfMemoryException">There is not enough memory available on the system.</exception>
        internal int Capacity
        {
            get { return keys.Length; }
            set
            {
                if (value == keys.Length)
                    return;
                if (value < _size)
                    throw new ArgumentOutOfRangeException();
                if (value > 0)
                {
                    var keyArray = new TKey[value];
                    var objArray = new TValue[value];
                    if (_size > 0)
                    {
                        Array.Copy(keys, 0, keyArray, 0, _size);
                        Array.Copy(values, 0, objArray, 0, _size);
                    }
                    keys = keyArray;
                    values = objArray;
                }
                else
                {
                    keys = emptyKeys;
                    values = emptyValues;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IComparer`1" /> for the sorted list.
        /// </summary>
        /// <value>The comparer.</value>
        internal IComparer<TKey> Comparer
        {
            get { return comparer; }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        /// <value>The keys.</value>
        internal IList<TKey> Keys
        {
            get { return GetKeyListHelper(); }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        /// <value>The values.</value>
        internal IList<TValue> Values
        {
            get { return GetValueListHelper(); }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>The keys.</value>
        ICollection IDictionary.Keys
        {
            get { return GetKeyListHelper(); }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>The values.</value>
        ICollection IDictionary.Values
        {
            get { return GetValueListHelper(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> object has a fixed size.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
        /// </summary>
        /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        /// <value>The synchronize root.</value>
        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                return _syncRoot;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        object IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    int index = IndexOfKey((TKey) key);
                    if (index >= 0)
                        return values[index];
                }
                return null;
            }
            set
            {
                if (!IsCompatibleKey(key))
                    throw new ArgumentNullException();
                throw new Exception();
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" /> object.
        /// </summary>
        /// <param name="key">The <see cref="T:System.Object" /> to use as the key of the element to add.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to use as the value of the element to add.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        void IDictionary.Add(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException();
            throw new Exception();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IDictionary" /> object contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary" /> object.</param>
        /// <returns><see langword="true" /> if the <see cref="T:System.Collections.IDictionary" /> contains an element with the key; otherwise, <see langword="false" />.</returns>
        bool IDictionary.Contains(object key)
        {
            if (IsCompatibleKey(key))
                return ContainsKey((TKey) key);
            return false;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (array.Rank != 1)
                throw new ArgumentException();
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException();
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException();
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException();
            var keyValuePairArray = array as KeyValuePair<TKey, TValue>[];
            if (keyValuePairArray != null)
            {
                for (int index = 0; index < Count; ++index)
                    keyValuePairArray[index + arrayIndex] = new KeyValuePair<TKey, TValue>(keys[index], values[index]);
            }
            else
            {
                var objArray = array as object[];
                if (objArray == null)
                    throw new ArgumentException();
                try
                {
                    for (int index = 0; index < Count; ++index)
                        objArray[index + arrayIndex] = new KeyValuePair<TKey, TValue>(keys[index], values[index]);
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the <see cref="T:System.Collections.IDictionary" /> object.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the <see cref="T:System.Collections.IDictionary" /> object.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" /> object.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        void IDictionary.Remove(object key)
        {
            if (!IsCompatibleKey(key))
                return;
            Remove((TKey) key);
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _size; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>The keys.</value>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return GetKeyListHelper(); }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value>The values.</value>
        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return GetValueListHelper(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get or set.</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a
        /// <see cref="T:System.Collections.Generic.KeyNotFoundException" /> and a set operation creates a new element using
        /// the specified key.</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and
        /// <paramref name="key" /> does not exist in the collection.</exception>
        public TValue this[TKey key]
        {
            get
            {
                int index = IndexOfKey(key);
                if (index >= 0)
                    return values[index];
                throw new Exception();
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException();
                int index = Array.BinarySearch(keys, 0, _size, key, comparer);
                if (index >= 0)
                {
                    values[index] = value;
                    ++version;
                }
                else
                    Insert(~index, key, value);
            }
        }

        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the
        /// <see cref="T:System.Collections.Generic.SortedList`2" />.</exception>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException();
            int num = Array.BinarySearch(keys, 0, _size, key, comparer);
            if (num >= 0)
                throw new ArgumentException();
            Insert(~num, key, value);
        }

        /// <summary>
        /// Adds the specified key value pair.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns><c>true</c> if [contains] [the specified key value pair]; otherwise, <c>false</c>.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = IndexOfKey(keyValuePair.Key);
            return index >= 0 && EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value);
        }

        /// <summary>
        /// Removes the specified key value pair.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = IndexOfKey(keyValuePair.Key);
            if (index < 0 || !EqualityComparer<TValue>.Default.Equals(values[index], keyValuePair.Value))
                return false;
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        public void Clear()
        {
            ++version;
            Array.Clear(keys, 0, _size);
            Array.Clear(values, 0, _size);
            _size = 0;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.SortedList`2" /> contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedList`2" />.</param>
        /// <returns>true if the <see cref="T:System.Collections.Generic.SortedList`2" /> contains an element with the specified key;
        /// otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool ContainsKey(TKey key)
        {
            return IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException();
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException();
            for (int index = 0; index < Count; ++index)
            {
                var keyValuePair = new KeyValuePair<TKey, TValue>(keys[index], values[index]);
                array[arrayIndex + index] = keyValuePair;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, 1);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, 1);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed
        /// uninitialized.</param>
        /// <returns>true if the <see cref="T:System.Collections.Generic.SortedList`2" /> contains an element with the specified key;
        /// otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = IndexOfKey(key);
            if (index >= 0)
            {
                value = values[index];
                return true;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false.  This method also returns false if
        /// <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.SortedList`2" />.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool Remove(TKey key)
        {
            int index = IndexOfKey(key);
            if (index >= 0)
                RemoveAt(index);
            return index >= 0;
        }

        /// <summary>
        /// Gets the key list helper.
        /// </summary>
        /// <returns>KeyList.</returns>
        private KeyList GetKeyListHelper()
        {
            return keyList ?? (keyList = new KeyList(this));
        }

        /// <summary>
        /// Gets the value list helper.
        /// </summary>
        /// <returns>ValueList.</returns>
        private ValueList GetValueListHelper()
        {
            return valueList ?? (valueList = new ValueList(this));
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.SortedList`2" /> contains a specific value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.SortedList`2" />. The value can
        /// be null for reference types.</param>
        /// <returns>true if the <see cref="T:System.Collections.Generic.SortedList`2" /> contains an element with the specified value;
        /// otherwise, false.</returns>
        internal bool ContainsValue(TValue value)
        {
            return IndexOfValue(value) >= 0;
        }

        /// <summary>
        /// Ensures the capacity.
        /// </summary>
        /// <param name="min">The minimum.</param>
        private void EnsureCapacity(int min)
        {
            int num = keys.Length == 0 ? 4 : keys.Length*2;
            if ((uint) num > 2146435071U)
                num = 2146435071;
            if (num < min)
                num = min;
            Capacity = num;
        }

        /// <summary>
        /// Gets the index of the by.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>TValue.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private TValue GetByIndex(int index)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException();
            return values[index];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1" /> of type
        /// <see cref="T:System.Collections.Generic.KeyValuePair`2" /> for the
        /// <see cref="T:System.Collections.Generic.SortedList`2" />.</returns>
        internal IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this, 1);
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>TKey.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private TKey GetKey(int index)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException();
            return keys[index];
        }

        /// <summary>
        /// Searches for the specified key and returns the zero-based index within the entire
        /// <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedList`2" />.</param>
        /// <returns>The zero-based index of <paramref name="key" /> within the entire
        /// <see cref="T:System.Collections.Generic.SortedList`2" />, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        internal int IndexOfKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException();
            int num = Array.BinarySearch(keys, 0, _size, key, comparer);
            if (num < 0)
                return -1;
            return num;
        }

        /// <summary>
        /// Searches for the specified value and returns the zero-based index of the first occurrence within the entire
        /// <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.SortedList`2" />.  The value can
        /// be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="value" /> within the entire
        /// <see cref="T:System.Collections.Generic.SortedList`2" />, if found; otherwise, -1.</returns>
        internal int IndexOfValue(TValue value)
        {
            return Array.IndexOf(values, value, 0, _size);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void Insert(int index, TKey key, TValue value)
        {
            if (_size == keys.Length)
                EnsureCapacity(_size + 1);
            if (index < _size)
            {
                Array.Copy(keys, index, keys, index + 1, _size - index);
                Array.Copy(values, index, values, index + 1, _size - index);
            }
            keys[index] = key;
            values[index] = value;
            ++_size;
            ++version;
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.Generic.SortedList`2" />.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> is less than zero.-or-
        /// <paramref name="index" /> is equal to or greater than
        /// <see cref="P:System.Collections.Generic.SortedList`2.Count" />.</exception>
        internal void RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException();
            --_size;
            if (index < _size)
            {
                Array.Copy(keys, index + 1, keys, index, _size - index);
                Array.Copy(values, index + 1, values, index, _size - index);
            }
            keys[_size] = default(TKey);
            values[_size] = default(TValue);
            ++version;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.SortedList`2" />,
        /// if that number is less than 90 percent of current capacity.
        /// </summary>
        internal void TrimExcess()
        {
            if (_size >= (int) (keys.Length*0.9))
                return;
            Capacity = _size;
        }

        /// <summary>
        /// Determines whether [is compatible key] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if [is compatible key] [the specified key]; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
                throw new ArgumentNullException();
            return key is TKey;
        }

        /// <summary>
        /// Struct Enumerator
        /// Implements the <see cref="System.Collections.Generic.IEnumerator{System.Collections.Generic.KeyValuePair{TKey, TValue}}" />
        /// Implements the <see cref="System.Collections.IDictionaryEnumerator" />
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEnumerator{System.Collections.Generic.KeyValuePair{TKey, TValue}}" />
        /// <seealso cref="System.Collections.IDictionaryEnumerator" />
        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            /// <summary>
            /// The sorted list
            /// </summary>
            private readonly SortedList<TKey, TValue> _sortedList;
            /// <summary>
            /// The get enumerator ret type
            /// </summary>
            private readonly int getEnumeratorRetType;
            /// <summary>
            /// The version
            /// </summary>
            private readonly int version;
            /// <summary>
            /// The index
            /// </summary>
            private int index;
            /// <summary>
            /// The key
            /// </summary>
            private TKey key;
            /// <summary>
            /// The value
            /// </summary>
            private TValue value;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="sortedList">The sorted list.</param>
            /// <param name="getEnumeratorRetType">Type of the get enumerator ret.</param>
            internal Enumerator(SortedList<TKey, TValue> sortedList, int getEnumeratorRetType)
            {
                _sortedList = sortedList;
                index = 0;
                version = _sortedList.version;
                this.getEnumeratorRetType = getEnumeratorRetType;
                key = default(TKey);
                value = default(TValue);
            }

            /// <summary>
            /// Gets the key.
            /// </summary>
            /// <value>The key.</value>
            /// <exception cref="InvalidOperationException"></exception>
            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (index == 0 || index == _sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return key;
                }
            }

            /// <summary>
            /// Gets the entry.
            /// </summary>
            /// <value>The entry.</value>
            /// <exception cref="InvalidOperationException"></exception>
            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (index == 0 || index == _sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return new DictionaryEntry(key, value);
                }
            }

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <value>The value.</value>
            /// <exception cref="InvalidOperationException"></exception>
            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (index == 0 || index == _sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return value;
                }
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            /// <value>The current.</value>
            public KeyValuePair<TKey, TValue> Current
            {
                get { return new KeyValuePair<TKey, TValue>(key, value); }
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            /// <value>The current.</value>
            /// <exception cref="InvalidOperationException"></exception>
            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == _sortedList.Count + 1)
                        throw new InvalidOperationException();
                    if (getEnumeratorRetType == 2)
                        return new DictionaryEntry(key, value);
                    return new KeyValuePair<TKey, TValue>(key, value);
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                index = 0;
                key = default(TKey);
                value = default(TValue);
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns><see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException"></exception>
            public bool MoveNext()
            {
                if (version != _sortedList.version)
                    throw new InvalidOperationException();
                if ((uint) index < (uint) _sortedList.Count)
                {
                    key = _sortedList.keys[index];
                    value = _sortedList.values[index];
                    ++index;
                    return true;
                }
                index = _sortedList.Count + 1;
                key = default(TKey);
                value = default(TValue);
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            void IEnumerator.Reset()
            {
                if (version != _sortedList.version)
                    throw new InvalidOperationException();
                index = 0;
                key = default(TKey);
                value = default(TValue);
            }
        }

        /// <summary>
        /// Class KeyList. This class cannot be inherited.
        /// Implements the <see cref="System.Collections.Generic.IList{TKey}" />
        /// Implements the <see cref="System.Collections.ICollection" />
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IList{TKey}" />
        /// <seealso cref="System.Collections.ICollection" />
        private sealed class KeyList : IList<TKey>, ICollection
        {
            /// <summary>
            /// The dictionary
            /// </summary>
            private readonly SortedList<TKey, TValue> _dict;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyList"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary.</param>
            internal KeyList(SortedList<TKey, TValue> dictionary)
            {
                _dict = dictionary;
            }

            /// <summary>
            /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
            /// </summary>
            /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            /// <summary>
            /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
            /// </summary>
            /// <value>The synchronize root.</value>
            object ICollection.SyncRoot
            {
                get { return ((ICollection) _dict).SyncRoot; }
            }

            /// <summary>
            /// Copies to.
            /// </summary>
            /// <param name="array">The array.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <exception cref="ArgumentException"></exception>
            /// <exception cref="ArgumentException"></exception>
            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if (array.Rank != 1)
                    throw new ArgumentException();
                try
                {
                    Array.Copy(_dict.keys, 0, array, arrayIndex, _dict.Count);
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException();
                }
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <value>The count.</value>
            public int Count
            {
                get { return _dict._size; }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public bool IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Gets or sets the <see cref="TKey"/> at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>TKey.</returns>
            /// <exception cref="NotSupportedException"></exception>
            public TKey this[int index]
            {
                get { return _dict.GetKey(index); }
                set { throw new NotSupportedException(); }
            }

            /// <summary>
            /// Adds the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <exception cref="NotSupportedException"></exception>
            public void Add(TKey key)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <exception cref="NotSupportedException"></exception>
            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Determines whether this instance contains the object.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.</returns>
            public bool Contains(TKey key)
            {
                return _dict.ContainsKey(key);
            }

            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                Array.Copy(_dict.keys, 0, array, arrayIndex, _dict.Count);
            }

            /// <summary>
            /// Inserts the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="value">The value.</param>
            /// <exception cref="NotSupportedException"></exception>
            public void Insert(int index, TKey value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            public IEnumerator<TKey> GetEnumerator()
            {
                return new SortedListKeyEnumerator(_dict);
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new SortedListKeyEnumerator(_dict);
            }

            /// <summary>
            /// Indexes the of.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>System.Int32.</returns>
            /// <exception cref="ArgumentNullException"></exception>
            public int IndexOf(TKey key)
            {
                if (key == null)
                    throw new ArgumentNullException();
                int num = Array.BinarySearch(_dict.keys, 0, _dict.Count, key, _dict.comparer);
                if (num >= 0)
                    return num;
                return -1;
            }

            /// <summary>
            /// Removes the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            /// <exception cref="NotSupportedException"></exception>
            public bool Remove(TKey key)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the item to remove.</param>
            /// <exception cref="NotSupportedException"></exception>
            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Class SortedListKeyEnumerator. This class cannot be inherited.
        /// Implements the <see cref="System.Collections.Generic.IEnumerator{TKey}" />
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEnumerator{TKey}" />
        private sealed class SortedListKeyEnumerator : IEnumerator<TKey>
        {
            /// <summary>
            /// The sorted list
            /// </summary>
            private readonly SortedList<TKey, TValue> _sortedList;
            /// <summary>
            /// The version
            /// </summary>
            private readonly int version;
            /// <summary>
            /// The current key
            /// </summary>
            private TKey currentKey;
            /// <summary>
            /// The index
            /// </summary>
            private int index;

            /// <summary>
            /// Initializes a new instance of the <see cref="SortedListKeyEnumerator"/> class.
            /// </summary>
            /// <param name="sortedList">The sorted list.</param>
            internal SortedListKeyEnumerator(SortedList<TKey, TValue> sortedList)
            {
                _sortedList = sortedList;
                version = sortedList.version;
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value>The current.</value>
            public TKey Current
            {
                get { return currentKey; }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value>The current.</value>
            /// <exception cref="InvalidOperationException"></exception>
            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == _sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return currentKey;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                index = 0;
                currentKey = default(TKey);
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns><see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException"></exception>
            public bool MoveNext()
            {
                if (version != _sortedList.version)
                    throw new InvalidOperationException();
                if ((uint) index < (uint) _sortedList.Count)
                {
                    currentKey = _sortedList.keys[index];
                    ++index;
                    return true;
                }
                index = _sortedList.Count + 1;
                currentKey = default(TKey);
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            void IEnumerator.Reset()
            {
                if (version != _sortedList.version)
                    throw new InvalidOperationException();
                index = 0;
                currentKey = default(TKey);
            }
        }

        /// <summary>
        /// Class SortedListValueEnumerator. This class cannot be inherited.
        /// Implements the <see cref="System.Collections.Generic.IEnumerator{TValue}" />
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEnumerator{TValue}" />
        private sealed class SortedListValueEnumerator : IEnumerator<TValue>
        {
            /// <summary>
            /// The sorted list
            /// </summary>
            private readonly SortedList<TKey, TValue> _sortedList;
            /// <summary>
            /// The version
            /// </summary>
            private readonly int version;
            /// <summary>
            /// The current value
            /// </summary>
            private TValue currentValue;
            /// <summary>
            /// The index
            /// </summary>
            private int index;

            /// <summary>
            /// Initializes a new instance of the <see cref="SortedListValueEnumerator"/> class.
            /// </summary>
            /// <param name="sortedList">The sorted list.</param>
            internal SortedListValueEnumerator(SortedList<TKey, TValue> sortedList)
            {
                _sortedList = sortedList;
                version = sortedList.version;
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value>The current.</value>
            public TValue Current
            {
                get { return currentValue; }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value>The current.</value>
            /// <exception cref="InvalidOperationException"></exception>
            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == _sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return currentValue;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                index = 0;
                currentValue = default(TValue);
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns><see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException"></exception>
            public bool MoveNext()
            {
                if (version != _sortedList.version)
                    throw new InvalidOperationException();
                if ((uint) index < (uint) _sortedList.Count)
                {
                    currentValue = _sortedList.values[index];
                    ++index;
                    return true;
                }
                index = _sortedList.Count + 1;
                currentValue = default(TValue);
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            void IEnumerator.Reset()
            {
                if (version != _sortedList.version)
                    throw new InvalidOperationException();
                index = 0;
                currentValue = default(TValue);
            }
        }

        /// <summary>
        /// Class ValueList. This class cannot be inherited.
        /// Implements the <see cref="System.Collections.Generic.IList{TValue}" />
        /// Implements the <see cref="System.Collections.ICollection" />
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IList{TValue}" />
        /// <seealso cref="System.Collections.ICollection" />
        private sealed class ValueList : IList<TValue>, ICollection
        {
            /// <summary>
            /// The dictionary
            /// </summary>
            private readonly SortedList<TKey, TValue> _dict;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueList"/> class.
            /// </summary>
            /// <param name="dictionary">The dictionary.</param>
            internal ValueList(SortedList<TKey, TValue> dictionary)
            {
                _dict = dictionary;
            }

            /// <summary>
            /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
            /// </summary>
            /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            /// <summary>
            /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
            /// </summary>
            /// <value>The synchronize root.</value>
            object ICollection.SyncRoot
            {
                get { return ((ICollection) _dict).SyncRoot; }
            }

            /// <summary>
            /// Copies to.
            /// </summary>
            /// <param name="array">The array.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <exception cref="ArgumentException"></exception>
            /// <exception cref="ArgumentException"></exception>
            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if (array.Rank != 1)
                    throw new ArgumentException();
                try
                {
                    Array.Copy(_dict.values, 0, array, arrayIndex, _dict.Count);
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException();
                }
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <value>The count.</value>
            public int Count
            {
                get { return _dict._size; }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public bool IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Gets or sets the <see cref="TValue"/> at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>TValue.</returns>
            /// <exception cref="NotSupportedException"></exception>
            public TValue this[int index]
            {
                get { return _dict.GetByIndex(index); }
                set { throw new NotSupportedException(); }
            }

            /// <summary>
            /// Adds the specified key.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <exception cref="NotSupportedException"></exception>
            public void Add(TValue key)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <exception cref="NotSupportedException"></exception>
            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Determines whether this instance contains the object.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
            public bool Contains(TValue value)
            {
                return _dict.ContainsValue(value);
            }

            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                Array.Copy(_dict.values, 0, array, arrayIndex, _dict.Count);
            }

            /// <summary>
            /// Inserts the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="value">The value.</param>
            /// <exception cref="NotSupportedException"></exception>
            public void Insert(int index, TValue value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            public IEnumerator<TValue> GetEnumerator()
            {
                return new SortedListValueEnumerator(_dict);
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new SortedListValueEnumerator(_dict);
            }

            /// <summary>
            /// Indexes the of.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>System.Int32.</returns>
            public int IndexOf(TValue value)
            {
                return Array.IndexOf(_dict.values, value, 0, _dict.Count);
            }

            /// <summary>
            /// Removes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            /// <exception cref="NotSupportedException"></exception>
            public bool Remove(TValue value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes at.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <exception cref="NotSupportedException"></exception>
            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
        }
    }
}