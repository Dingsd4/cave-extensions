using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cave.Collections.Generic
{
    /// <summary>Gets a generic typed set of objects.</summary>
    [DebuggerDisplay("Count={Count}")]
    [SuppressMessage("Design", "CA1000")]
    [SuppressMessage("Naming", "CA1710")]
    [SuppressMessage("Naming", "CA1716")]
#if NET20
    public sealed class Set<T> : IItemSet<T>
#else
    public sealed class Set<T> : IItemSet<T>
#endif
    {
        #region operators

        /// <summary>Gets a <see cref="Set{T}" /> containing all objects part of one of the specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static Set<T> operator |(Set<T> set1, Set<T> set2) => BitwiseOr(set1, set2);

        /// <summary>Gets a <see cref="Set{T}" /> containing only objects part of both of the specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static Set<T> operator &(Set<T> set1, Set<T> set2) => BitwiseAnd(set1, set2);

        /// <summary>
        ///     Gets a <see cref="Set{T}" /> containing all objects part of the first set after removing all objects present
        ///     at the second set.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static Set<T> operator -(Set<T> set1, Set<T> set2) => Subtract(set1, set2);

        /// <summary>Builds a new <see cref="Set{T}" /> containing only the items found exclusivly in one of both specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static Set<T> operator ^(Set<T> set1, Set<T> set2) => Xor(set1, set2);

        /// <summary>Checks two sets for equality.</summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static bool operator ==(Set<T> set1, Set<T> set2)
        {
            if (set1 is null)
            {
                return set2 is null;
            }

            if (set2 is null)
            {
                return false;
            }

            return set1.Equals(set2);
        }

        /// <summary>Checks two sets for inequality.</summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static bool operator !=(Set<T> set1, Set<T> set2) => !(set1 == set2);

        #endregion

        #region static Member

        /// <summary>Builds the union of two specified <see cref="Set{T}" />s.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static Set<T> BitwiseOr(IEnumerable<T> set1, IEnumerable<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }

            var result = new Set<T>();
            result.IncludeRange(set1);
            result.IncludeRange(set2);
            return result;
        }

        /// <summary>Builds the intersection of two specified <see cref="Set{T}" />s.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static Set<T> BitwiseAnd(IEnumerable<T> set1, IEnumerable<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }
#if NET20
            var s2 = new Set<T>(set2);
            var result = new Set<T>();
            foreach (var item in set1)
            {
                if (s2.Contains(item))
                {
                    result.Add(item);
                }
            }
            return result;
#else
            var result = new Set<T>();
            result.list.UnionWith(set1);
            result.list.IntersectWith(set2);
            return result;
#endif
        }

        /// <summary>
        ///     Subtracts the specified <see cref="Set{T}" /> from this one and returns a new <see cref="Set{T}" /> containing
        ///     the result.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static Set<T> Subtract(IEnumerable<T> set1, IEnumerable<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }
#if NET20
            var s2 = new Set<T>(set2);
            var result = new Set<T>();
            foreach (var item in set1)
            {
                if (!s2.Contains(item))
                {
                    result.Add(item);
                }
            }
            return result;
#else
            var result = new Set<T>();
            result.list.UnionWith(set1);
            result.list.ExceptWith(set2);
            return result;
#endif
        }

        /// <summary>Builds a new <see cref="Set{T}" /> containing only items found exclusivly in one of both specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static Set<T> Xor(IEnumerable<T> set1, IEnumerable<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }
#if NET20
            var counter = new Dictionary<T, int>();
            foreach (var item in set1)
            {
                int count;
                counter.TryGetValue(item, out count);
                counter[item] = ++count;
            }
            var result = new Set<T>();
            foreach (var item in counter)
            {
                if (item.Value == 0)
                {
                    result.Add(item.Key);
                }
            }
            return result;
#else
            var result = new Set<T>();
            result.list.UnionWith(set1);
            result.list.SymmetricExceptWith(set2);
            return result;
#endif
        }

        #endregion

        #region private Member

#if NET20
        Dictionary<T, byte> list = new Dictionary<T, byte>();
#else
        readonly HashSet<T> list = new HashSet<T>();
#endif

        #endregion

        #region constructors

        /// <summary>Initializes a new instance of the <see cref="Set{T}" /> class.</summary>
        public Set() { }

        /// <summary>Initializes a new instance of the <see cref="Set{T}" /> class.</summary>
        public Set(params T[] items) : this() => IncludeRange(items);

        /// <summary>Initializes a new instance of the <see cref="Set{T}" /> class.</summary>
        public Set(IEnumerable<T> items) : this() => IncludeRange(items);

        /// <summary>Initializes a new instance of the <see cref="Set{T}" /> class.</summary>
        public Set(params IEnumerable<T>[] blocks)
            : this()
        {
            foreach (var items in blocks)
            {
                IncludeRange(items);
            }
        }

        /// <summary>Initializes a new instance of the <see cref="Set{T}" /> class.</summary>
        public Set(T item, params IEnumerable<T>[] blocks)
            : this()
        {
            Include(item);
            foreach (var items in blocks)
            {
                IncludeRange(items);
            }
        }

        #endregion

        #region public Member

#if NET20
        /// <inheritdoc />
        public bool ContainsRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var allFound = true;
            foreach (var obj in items)
            {
                allFound &= Contains(obj);
            }
            return allFound;
        }
#else
        /// <inheritdoc />
        public bool ContainsRange(IEnumerable<T> items) => list.IsSubsetOf(items);
#endif


        /// <inheritdoc />
        public bool IsEmpty => list.Count == 0;

        /// <inheritdoc />
        public bool Contains(T item) =>
#if NET20
            list.ContainsKey(item);
#else
            list.Contains(item);
#endif


        /// <inheritdoc />
        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
#if NET20
            list.Add(item, 0);
#else
            if (!list.Add(item))
            {
                throw new ArgumentException("Item already present!");
            }
#endif
        }

        /// <inheritdoc />
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
#if NET20
            foreach (var obj in items)
            {
                Add(obj);
            }
#else
            foreach (var obj in items)
            {
                Add(obj);
            }
#endif
        }

        /// <inheritdoc />
        public void AddRange(params T[] items) => AddRange((IEnumerable<T>)items);

#if NET20
        /// <inheritdoc />
        public bool Include(T item)
        {
            var addNew = !list.ContainsKey(item);
            if (addNew)
            {
                list[item] = 0;
            }

            return addNew;
        }
#else
        /// <inheritdoc />
        public bool Include(T item) => list.Add(item);
#endif


        /// <inheritdoc />
        public int IncludeRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
#if NET20
            var count = 0;
            foreach (var item in items)
            {
                if (Include(item))
                {
                    count++;
                }
            }
            return count;
#else
            var oldCount = list.Count;
            list.UnionWith(items);
            return list.Count - oldCount;
#endif
        }

        /// <inheritdoc />
        public int IncludeRange(params T[] items) => IncludeRange((IEnumerable<T>) items);

#if NET20
        /// <inheritdoc />
        public bool TryRemove(T value)
        {
            if (Contains(value))
            {
                Remove(value);
                return true;
            }
            return false;
        }
#else
        /// <inheritdoc />
        public bool TryRemove(T value) => list.Remove(value);
#endif


        /// <inheritdoc />
        public int TryRemoveRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var count = 0;
            foreach (var item in items)
            {
                if (TryRemove(item))
                {
                    count++;
                }
            }

            return count;
        }

        /// <inheritdoc cref="ICollection{T}" />
        bool ICollection<T>.Remove(T item)
        {
            Remove(item);
            return true;
        }

        /// <inheritdoc cref="IItemSet{T}" />
        public void Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!list.Remove(item))
            {
                throw new KeyNotFoundException();
            }
        }

        /// <inheritdoc />
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var obj in items)
            {
                Remove(obj);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
#if NET20
            list = new Dictionary<T, byte>();
#else
            list.Clear();
            list.TrimExcess();
#endif
        }

        #endregion

        #region ICollection Member

#if NET20
        /// <summary>
        /// Copies all items present at the set to the specified array, starting at a specified index.
        /// </summary>
        /// <param name="array">one-dimensional array to copy to.</param>
        /// <param name="arrayIndex">the zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) => list.Keys.CopyTo(array, arrayIndex);
#else
        /// <summary>Copies all items present at the set to the specified array, starting at a specified index.</summary>
        /// <param name="array">one-dimensional array to copy to.</param>
        /// <param name="index">the zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int index) => list.CopyTo(array, index);
#endif

        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            foreach (object item in this)
            {
                array.SetValue(item, index++);
            }
        }

        /// <inheritdoc />
        public int Count => list.Count;

        /// <summary>Gets a value indicating whether the set is synchronized or not.</summary>
        public bool IsSynchronized => false;

        /// <summary>Gets this instance.</summary>
        public object SyncRoot => this;

        #endregion

        #region IEnumerable Member

        /// <summary>Gets an <see cref="IEnumerator" /> for this set.</summary>
        IEnumerator IEnumerable.GetEnumerator() =>
#if NET20
            list.Keys.GetEnumerator();
#else
            list.GetEnumerator();
#endif

        #endregion

        #region ICloneable Member

        /// <summary>Creates a copy of this set.</summary>
        public object Clone() =>
#if NET20
            new Set<T>(list.Keys);
#else
            new Set<T>(list);
#endif


        #endregion

        #region ICollection<T> Member

        /// <summary>Gets a value indicating whether the set is readonly or not.</summary>
        public bool IsReadOnly => false;

        #endregion

        #region IEnumerable<T> Member

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() =>
#if NET20
            list.Keys.GetEnumerator();
#else
            list.GetEnumerator();
#endif

        #endregion

        /// <summary>Gets an array of all elements present.</summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            var result = new T[Count];
            CopyTo(result, 0);
            return result;
        }

        /// <summary>Checks another Set{T} instance for equality.</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as Set<T>;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

#if NET20
        /// <summary>Checks another Set{T} instance for equality.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IItemSet<T> other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Count != Count)
            {
                return false;
            }

            return ContainsRange(other);
        }
#else
        /// <summary>Checks another Set{T} instance for equality.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IItemSet<T> other) => list.SetEquals(other);
#endif


        /// <summary>Gets the hash code of the base list.</summary>
        /// <returns></returns>
        public override int GetHashCode() => list.GetHashCode();
    }

    /// <summary>
    ///     Gets a typed 2D set. (Set A may only contain each value once. List B may contain any value multiple times. If
    ///     typeof(A) == typeof(B) a value may be present once at set A and multiple times at set B. Each value in set a is
    ///     linked to a value in list b via its index) This class is very similar to Dictionary{A, B}, in fact it uses one.
    ///     Additionally to the fast Name to value lookup it provides indexing like a list.
    /// </summary>
    [DebuggerDisplay("Count={Count}")]
    [SuppressMessage("Design", "CA1000")]
    [SuppressMessage("Naming", "CA1710")]
    [SuppressMessage("Naming", "CA1716")]
    public sealed class Set<TKey, TValue> : IItemSet<TKey, TValue>
    {
        readonly List<ItemPair<TKey, TValue>> list = new List<ItemPair<TKey, TValue>>();
        readonly Dictionary<TKey, ItemPair<TKey, TValue>> lookupA = new Dictionary<TKey, ItemPair<TKey, TValue>>();

        /// <summary>Clears the set.</summary>
        public void Clear()
        {
            list.Clear();
            lookupA.Clear();
        }

        /// <summary>Gets the index of the specified A object. This is an O(n) operation.</summary>
        /// <param name="key">'A' object to be found.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOfA(TKey key)
        {
            if (!lookupA.ContainsKey(key))
            {
                return -1;
            }

            return list.IndexOf(lookupA[key]);
        }

        /// <summary>Not supported. Use UniqueSet instead.</summary>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">Thrown if the set does not support indexing.</exception>
        /// <returns></returns>
        [Obsolete("Not supported. Use UniqueSet instead.")]
        public int IndexOfB(TValue value) => throw new NotSupportedException();

        /// <summary>Gets a read only collection for the A elements of the Set. This method is an O(1) operation;.</summary>
        public IList<TKey> ItemsA => new ReadOnlyListA<TKey, TValue>(this);

        /// <summary>Gets a read only collection for the B elements of the Set. This method is an O(1) operation;.</summary>
        public IList<TValue> ItemsB => new ReadOnlyListB<TKey, TValue>(this);

        /// <summary>Gets the number of elements actually present at the Set.</summary>
        public int Count => list.Count;

        /// <summary>Gets a value indicating whether the set is readonly or not.</summary>
        public bool IsReadOnly => false;

        /// <summary>Returns an enumerator that iterates through the set.</summary>
        /// <returns>An IEnumerator object that can be used to iterate through the set.</returns>
        public IEnumerator<ItemPair<TKey, TValue>> GetEnumerator() => list.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through the set.</summary>
        /// <returns>An IEnumerator object that can be used to iterate through the set.</returns>
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        /// <summary>Gets the index of the specified ItemPair. This is an O(1) operation.</summary>
        /// <param name="item">The ItemPair to search for.</param>
        /// <returns>The index of the ItemPair if found in the list; otherwise, -1.</returns>
        public int IndexOf(ItemPair<TKey, TValue> item) => list.IndexOf(item);

        /// <summary>
        ///     Inserts a new ItemPair at the specified index. This method needs a full index rebuild and is an O(n)
        ///     operation, where n is Count.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="item">The ItemPair to insert.</param>
        public void Insert(int index, ItemPair<TKey, TValue> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            try
            {
                lookupA.Add(item.A, item);
                list.Insert(index, item);
            }
            catch
            {
                Clear();
                throw;
            }
        }

        /// <summary>
        ///     Removes the ItemPair at the specified index. This method needs a full index rebuild and is an O(n) operation,
        ///     where n is Count.
        /// </summary>
        /// <param name="index">The index to remove the item.</param>
        public void RemoveAt(int index)
        {
            try
            {
                var item = list[index];
                list.RemoveAt(index);
                if (!lookupA.Remove(item.A))
                {
                    throw new KeyNotFoundException();
                }
            }
            catch
            {
                Clear();
                throw;
            }
        }

        /// <summary>
        ///     Accesses the ItemPair at the specified index. The getter is a O(1) operation. The setter needs a full index
        ///     rebuild and is an O(n) operation, where n is Count.
        /// </summary>
        /// <param name="index">The index of the ItemPair to be accessed.</param>
        /// <returns></returns>
        public ItemPair<TKey, TValue> this[int index]
        {
            get => list[index];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                var old = list[index];
                if (!lookupA.Remove(old.A))
                {
                    throw new KeyNotFoundException();
                }

                try
                {
                    lookupA.Add(value.A, value);
                }
                catch
                {
                    lookupA.Add(old.A, old);
                    throw;
                }

                list[index] = value;
            }
        }

        /// <summary>Adds an itempair to the set.</summary>
        /// <param name="item"></param>
        public void Add(ItemPair<TKey, TValue> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Add(item.A, item.B);
        }

        /// <summary>Checks whether an itempair is part of the set or not.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ItemPair<TKey, TValue> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return lookupA.ContainsKey(item.A) && Equals(lookupA[item.A].B, item.B);
        }

        /// <summary>Checks whether a specified A key is present.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsA(TKey key) => lookupA.ContainsKey(key);

        /// <summary>Not supported. Use UniqueSet.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not Supported. Use UniqueSet!")]
        public bool ContainsB(TValue value) => throw new NotSupportedException();

        /// <summary>Not Supported.</summary>
        /// <param name="value">The B index.</param>
        /// <returns></returns>
        [Obsolete("Not supported. Use UniqueSet instead!")]
        public ItemPair<TKey, TValue> GetB(TValue value) => throw new NotSupportedException();

        /// <summary>Gets the A element that is assigned to the specified B element. This method is an O(1) operation;.</summary>
        /// <param name="value">The A index.</param>
        /// <returns></returns>
        public ItemPair<TKey, TValue> GetA(TKey value) => lookupA[value];

        /// <summary>Copies all item of the set to the specified array starting at the specified index.</summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ItemPair<TKey, TValue>[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        /// <summary>Removes an itempair from the set.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ItemPair<TKey, TValue> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!lookupA.Remove(item.A))
            {
                throw new KeyNotFoundException();
            }

            return list.Remove(item);
        }

        /// <summary>Removes the item with the specified A key.</summary>
        /// <param name="key">The A key.</param>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        public void RemoveA(TKey key) => Remove(GetA(key));

        /// <summary>Not supported.</summary>
        /// <param name="value">The A key.</param>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        [Obsolete("Not supported. Use UniqueSet instead!")]
        public void RemoveB(TValue value) => throw new NotSupportedException();

        /// <summary>Adds an item pair to the end of the List. This is an O(1) operation.</summary>
        /// <param name="key">The A object to be added.</param>
        /// <param name="value">The B object to be added.</param>
        public void Add(TKey key, TValue value)
        {
            var node = new ItemPair<TKey, TValue>(key, value);
            lookupA.Add(key, node);
            list.Add(node);
        }

        /// <summary>Gets the index of the specified ItemPair. This is an O(1) operation.</summary>
        /// <param name="key">The A value of the ItemPair to search for.</param>
        /// <param name="value">The B value of the ItemPair to search for.</param>
        /// <returns>The index of the ItemPair if found in the list; otherwise, -1.</returns>
        public int IndexOf(TKey key, TValue value) => IndexOf(new ItemPair<TKey, TValue>(key, value));

        /// <summary>
        ///     Inserts a new ItemPair at the specified index. This method needs a full index rebuild and is an O(n)
        ///     operation, where n is Count.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="key">The A value of the ItemPair to insert.</param>
        /// <param name="value">The B value of the ItemPair to insert.</param>
        public void Insert(int index, TKey key, TValue value) => Insert(index, new ItemPair<TKey, TValue>(key, value));

        /// <summary>Checks whether an itempair is part of the set or not.</summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(TKey key, TValue value) => Contains(new ItemPair<TKey, TValue>(key, value));

        /// <summary>Removes an itempair from the set.</summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(TKey key, TValue value) => Remove(new ItemPair<TKey, TValue>(key, value));

        /// <summary>Reverses the index of the set.</summary>
        public void Reverse() => list.Reverse();
    }
}
