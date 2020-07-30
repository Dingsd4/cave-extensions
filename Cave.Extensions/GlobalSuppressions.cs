﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1051", Justification = "Public fields needed")]
[assembly: SuppressMessage("Globalization", "CA1303", Justification = "Messages will not be globalized")]
[assembly: SuppressMessage("Globalization", "CA1308", Justification = "Lowercase required.")]
[assembly: SuppressMessage("Reliability", "CA2002", Justification = "Locking on this is required for synchronized types")]
[assembly: SuppressMessage("Reliability", "CA2008", Justification = "Parallel code requires Task.Factory.StartNew()")]
[assembly: SuppressMessage("Performance", "CA1825", Justification = "Array.Empty<> is not available at most frameworks")]
[assembly: SuppressMessage("Usage", "CA2225", Justification = "No named operator overloading")]
[assembly: SuppressMessage("Naming", "CA1716", Justification = "Required for usage compatibility to framework")]

