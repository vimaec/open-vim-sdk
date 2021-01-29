using System.Linq;
using System.Text;

namespace Vim.DotNetUtilities
{
    public class CodeBuilder
    {
        private int indentCount;
        private StringBuilder sb = new StringBuilder();

        public CodeBuilder AppendLine(string line = "")
        {
            var openBraces = line.Count(c => c == '{');
            var closeBraces = line.Count(c => c == '}');
            
            // Sometimes we have {} on the same line
            if (openBraces == closeBraces)
            {
                openBraces = 0;
                closeBraces = 0;
            }

            indentCount -= closeBraces;
            sb.Append(new string(' ', indentCount * 4));
            sb.AppendLine(line);
            indentCount += openBraces;
            return this;
        }

        public override string ToString()
            => sb.ToString();
    }
}
