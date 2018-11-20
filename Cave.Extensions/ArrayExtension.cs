#region CopyRight 2018
/*
    Copyright (c) 2007-2018 Andreas Rohleder (andreas@rohleder.cc)
    All rights reserved
*/
#endregion
#region License LGPL-3
/*
    This program/library/sourcecode is free software; you can redistribute it
    and/or modify it under the terms of the GNU Lesser General Public License
    version 3 as published by the Free Software Foundation subsequent called
    the License.

    You may not use this program/library/sourcecode except in compliance
    with the License. The License is included in the LICENSE file
    found at the installation directory or the distribution package.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be included
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Provides extensions to byte[], array and IEnumerable instances
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// Retrieves a number of elements from the array as new array instance
        /// </summary>
        /// <param name="data">Source array</param>
        /// <param name="index">Element index</param>
        /// <param name="count">Number of elements to copy</param>
        /// <returns>Returns a new array instance</returns>
        public static T[] GetRange<T>(this T[] data, int index, int count)
        {
            T[] result = new T[count];
            Array.Copy(data, index, result, 0, count);
            return result;
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance
        /// </summary>
        /// <param name="data">Source array</param>
        /// <param name="index">Element index</param>
        /// <returns>Returns a new array instance</returns>
        public static T[] GetRange<T>(this T[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            T[] result = new T[data.Length - index];
            Array.Copy(data, index, result, 0, result.Length);
            return result;
        }

        /// <summary>Shuffles items with the specified seed. The same seed will always result in the same order.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items to shuffle.</param>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this IEnumerable<T> items, int seed = 0)
        {
            unchecked
            {
                if (seed == 0)
                {
                    seed = (int)DateTime.UtcNow.Ticks;
                }

                List<T> result = items.ToList();
                int count = result.Count;
                for (int i = 0; i < count; i++)
                {
                    int n = Math.Abs((i ^ seed).GetHashCode()) % count;
                    T t = result[i];
                    result[i] = result[n];
                    result[n] = t;
                }
                return result;
            }
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance
        /// </summary>
        /// <param name="data">Source array</param>
        /// <param name="index">Element index</param>
        /// <param name="count">Number of elements to copy</param>
        /// <returns>Returns a new array instance</returns>
        public static T[] GetRange<T>(this IList<T> data, int index, int count)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = data[index++];
            }

            return result;
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance
        /// </summary>
        /// <param name="data">Source array</param>
        /// <param name="index">Element index</param>
        /// <returns>Returns a new array instance</returns>
        public static T[] GetRange<T>(this IList<T> data, int index)
        {
            return GetRange(data, index, data.Count - index);
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance
        /// </summary>
        /// <param name="data">Source array</param>
        /// <param name="index">Element index</param>
        /// <param name="count">Number of elements to copy</param>
        /// <returns>Returns a new array instance</returns>
        public static IEnumerable<T> SubRange<T>(this IEnumerable<T> data, int index, int count)
        {
            return data.Where((v, i) => i >= index && i < index + count);
        }

        /// <summary>
        /// Retrieves a number of elements from the array as new array instance
        /// </summary>
        /// <param name="data">Source array</param>
        /// <param name="index">Element index</param>
        /// <returns>Returns a new array instance</returns>
        public static IEnumerable<T> SubRange<T>(this IEnumerable<T> data, int index)
        {
            return data.Where((v, i) => i >= index);
        }

        /// <summary>Concatenates elements</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns></returns>
        public static T[] Concat<T>(this T[] t1, params T[] t2)
        {
            T[] result = new T[t1.Length + t2.Length];
            Array.Copy(t1, 0, result, 0, t1.Length);
            Array.Copy(t2, 0, result, t1.Length, t2.Length);
            return result;
        }

        /// <summary>Concatenates elements</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns></returns>
        public static T[] Concat<T>(this T[] t1, params T[][] t2)
        {
            int count = t1.Length;
            for (int i = 0; i < t2.Length; i++)
            {
                count += t2[i].Length;
            }
            T[] result = new T[count];
            Array.Copy(t1, 0, result, 0, t1.Length);
            int start = t1.Length;
            for (int i = 0; i < t2.Length; i++)
            {
                Array.Copy(t2[i], 0, result, start, t2[i].Length);
                start += t2[i].Length;
            }
            return result;
        }

        /// <summary>Checks whether a range of bytes matches the comparand</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="comparand">The comparand.</param>
        /// <returns></returns>
        public static bool RangeEquals(this byte[] bytes, int offset, int count, byte[] comparand)
        {
            if (offset < 0 || count < 0 || bytes.Length < offset + count)
            {
                return false;
            }

            for (int i = 0, j = offset; i < count; i++, j++)
            {
                if (bytes[j] != comparand[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Checks whether data starts with the specified pattern or not</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="encoding">The encoding (defaults to <see cref="Encoding.UTF8"/>).</param>
        /// <returns></returns>
        public static bool StartsWith(this byte[] data, string pattern, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] bytes = encoding.GetBytes(pattern);
            return StartsWith(data, bytes);
        }

        /// <summary>Checks whether data starts with the specified pattern or not</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public static bool StartsWith(this byte[] data, byte[] pattern)
        {
            if (pattern.Length > data.Length)
            {
                return false;
            }

            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] != data[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Finds the startindex of the first occurence of the specified pattern.</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public static int IndexOf(this byte[] data, byte[] pattern)
        {
            int matchIndex = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == pattern[matchIndex++])
                {
                    // last pattern byte reached ?
                    if (matchIndex + 1 == pattern.Length)
                    {
                        // yes
                        return i - matchIndex;
                    }
                }
                else
                {
                    // no match, reset
                    matchIndex = 0;
                }
            }
            return -1;
        }

        /// <summary>Replaces the specified pattern.</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacer">The replacer.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Pattern not found.</exception>
        public static byte[] ReplaceFirst(this byte[] data, byte[] pattern, byte[] replacer)
        {
            int i = data.IndexOf(pattern);
            if (i < 0)
            {
                return data;
            }

            byte[] result = new byte[data.Length - pattern.Length + replacer.Length];
            Buffer.BlockCopy(data, 0, result, 0, i);
            Buffer.BlockCopy(replacer, 0, result, i, replacer.Length);
            int offs = i + pattern.Length + 1;
            Buffer.BlockCopy(data, offs, result, i + replacer.Length, data.Length - offs);
            return result;
        }

        /// <summary>Replaces the specified pattern.</summary>
        /// <param name="data">The data.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacers">The replacers.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Pattern not found.</exception>
        public static byte[] ReplaceFirst(this byte[] data, byte[] pattern, params byte[][] replacers)
        {
            int i = data.IndexOf(pattern);
            if (i < 0)
            {
                return data;
            }

            int replacersLength = replacers.Select(r => r.Length).Sum();
            byte[] result = new byte[data.Length - pattern.Length + replacersLength];
            Buffer.BlockCopy(data, 0, result, 0, i);
            {
                int offs = i;
                foreach (byte[] replacer in replacers)
                {
                    Buffer.BlockCopy(replacer, 0, result, offs, replacer.Length);
                    offs += replacer.Length;
                }
            }
            {
                int offs = i + pattern.Length + 1;
                Buffer.BlockCopy(data, offs, result, i + replacersLength, data.Length - offs);
            }
            return result;
        }

        /// <summary>
        /// Performs an <see cref="Array.IndexOf{T}(T[], T)"/> call and returns the result.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array to search.</param>
        /// <param name="value">The object to locate in array.</param>
        /// <returns>The zero-based index of the first occurrence of value in the entire array, if found; otherwise, �1.</returns>
        public static int IndexOf<T>(this T[] array, T value) => Array.IndexOf(array, value);
    }
}
