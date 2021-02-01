using System;
using System.Collections.Generic;
using System.Text;

namespace Vim.DotNetUtilities.Logging
{
    public class IndentLogger: ILogger
    {
        private const string _indentation = "  ";

        public readonly ILogger InnerLogger;
        private string _indentPrefix = string.Empty;

        public IndentLogger(ILogger innerLogger)
        {
            InnerLogger = innerLogger;
        }

        public IndentLogger Indent()
        {
            _indentPrefix += _indentation;
            return this;
        }

        public IndentLogger Outdent()
        {
            _indentPrefix = _indentPrefix.Substring(0, Math.Max(_indentPrefix.Length - _indentation.Length, 0));
            return this;
        }

        public ILogger Log(string message = "", LogLevel level = LogLevel.Trace)
        {
            InnerLogger.Log(_indentPrefix + message, level);
            return this;
        }

        public IndentLogger Log(string label, IEnumerable<string> messages, LogLevel level = LogLevel.Trace)
        {
            Log(label, level);
            Log("{");
            Indent();
            this.Log(messages, level);
            Outdent();
            Log("}");
            return this;
        }
    }
}
