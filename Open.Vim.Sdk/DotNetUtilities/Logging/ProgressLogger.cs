using System;

namespace Vim.DotNetUtilities.Logging
{
    public class ProgressLogger : ICancelableProgressLogger
    {
        public Action<double> ReportingAction { get; }
        public Action<string> LoggingAction { get; }
        public Action CancelAction { get; }
        public bool CancelRequested { get; set; }

        public ProgressLogger(Action<string> loggingAction, Action<double> reportingAction, Action cancelAction)
            => (LoggingAction, ReportingAction, CancelAction) = (loggingAction, reportingAction, cancelAction);

        public void Cancel()
        {
            if (CancelRequested)
                return;
            CancelRequested = true;
            CancelAction?.Invoke();
        }

        public bool IsCancelRequested()
            => CancelRequested;

        public ILogger Log(string message = "", LogLevel level = LogLevel.Trace)
        {
            LoggingAction?.Invoke(message);
            return this;
        }

        public void Report(double value)
            => ReportingAction?.Invoke(value);
    }
}
