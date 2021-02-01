using System;
using System.Collections.Generic;
using System.Text;

namespace Vim.DotNetUtilities
{
    // From: https://madskristensen.net/blog/A-shorter-and-URL-friendly-GUID

    /// <summary>
    /// A class which shortens GUID string into a 22 character long string.
    /// </summary>
    public static class GuidEncoder
    {
        public static string Encode(string guidText)
            => Encode(new Guid(guidText));

        public static string Encode(Guid guid)
            => Convert.ToBase64String(guid.ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                .Substring(0, 22);

        public static Guid Decode(string encoded)
            => new Guid(
                Convert.FromBase64String(
                    encoded
                    .Replace("_", "/")
                    .Replace("-", "+") + "=="));
        
        /// <summary>
        /// Shortens the given GUID into a 22 character long string.
        /// </summary>
        public static string ToShortString(this Guid guid)
            => Encode(guid);
    }
}
