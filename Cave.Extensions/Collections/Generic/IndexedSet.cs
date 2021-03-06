using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cave.Collections.Generic
{
    /// <summary>Gets a generic typed set of objects.</summary>
    /// <typeparam name="T">Value type for the set.</typeparam>
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    [SuppressMessage("Design", "CA1000")]
    [SuppressMessage("Naming", "CA1710")]
    [SuppressMessage("Naming", "CA1716")]
    public sealed class IndexedSet<T> : IList<T>, IEquatable<IndexedSet<T>>
    {
        #region IEquatable<IndexedSet<T>> Members

        /// <summary>Checks another Set{T} instance for equality.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IndexedSet<T> other) => (other != null) && (other.Count == Count) && ContainsRange(other);

        #endregion

        #region IList<T> Members

        #region ICollection<T> Member

        /// <summary>Gets a value indicating whether the set is readonly or not.</summary>
        public bool IsReadOnly => false;

        #endregion

        #region IEnumerable Member

        /// <summary>Gets an <see cref="IEnumerator" /> for this set.</summary>
        public IEnumerator GetEnumerator() => Items.GetEnumerator();

        #endregion

        #region IEnumerable<T> Member

        /// <summary>Gets an <see cref="IEnumerator" /> for this set.</summary>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Items.GetEnumerator();

        #endregion

        #endregion

        #region Overrides

        /// <summary>Checks another Set{T} instance for equality.</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as IndexedSet<T>;
            return (other != null) && Equals(other);
        }

        /// <summary>Gets the hash code of the base list.</summary>
        /// <returns></returns>
        public override int GetHashCode() => Items.GetHashCode();

        #endregion

        #region Members

        #region ICloneable Member

        /// <summary>Creates a copy of this set.</summary>
        public object Clone() => new IndexedSet<T>(Items);

        #endregion

        /// <summary>Gets an array of all elements present.</summary>
        /// <returns></returns>
        public T[] ToArray() => Items.ToArray();

        #endregion

        #region operators

        /// <summary>Gets a <see cref="Set{T}" /> containing all objects part of one of the specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static IndexedSet<T> operator |(IndexedSet<T> set1, IndexedSet<T> set2) => BitwiseOr(set1, set2);

        /// <summary>Gets a <see cref="Set{T}" /> containing only objects part of both of the specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static IndexedSet<T> operator &(IndexedSet<T> set1, IndexedSet<T> set2) => BitwiseAnd(set1, set2);

        /// <summary>Gets a <see cref="Set{T}" /> containing all objects part of the first set after removing all objects present at the second set.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static IndexedSet<T> operator -(IndexedSet<T> set1, IndexedSet<T> set2) => Subtract(set1, set2);

        /// <summary>Builds a new <see cref="Set{T}" /> containing only the items found exclusively in one of both specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static IndexedSet<T> operator ^(IndexedSet<T> set1, IndexedSet<T> set2) => Xor(set1, set2);

        /// <summary>Checks two sets for equality.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>true if the sets equal each other.</returns>
        public static bool operator ==(IndexedSet<T> set1, IndexedSet<T> set2) => set1 is null ? set2 is null : !(set2 is null) && set1.Equals(set2);

        /// <summary>Checks two sets for inequality.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>false if the sets equal each other.</returns>
        public static bool operator !=(IndexedSet<T> set1, IndexedSet<T> set2) => !(set1 == set2);

        #endregion

        #region static Member

        /// <summary>Builds the union of two specified <see cref="Set{T}" />s.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static IndexedSet<T> BitwiseOr(IndexedSet<T> set1, IndexedSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }

            if (set1.Count < set2.Count)
            {
                return BitwiseOr(set2, set1);
            }

            var result = new IndexedSet<T>();
            result.AddRange(set2);
            foreach (T item in set1)
            {
                if (set2.Contains(item))
                {
                    continue;
                }

                result.Add(item);
            }

            return result;
        }

        /// <summary>Builds the intersection of two specified <see cref="Set{T}" />s.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static IndexedSet<T> BitwiseAnd(IndexedSet<T> set1, IndexedSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }

            if (set1.Count < set2.Count)
            {
                return BitwiseAnd(set2, set1);
            }

            var result = new IndexedSet<T>();
            foreach (T itemsItem in set1)
            {
                if (set2.Contains(itemsItem))
                {
                    result.Add(itemsItem);
                }
            }

            return result;
        }

        /// <summary>Subtracts the specified <see cref="Set{T}" /> from this one and returns a new <see cref="Set{T}" /> containing the result.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static IndexedSet<T> Subtract(IndexedSet<T> set1, IndexedSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }

            var result = new IndexedSet<T>();
            foreach (T setItem in set1)
            {
                if (!set2.Contains(setItem))
                {
                    result.Add(setItem);
                }
            }

            return result;
        }

        /// <summary>Builds a new <see cref="Set{T}" /> containing only items found exclusivly in one of both specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static IndexedSet<T> Xor(IndexedSet<T> set1, IndexedSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }

            if (set1.Count < set2.Count)
            {
                return Xor(set2, set1);
            }

            var newSet2 = new LinkedList<T>(set2);
            var result = new IndexedSet<T>();
            foreach (T setItem in set1)
            {
                if (!set2.Contains(setItem))
                {
                    result.Add(setItem);
                }
                else
                {
                    newSet2.Remove(setItem);
                }
            }

            result.AddRange(newSet2);
            return result;
        }

        #endregion

        #region private Member

        readonly List<T> Items;
        readonly Dictionary<T, int> Lookup;

        #endregion

        #region constructors

        /// <summary>Initializes a new instance of the <see cref="IndexedSet{T}" /> class.</summary>
        public IndexedSet()
        {
            Items = new List<T>();
            Lookup = new Dictionary<T, int>();
        }

        /// <summary>Initializes a new instance of the <see cref="IndexedSet{T}" /> class.</summary>
        public IndexedSet(int capacity)
        {
            Items = new List<T>(capacity);
            Lookup = new Dictionary<T, int>(capacity);
        }

        /// <summary>Initializes a new instance of the <see cref="IndexedSet{T}" /> class.</summary>
        public IndexedSet(T item)
            : this(256) =>
            Add(item);

        /// <summary>Initializes a new instance of the <see cref="IndexedSet{T}" /> class.</summary>
        public IndexedSet(IEnumerable<T> items)
            : this() =>
            AddRange(items);

        #endregion

        #region public Member

        /// <summary>Builds the union of the specified and this <see cref="Set{T}" /> and returns a new set with the result.</summary>
        /// <param name="items">Provides the other <see cref="Set{T}" /> used.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public IndexedSet<T> Union(IndexedSet<T> items) => BitwiseOr(this, items);

        /// <summary>Builds the intersection of the specified and this <see cref="Set{T}" /> and returns a new set with the result.</summary>
        /// <param name="items">Provides the other <see cref="Set{T}" /> used.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public IndexedSet<T> Intersect(IndexedSet<T> items) => BitwiseAnd(this, items);

        /// <summary>Subtracts a specified <see cref="Set{T}" /> from this one and returns a new set with the result.</summary>
        /// <param name="items">Provides the other <see cref="Set{T}" /> used.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public IndexedSet<T> Subtract(IndexedSet<T> items) => Subtract(this, items);

        /// <summary>Builds a new <see cref="Set{T}" /> containing only items found exclusivly in one of both specified sets.</summary>
        /// <param name="items">Provides the other <see cref="Set{T}" /> used.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public IndexedSet<T> ExclusiveOr(IndexedSet<T> items) => Xor(this, items);

        /// <summary>Checks whether a specified object is part of the set.</summary>
        public bool Contains(T item) => Lookup.ContainsKey(item);

        /// <summary>Checks whether all specified objects are part of the set.</summary>
        public bool ContainsRange(ICollection<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var allFound = true;
            foreach (var obj in collection)
            {
                allFound &= Contains(obj);
            }

            return allFound;
        }

        /// <summary>Gets a value indicating whether the list is empty or not.</summary>
        public bool IsEmpty => Items.Count == 0;

        /// <summary>Adds a specified object to the set.</summary>
        /// <param name="item">The item to be added to the set.</param>
        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var index = Items.Count;
            try
            {
                Items.Add(item);
                Lookup.Add(item, index);
            }
            catch
            {
                RebuildIndex();
                throw;
            }
        }

        /// <summary>Adds a range of objects to the set.</summary>
        /// <param name="items">The objects to be added to the list.</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var obj in items)
            {
                Add(obj);
            }
        }

        /// <summary>Includes an object that is not already present in the set (others are ignored).</summary>
        /// <param name="item">The object to be included.</param>
        public void Include(T item)
        {
            if (!Contains(item))
            {
                Add(item);
            }
        }

        /// <summary>Includes objects that are not already present in the set (others are ignored).</summary>
        /// <param name="items">The objects to be included.</param>
        public void IncludeRange(ICollection<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var obj in items)
            {
                Include(obj);
            }
        }

        void RebuildIndex()
        {
            Lookup.Clear();
            for (var i = 0; i < Items.Count; i++)
            {
                Lookup.Add(Items[i], i);
            }
        }

        /// <summary>Removes an object from the set.</summary>
        /// <param name="item">The object to be removed.</param>
        public bool Remove(T item)
        {
            RemoveAt(Lookup[item]);
            return true;
        }

        /// <summary>Removes objects from the set.</summary>
        public void RemoveRange(ICollection<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var obj in collection)
            {
                Remove(obj);
            }
        }

        /// <summary>Clears the set.</summary>
        public void Clear()
        {
            Lookup.Clear();
            Items.Clear();
        }

        #endregion

        #region IList<T> member

        /// <summary>Returns the zero-based index of the first occurrence of a value in the set.</summary>
        /// <param name="item">The object to locate in the set.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire set, if found; otherwise, –1.</returns>
        public int IndexOf(T item) => Lookup[item];

        /// <summary>Inserts an element into the set at the specified index.</summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        public void Insert(int index, T item)
        {
            if ((index < 0) || (index > Items.Count))
            {
                throw new IndexOutOfRangeException();
            }

            Lookup.Add(item, index);
            Items.Insert(index, item);
            for (var i = index + 1; i < Items.Count; i++)
            {
                Lookup[Items[i]] = i - 1;
            }
        }

        /// <summary>Removes the element at the specified index of the set.</summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            if ((index < 0) || (index > Items.Count))
            {
                throw new IndexOutOfRangeException();
            }

            try
            {
                for (var i = index + 1; i < Items.Count; i++)
                {
                    Lookup[Items[i]] = i - 1;
                }

                if (!Lookup.Remove(Items[index]))
                {
                    throw new IndexOutOfRangeException();
                }

                Items.RemoveAt(index);
            }
            catch
            {
                RebuildIndex();
                throw;
            }
        }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get => Items[index];
            set
            {
                try
                {
                    var oldKey = Items[index];
                    if (!Lookup.Remove(oldKey))
                    {
                        throw new KeyNotFoundException();
                    }

                    Lookup.Add(value, index);
                    Items[index] = value;
                }
                catch
                {
                    RebuildIndex();
                    throw;
                }
            }
        }

        #endregion

        #region ICollection Member

        /// <summary>Copies all objects present at the set to the specified array, starting at a specified index.</summary>
        /// <param name="array">one-dimensional array to copy to.</param>
        /// <param name="arrayIndex">the zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        /// <summary>Gets the number of objects present at the set.</summary>
        public int Count => Items.Count;

        #endregion
    }
}
