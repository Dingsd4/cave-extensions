﻿using System;
using System.Collections.Generic;

namespace Cave
{
    /// <summary>
    /// Provides extensions for <see cref="IDictionary{TKey, TValue}"/> instances
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// Tries to add an entry to the <see cref="IDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="key">The key to add</param>
        /// <param name="value">The value to add</param>
        /// <returns>Returns true if the entry was added, false otherwise</returns>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// Tries to add an entry to the <see cref="IDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="key">The key to add</param>
        /// <param name="valueFunc">A function to retrieve the value for a specified key</param>
        /// <returns>Returns true if the entry was added, false otherwise</returns>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFunc)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            TValue value = valueFunc != null ? valueFunc(key) : default(TValue);
            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// Tries to add a number of entries to the <see cref="IDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="pairs">The key value pairs to add</param>
        public static void TryAddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            foreach (var pair in pairs)
            {
                TryAdd(dictionary, pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Tries to add a number of entries to the <see cref="IDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="keys">The keys to add</param>
        /// <param name="valueFunc">A function to retrieve the value for a specified key</param>
        public static void TryAddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys, Func<TKey, TValue> valueFunc)
        {
            foreach (var key in keys)
            {
                TryAdd(dictionary, key, valueFunc);
            }
        }

        /// <summary>
        /// Tries to retrieve the value for the specified key
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">Dictionary instance to add key value pair to</param>
        /// <param name="key">The key to get</param>
        /// <returns>Returns the value found or default(value)</returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out TValue value);
            return value;
        }
    }
}
