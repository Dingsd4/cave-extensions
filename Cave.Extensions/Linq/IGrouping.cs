﻿#if NET20
#pragma warning disable CS1591 // we will not document back ports

using System.Collections.Generic;

namespace System.Linq
{

    public interface IGrouping<TKey, TElement> : IEnumerable<TElement>

    {
        TKey Key { get; }
    }
}

#pragma warning restore CS1591
#endif

