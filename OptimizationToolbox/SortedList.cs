﻿/*************************************************************************
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
    /// Represents a collection of key/value pairs that are sorted by key based on the associated <see cref="T:System.Collections.Generic.IComparer`1"/> implementation.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam><typeparam name="TValue">The type of values in the collection.</typeparam>
    internal class SortedList<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        private static TKey[] emptyKeys = new TKey[0];
        private static TValue[] emptyValues = new TValue[0];
        private TKey[] keys;
        private TValue[] values;
        private int _size;
        private int version;
        private IComparer<TKey> comparer;
        private SortedList<TKey, TValue>.KeyList keyList;
        private SortedList<TKey, TValue>.ValueList valueList;
        private object _syncRoot;
        private const int _defaultCapacity = 4;
        private const int MaxArrayLength = 2146435071;

        /// <summary>
        /// Gets or sets the number of elements that the <see cref="T:System.Collections.Generic.SortedList`2"/> can contain.
        /// </summary>
        /// 
        /// <returns>
        /// The number of elements that the <see cref="T:System.Collections.Generic.SortedList`2"/> can contain.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><see cref="P:System.Collections.Generic.SortedList`2.Capacity"/> is set to a value that is less than <see cref="P:System.Collections.Generic.SortedList`2.Count"/>.</exception><exception cref="T:System.OutOfMemoryException">There is not enough memory available on the system.</exception>
        public int Capacity
        {
            get
            {
                return this.keys.Length;
            }
            set
            {
                if (value == this.keys.Length)
                    return;
                if (value < this._size)
                    throw new ArgumentOutOfRangeException();
                if (value > 0)
                {
                    TKey[] keyArray = new TKey[value];
                    TValue[] objArray = new TValue[value];
                    if (this._size > 0)
                    {
                        Array.Copy((Array)this.keys, 0, (Array)keyArray, 0, this._size);
                        Array.Copy((Array)this.values, 0, (Array)objArray, 0, this._size);
                    }
                    this.keys = keyArray;
                    this.values = objArray;
                }
                else
                {
                    this.keys = SortedList<TKey, TValue>.emptyKeys;
                    this.values = SortedList<TKey, TValue>.emptyValues;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IComparer`1"/> for the sorted list.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.IComparable`1"/> for the current <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </returns>
        public IComparer<TKey> Comparer
        {
            get
            {
                return this.comparer;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return this._size;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IList`1"/> containing the keys in the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </returns>
        public IList<TKey> Keys
        {
            get
            {
                return (IList<TKey>)this.GetKeyListHelper();
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return (ICollection<TKey>)this.GetKeyListHelper();
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return (ICollection)this.GetKeyListHelper();
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IList`1"/> containing the values in the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </returns>
        public IList<TValue> Values
        {
            get
            {
                return (IList<TValue>)this.GetValueListHelper();
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return (ICollection<TValue>)this.GetValueListHelper();
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return (ICollection)this.GetValueListHelper();
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), (object)null);
                return this._syncRoot;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// 
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="T:System.Collections.Generic.KeyNotFoundException"/> and a set operation creates a new element using the specified key.
        /// </returns>
        /// <param name="key">The key whose value to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> does not exist in the collection.</exception>
        public TValue this[TKey key]
        {
            get
            {
                int index = this.IndexOfKey(key);
                if (index >= 0)
                    return this.values[index];
                throw new Exception();
                return default(TValue);
            }
            set
            {
                if ((object)key == null)
                    throw new ArgumentNullException();
                int index = Array.BinarySearch<TKey>(this.keys, 0, this._size, key, this.comparer);
                if (index >= 0)
                {
                    this.values[index] = value;
                    ++this.version;
                }
                else
                    this.Insert(~index, key, value);
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (SortedList<TKey, TValue>.IsCompatibleKey(key))
                {
                    int index = this.IndexOfKey((TKey)key);
                    if (index >= 0)
                        return (object)this.values[index];
                }
                return (object)null;
            }
            set
            {
                if (!SortedList<TKey, TValue>.IsCompatibleKey(key))
                    throw new ArgumentNullException();
                throw new Exception();
                try
                {
                    TKey index = (TKey)key;
                    try
                    {
                        this[index] = (TValue)value;
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new ArgumentException();
                    }
                }
                catch (InvalidCastException ex)
                {
                    throw new ArgumentException();
                }
            }
        }

        static SortedList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        public SortedList()
        {
            this.keys = SortedList<TKey, TValue>.emptyKeys;
            this.values = SortedList<TKey, TValue>.emptyValues;
            this._size = 0;
            this.comparer = (IComparer<TKey>)Comparer<TKey>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the specified initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.SortedList`2"/> can contain.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public SortedList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException();
            this.keys = new TKey[capacity];
            this.values = new TValue[capacity];
            this.comparer = (IComparer<TKey>)Comparer<TKey>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the default initial capacity, and uses the specified <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing keys.-or-null to use the default <see cref="T:System.Collections.Generic.Comparer`1"/> for the type of the key.</param>
        public SortedList(IComparer<TKey> comparer)
            : this()
        {
            if (comparer == null)
                return;
            this.comparer = comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that is empty, has the specified initial capacity, and uses the specified <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.SortedList`2"/> can contain.</param><param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing keys.-or-null to use the default <see cref="T:System.Collections.Generic.Comparer`1"/> for the type of the key.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public SortedList(int capacity, IComparer<TKey> comparer)
            : this(comparer)
        {
            this.Capacity = capacity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that contains elements copied from the specified <see cref="T:System.Collections.Generic.IDictionary`2"/>, has sufficient capacity to accommodate the number of elements copied, and uses the default <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2"/> whose elements are copied to the new <see cref="T:System.Collections.Generic.SortedList`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="dictionary"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        public SortedList(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, (IComparer<TKey>)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedList`2"/> class that contains elements copied from the specified <see cref="T:System.Collections.Generic.IDictionary`2"/>, has sufficient capacity to accommodate the number of elements copied, and uses the specified <see cref="T:System.Collections.Generic.IComparer`1"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2"/> whose elements are copied to the new <see cref="T:System.Collections.Generic.SortedList`2"/>.</param><param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing keys.-or-null to use the default <see cref="T:System.Collections.Generic.Comparer`1"/> for the type of the key.</param><exception cref="T:System.ArgumentNullException"><paramref name="dictionary"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        public SortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
            : this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException();
            dictionary.Keys.CopyTo(this.keys, 0);
            dictionary.Values.CopyTo(this.values, 0);
            // Array.Sort<TKey, TValue>(this.keys, this.values, comparer);
            this._size = dictionary.Count;
        }

        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param><param name="value">The value of the element to add. The value can be null for reference types.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.SortedList`2"/>.</exception>
        public void Add(TKey key, TValue value)
        {
            if ((object)key == null)
                throw new ArgumentNullException();
            int num = Array.BinarySearch<TKey>(this.keys, 0, this._size, key, this.comparer);
            if (num >= 0)
                throw new ArgumentException();
            this.Insert(~num, key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            this.Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = this.IndexOfKey(keyValuePair.Key);
            return index >= 0 && EqualityComparer<TValue>.Default.Equals(this.values[index], keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int index = this.IndexOfKey(keyValuePair.Key);
            if (index < 0 || !EqualityComparer<TValue>.Default.Equals(this.values[index], keyValuePair.Value))
                return false;
            this.RemoveAt(index);
            return true;
        }

        void IDictionary.Add(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException();
            throw new Exception();
            try
            {
                TKey key1 = (TKey)key;
                try
                {
                    this.Add(key1, (TValue)value);
                }
                catch (InvalidCastException ex)
                {
                    throw new ArgumentException();
                }
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException();
            }
        }

        private SortedList<TKey, TValue>.KeyList GetKeyListHelper()
        {
            if (this.keyList == null)
                this.keyList = new SortedList<TKey, TValue>.KeyList(this);
            return this.keyList;
        }

        private SortedList<TKey, TValue>.ValueList GetValueListHelper()
        {
            if (this.valueList == null)
                this.valueList = new SortedList<TKey, TValue>.ValueList(this);
            return this.valueList;
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        public void Clear()
        {
            ++this.version;
            Array.Clear((Array)this.keys, 0, this._size);
            Array.Clear((Array)this.values, 0, this._size);
            this._size = 0;
        }

        bool IDictionary.Contains(object key)
        {
            if (SortedList<TKey, TValue>.IsCompatibleKey(key))
                return this.ContainsKey((TKey)key);
            else
                return false;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.SortedList`2"/> contains a specific key.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.SortedList`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedList`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(TKey key)
        {
            return this.IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.SortedList`2"/> contains a specific value.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.SortedList`2"/> contains an element with the specified value; otherwise, false.
        /// </returns>
        /// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.SortedList`2"/>. The value can be null for reference types.</param>
        public bool ContainsValue(TValue value)
        {
            return this.IndexOfValue(value) >= 0;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException();
            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException();
            for (int index = 0; index < this.Count; ++index)
            {
                KeyValuePair<TKey, TValue> keyValuePair = new KeyValuePair<TKey, TValue>(this.keys[index], this.values[index]);
                array[arrayIndex + index] = keyValuePair;
            }
        }

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
            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException();
            KeyValuePair<TKey, TValue>[] keyValuePairArray = array as KeyValuePair<TKey, TValue>[];
            if (keyValuePairArray != null)
            {
                for (int index = 0; index < this.Count; ++index)
                    keyValuePairArray[index + arrayIndex] = new KeyValuePair<TKey, TValue>(this.keys[index], this.values[index]);
            }
            else
            {
                object[] objArray = array as object[];
                if (objArray == null)
                    throw new ArgumentException();
                try
                {
                    for (int index = 0; index < this.Count; ++index)
                        objArray[index + arrayIndex] = (object)new KeyValuePair<TKey, TValue>(this.keys[index], this.values[index]);
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw new ArgumentException();
                }
            }
        }

        private void EnsureCapacity(int min)
        {
            int num = this.keys.Length == 0 ? 4 : this.keys.Length * 2;
            if ((uint)num > 2146435071U)
                num = 2146435071;
            if (num < min)
                num = min;
            this.Capacity = num;
        }

        private TValue GetByIndex(int index)
        {
            if (index < 0 || index >= this._size)
                throw new ArgumentOutOfRangeException();
            return this.values[index];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerator`1"/> of type <see cref="T:System.Collections.Generic.KeyValuePair`2"/> for the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<TKey, TValue>>)new SortedList<TKey, TValue>.Enumerator(this, 1);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<TKey, TValue>>)new SortedList<TKey, TValue>.Enumerator(this, 1);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (IDictionaryEnumerator)new SortedList<TKey, TValue>.Enumerator(this, 2);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new SortedList<TKey, TValue>.Enumerator(this, 1);
        }

        private TKey GetKey(int index)
        {
            if (index < 0 || index >= this._size)
                throw new ArgumentOutOfRangeException();
            return this.keys[index];
        }

        /// <summary>
        /// Searches for the specified key and returns the zero-based index within the entire <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The zero-based index of <paramref name="key"/> within the entire <see cref="T:System.Collections.Generic.SortedList`2"/>, if found; otherwise, -1.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedList`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public int IndexOfKey(TKey key)
        {
            if ((object)key == null)
                throw new ArgumentNullException();
            int num = Array.BinarySearch<TKey>(this.keys, 0, this._size, key, this.comparer);
            if (num < 0)
                return -1;
            else
                return num;
        }

        /// <summary>
        /// Searches for the specified value and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value"/> within the entire <see cref="T:System.Collections.Generic.SortedList`2"/>, if found; otherwise, -1.
        /// </returns>
        /// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.SortedList`2"/>.  The value can be null for reference types.</param>
        public int IndexOfValue(TValue value)
        {
            return Array.IndexOf<TValue>(this.values, value, 0, this._size);
        }

        private void Insert(int index, TKey key, TValue value)
        {
            if (this._size == this.keys.Length)
                this.EnsureCapacity(this._size + 1);
            if (index < this._size)
            {
                Array.Copy((Array)this.keys, index, (Array)this.keys, index + 1, this._size - index);
                Array.Copy((Array)this.values, index, (Array)this.values, index + 1, this._size - index);
            }
            this.keys[index] = key;
            this.values[index] = value;
            ++this._size;
            ++this.version;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// 
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.SortedList`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = this.IndexOfKey(key);
            if (index >= 0)
            {
                value = this.values[index];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.SortedList`2.Count"/>.</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this._size)
                throw new ArgumentOutOfRangeException();
            --this._size;
            if (index < this._size)
            {
                Array.Copy((Array)this.keys, index + 1, (Array)this.keys, index, this._size - index);
                Array.Copy((Array)this.values, index + 1, (Array)this.values, index, this._size - index);
            }
            this.keys[this._size] = default(TKey);
            this.values[this._size] = default(TValue);
            ++this.version;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </summary>
        /// 
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.SortedList`2"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool Remove(TKey key)
        {
            int index = this.IndexOfKey(key);
            if (index >= 0)
                this.RemoveAt(index);
            return index >= 0;
        }

        void IDictionary.Remove(object key)
        {
            if (!SortedList<TKey, TValue>.IsCompatibleKey(key))
                return;
            this.Remove((TKey)key);
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.SortedList`2"/>, if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess()
        {
            if (this._size >= (int)((double)this.keys.Length * 0.9))
                return;
            this.Capacity = this._size;
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
                throw new ArgumentNullException();
            return key is TKey;
        }

        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
        {
            private SortedList<TKey, TValue> _sortedList;
            private TKey key;
            private TValue value;
            private int index;
            private int version;
            private int getEnumeratorRetType;
            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return (object)this.key;
                }
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return new DictionaryEntry((object)this.key, (object)this.value);
                }
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return new KeyValuePair<TKey, TValue>(this.key, this.value);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException();
                    if (this.getEnumeratorRetType == 2)
                        return (object)new DictionaryEntry((object)this.key, (object)this.value);
                    else
                        return (object)new KeyValuePair<TKey, TValue>(this.key, this.value);
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return (object)this.value;
                }
            }

            internal Enumerator(SortedList<TKey, TValue> sortedList, int getEnumeratorRetType)
            {
                this._sortedList = sortedList;
                this.index = 0;
                this.version = this._sortedList.version;
                this.getEnumeratorRetType = getEnumeratorRetType;
                this.key = default(TKey);
                this.value = default(TValue);
            }

            public void Dispose()
            {
                this.index = 0;
                this.key = default(TKey);
                this.value = default(TValue);
            }

            public bool MoveNext()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException();
                if ((uint)this.index < (uint)this._sortedList.Count)
                {
                    this.key = this._sortedList.keys[this.index];
                    this.value = this._sortedList.values[this.index];
                    ++this.index;
                    return true;
                }
                else
                {
                    this.index = this._sortedList.Count + 1;
                    this.key = default(TKey);
                    this.value = default(TValue);
                    return false;
                }
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException();
                this.index = 0;
                this.key = default(TKey);
                this.value = default(TValue);
            }
        }

        private sealed class SortedListKeyEnumerator : IEnumerator<TKey>, IDisposable, IEnumerator
        {
            private SortedList<TKey, TValue> _sortedList;
            private int index;
            private int version;
            private TKey currentKey;

            public TKey Current
            {
                get
                {
                    return this.currentKey;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return (object)this.currentKey;
                }
            }

            internal SortedListKeyEnumerator(SortedList<TKey, TValue> sortedList)
            {
                this._sortedList = sortedList;
                this.version = sortedList.version;
            }

            public void Dispose()
            {
                this.index = 0;
                this.currentKey = default(TKey);
            }

            public bool MoveNext()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException();
                if ((uint)this.index < (uint)this._sortedList.Count)
                {
                    this.currentKey = this._sortedList.keys[this.index];
                    ++this.index;
                    return true;
                }
                else
                {
                    this.index = this._sortedList.Count + 1;
                    this.currentKey = default(TKey);
                    return false;
                }
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException();
                this.index = 0;
                this.currentKey = default(TKey);
            }
        }

        private sealed class SortedListValueEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
        {
            private SortedList<TKey, TValue> _sortedList;
            private int index;
            private int version;
            private TValue currentValue;

            public TValue Current
            {
                get
                {
                    return this.currentValue;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this.index == 0 || this.index == this._sortedList.Count + 1)
                        throw new InvalidOperationException();
                    return (object)this.currentValue;
                }
            }

            internal SortedListValueEnumerator(SortedList<TKey, TValue> sortedList)
            {
                this._sortedList = sortedList;
                this.version = sortedList.version;
            }

            public void Dispose()
            {
                this.index = 0;
                this.currentValue = default(TValue);
            }

            public bool MoveNext()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException();
                if ((uint)this.index < (uint)this._sortedList.Count)
                {
                    this.currentValue = this._sortedList.values[this.index];
                    ++this.index;
                    return true;
                }
                else
                {
                    this.index = this._sortedList.Count + 1;
                    this.currentValue = default(TValue);
                    return false;
                }
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._sortedList.version)
                    throw new InvalidOperationException();
                this.index = 0;
                this.currentValue = default(TValue);
            }
        }

        private sealed class KeyList : IList<TKey>, ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
        {
            private SortedList<TKey, TValue> _dict;

            public int Count
            {
                get
                {
                    return this._dict._size;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return ((ICollection)this._dict).SyncRoot;
                }
            }

            public TKey this[int index]
            {
                get
                {
                    return this._dict.GetKey(index);
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            internal KeyList(SortedList<TKey, TValue> dictionary)
            {
                this._dict = dictionary;
            }

            public void Add(TKey key)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TKey key)
            {
                return this._dict.ContainsKey(key);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                Array.Copy((Array)this._dict.keys, 0, (Array)array, arrayIndex, this._dict.Count);
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if (array != null)
                {
                    if (array.Rank != 1)
                        throw new ArgumentException();
                }
                try
                {
                    Array.Copy((Array)this._dict.keys, 0, array, arrayIndex, this._dict.Count);
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw new ArgumentException();
                }
            }

            public void Insert(int index, TKey value)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return (IEnumerator<TKey>)new SortedList<TKey, TValue>.SortedListKeyEnumerator(this._dict);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)new SortedList<TKey, TValue>.SortedListKeyEnumerator(this._dict);
            }

            public int IndexOf(TKey key)
            {
                if ((object)key == null)
                    throw new ArgumentNullException();
                int num = Array.BinarySearch<TKey>(this._dict.keys, 0, this._dict.Count, key, this._dict.comparer);
                if (num >= 0)
                    return num;
                else
                    return -1;
            }

            public bool Remove(TKey key)
            {
                throw new NotSupportedException();
                return false;
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class ValueList : IList<TValue>, ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable
        {
            private SortedList<TKey, TValue> _dict;

            public int Count
            {
                get
                {
                    return this._dict._size;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return ((ICollection)this._dict).SyncRoot;
                }
            }

            public TValue this[int index]
            {
                get
                {
                    return this._dict.GetByIndex(index);
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            internal ValueList(SortedList<TKey, TValue> dictionary)
            {
                this._dict = dictionary;
            }

            public void Add(TValue key)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TValue value)
            {
                return this._dict.ContainsValue(value);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                Array.Copy((Array)this._dict.values, 0, (Array)array, arrayIndex, this._dict.Count);
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if (array != null)
                {
                    if (array.Rank != 1)
                        throw new ArgumentException();
                }
                try
                {
                    Array.Copy((Array)this._dict.values, 0, array, arrayIndex, this._dict.Count);
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw new ArgumentException();
                }
            }

            public void Insert(int index, TValue value)
            {
                throw new NotSupportedException();
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return (IEnumerator<TValue>)new SortedList<TKey, TValue>.SortedListValueEnumerator(this._dict);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)new SortedList<TKey, TValue>.SortedListValueEnumerator(this._dict);
            }

            public int IndexOf(TValue value)
            {
                return Array.IndexOf<TValue>(this._dict.values, value, 0, this._dict.Count);
            }

            public bool Remove(TValue value)
            {
                throw new NotSupportedException();
                return false;
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
        }
    }
}


