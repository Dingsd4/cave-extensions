﻿using System;
using System.Diagnostics;
using System.Reflection;

namespace Cave
{
    /// <summary>
    /// Provides property information, full path and value access for the <see cref="PropertyEnumerator" /> and
    /// <see cref="PropertyValueEnumerator" /> classes.
    /// </summary>
    [DebuggerDisplay("{FullPath}")]
    public class PropertyData
    {
        #region Constructors

        /// <summary>Creates a new instance of the <see cref="PropertyData" /> class.</summary>
        /// <param name="rootPath">The root path. This may not be null.</param>
        /// <param name="propertyInfo">The property info. This may not be null.</param>
        /// <param name="source">The source object of the property.</param>
        public PropertyData(string rootPath, PropertyInfo propertyInfo, object source = null)
        {
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            Source = source;
            CanGetValue = (Source != null) && (PropertyInfo.GetGetMethod().GetParameters().Length == 0);
        }

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether the <see cref="Value" /> property can be used or not.</summary>
        public bool CanGetValue { get; }

        /// <summary>Gets the full path of the property.</summary>
        public string FullPath => $"{RootPath}/{PropertyInfo.Name}";

        /// <summary>Gets the property information.</summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>Gets the root path of the property.</summary>
        public string RootPath { get; }

        /// <summary>
        /// Gets the source object of this property. This is null at <see cref="PropertyEnumerator" /> and may be null at
        /// <see cref="PropertyValueEnumerator" /> if the property value or root property value is null.
        /// </summary>
        public object Source { get; }

        /// <summary>Gets the current value of the property. This will result in exceptions if <see cref="CanGetValue" /> == false.</summary>
        public object Value => PropertyInfo.GetValue(Source, null);

        #endregion

        #region Members

        /// <summary>Gets the current property value of the specified object. The object has to match the PropertyInfo.DeclaringType.</summary>
        /// <param name="source">Source object to read from.</param>
        /// <returns>Returns the property value.</returns>
        public object GetValueOf(object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.GetType() != PropertyInfo.DeclaringType)
            {
                throw new ArgumentOutOfRangeException(nameof(source));
            }

            return PropertyInfo.GetValue(source, null);
        }

        #endregion
    }
}
