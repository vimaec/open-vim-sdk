namespace Vim.DotNetUtilities.Logging
{
    public interface ILogger
    {
        ILogger Log(string message = "", LogLevel level = LogLevel.Trace);
    }
}
