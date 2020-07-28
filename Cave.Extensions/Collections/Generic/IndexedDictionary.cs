using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cave.Collections.Generic
{
    /// <summary>Gets an indexed dictionary (a TKey, TValue dictionary supporting access to the KeyValuePair items by index).</summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("Count={Count}")]
    public class IndexedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        readonly Dictionary<TKey, TValue> dictionary;
        readonly List<TKey> keys;

        /// <summary>Gets the number of elements in the dictionary.</summary>
        public int Count => dictionary.Count;

        /// <summary>Removes all elements from the dictionary.</summary>
        public void Clear()
        {
            dictionary.Clear();
            keys.Clear();
        }

        /// <summary>Gets a value indicating whether the list is readonly or not.</summary>
        public bool IsReadOnly => false;

        /// <summary>Returns an enumerator that iterates through all items.</summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        #region IList<T1> implementation

        /// <summary>Returns the zero-based index of the first occurrence of the specified value.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IndexOf(TKey key) => keys.IndexOf(key);

        #endregion

        /// <summary>Gets the value at the specified index.</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue GetValueAt(int index) => dictionary[keys[index]];

        /// <summary>Sets the value at the specified index.</summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetValueAt(int index, TValue value) { dictionary[keys[index]] = value; }

        /// <summary>Gets the key at the specified index.</summary>
        /// <param name="index">index to read.</param>
        /// <returns>returns the key.</returns>
        public TKey GetKeyAt(int index) => keys[index];

        /// <summary>Gets the key and value at the specified index.</summary>
        /// <param name="index">index to read.</param>
        /// <param name="key">the key.</param>
        /// <param name="value">the value.</param>
        public void GetKeyValueAt(int index, out TKey key, out TValue value)
        {
            key = keys[index];
            value = dictionary[key];
        }

        class Enumerator : IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
        {
            readonly IndexedDictionary<TKey, TValue> dictionary;
            readonly IEnumerator<TKey> keyEnumerator;

            public Enumerator(IndexedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                keyEnumerator = this.dictionary.keys.GetEnumerator();
            }

            object IEnumerator.Current
            {
                get
                {
                    var key = keyEnumerator.Current;
                    return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
                }
            }

            public bool MoveNext() => keyEnumerator.MoveNext();

            public void Reset() { keyEnumerator.Reset(); }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    var key = keyEnumerator.Current;
                    return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
                }
            }

            public void Dispose() { keyEnumerator.Dispose(); }
        }

        #region IDictionary<T1, T2> implementation

        /// <summary>Initializes a new instance of the <see cref="IndexedDictionary{TKey, TValue}" /> class.</summary>
        public IndexedDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
            keys = new List<TKey>();
        }

        /// <summary>Adds the specified key and value to the dictionary.</summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
            keys.Add(key);
        }

        /// <summary>Determines whether the <see cref="Dictionary{TKey, TValue}" /> contains the specified key.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        /// <summary>Gets a collection containing the keys.</summary>
        public ICollection<TKey> Keys => keys.AsReadOnly();

        /// <summary>Removes the value with the specified key.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key) => dictionary.Remove(key) && keys.Remove(key);

        /// <summary>Gets the value associated with the specified key.</summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

        /// <summary>Gets a collection containing the values.</summary>
        public ICollection<TValue> Values => dictionary.Values;

        /// <summary>Gets/sets the value at the specified key.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key] = value;
                    return;
                }

                Add(key, value);
            }
        }

        /// <summary>Adds the specified key and value to the dictionary.</summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item) { Add(item.Key, item.Value); }

        /// <summary>Determines whether the dictionary contains a specific key value combination.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) => dictionary.ContainsKey(item.Key) && Equals(item.Value, dictionary[item.Key]);

        /// <summary>Copies the elements of the dictionary (unsorted) to an array, starting at the specified array index.</summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            var i = arrayIndex;
            foreach (var k in keys)
            {
                array[i++] = new KeyValuePair<TKey, TValue>(k, dictionary[k]);
            }
        }

        /// <summary>Removes the value with the specified key from the dictionary.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item) => dictionary.Remove(item.Key) && keys.Remove(item.Key);

        /// <summary>Returns an enumerator that iterates through the dictionary.</summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();

        #endregion
    }
}
