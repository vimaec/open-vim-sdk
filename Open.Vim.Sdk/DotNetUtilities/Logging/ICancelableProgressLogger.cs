﻿using System;

namespace Vim.DotNetUtilities.Logging
{
    /// <summary>
    /// For convenience this combines the chores of allowing long taks to be cancelled, to report progress,
    /// and to log status. See the Helper class for a function that will generate a default logger.
    /// </summary>
    public interface ICancelableProgressLogger : ICancelable, ILogger, IProgress<double>
    {
    }
}
