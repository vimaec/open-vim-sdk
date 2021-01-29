namespace Vim.DotNetUtilities.Logging
{
    public class NullLogger: ILogger
    {
        public ILogger Log(string message = "", LogLevel level = LogLevel.Trace)
            => this; 
    }
}
