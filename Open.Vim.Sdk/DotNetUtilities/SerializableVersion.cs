using System;

namespace Vim.DotNetUtilities
{
    // We've defined a serializable version since the System.Version type is not readily serializable by Newtonsoft.Json
    [Serializable]
    public class SerializableVersion
    {
        public int Major;
        public int Minor;
        public int Patch;

        public override string ToString()
            => string.Join(".", Major, Minor, Patch);

        public static SerializableVersion Parse(string input)
        {
            var result = new SerializableVersion();

            if (string.IsNullOrEmpty(input))
            {
                return result;
            }

            var tokens = input.Split('.');
            if (tokens.Length > 0) { int.TryParse(tokens[0], out result.Major); }
            if (tokens.Length > 1) { int.TryParse(tokens[1], out result.Minor); }
            if (tokens.Length > 2) { int.TryParse(tokens[2], out result.Patch); }

            return result;
        }
    }

    public static class SerializableVersionExtensions
    {
        public static System.Version ToVersion(this SerializableVersion version)
            => new System.Version(version.Major, version.Minor, version.Patch);

        public static SerializableVersion ToSerializableVersion(this System.Version version)
            => new SerializableVersion { Major = version.Major, Minor = version.Minor, Patch = version.Build };
    }
}
