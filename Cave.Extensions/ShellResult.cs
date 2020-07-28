﻿using System;
using System.Collections.Generic;

namespace Cave
{
#pragma warning disable CA2227
    /// <summary>A process result.</summary>
    public class ShellResult
    {
        /// <summary>Gets or sets all output lines.</summary>
        public IList<string> StdOut { get; protected internal set; }

        /// <summary>Gets or sets all error lines.</summary>
        public IList<string> StdErr { get; protected internal set; }

        /// <summary>Gets or sets the exit code of the process.</summary>
        public int ExitCode { get; protected internal set; }

        /// <summary>Gets or sets the exception catched on process start.</summary>
        public Exception Exception { get; protected internal set; }
    }
#pragma warning restore CA2227
}
