using System;
using System.Collections.Generic;

namespace Vim.DotNetUtilities
{
    public class UniqueNamer
    {
        private readonly Dictionary<string, int> _uniqueNames = new Dictionary<string, int>();
        
        public string GetUniqueName(string name)
        {
            if (!_uniqueNames.ContainsKey(name)) _uniqueNames[name] = 0;
            var number = _uniqueNames[name]++;
            return number == 0 ? name : $"{name}_{number}";
        }
    }
}
