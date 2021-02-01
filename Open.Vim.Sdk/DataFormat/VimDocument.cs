using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Vim.BFast;
using Vim.G3d;

namespace Vim.DataFormat
{
    /* TODO: finish this.
     * 
    /// <summary>
    /// This is the official public version of VIM
    /// </summary>
    public class VimDocument
    {
        public static Version CurrentVersion = new Version(1, 0, 0);
        public const string VimFourCC = "VIM1";

        public static class BufferNames
        {
            public const string entities = nameof(entities);
            public const string assets = nameof(assets);
            public const string geometry = nameof(geometry);
            public const string header = nameof(header);
            public const string strings = nameof(strings);
        }

        public class VimDocumentHeader
        {
            public const string DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";

            public readonly Guid Id;
            public readonly Guid Revision;
            public readonly Version Vim;
            public readonly DateTime Created;
            public readonly string Generator = "";

            public VimDocumentHeader(Guid? id = null)
            {
                Id = id ?? Guid.NewGuid();
                Revision = Guid.NewGuid();
                Vim = CurrentVersion;
                Created = DateTime.Now;
                Generator = Assembly.GetEntryAssembly().FullName;
            }

            public IReadOnlyDictionary<string, string> ToDictionary()
                => new Dictionary<string, string>
                {
                    { nameof(Id).ToLowerInvariant(), Id.ToString() },
                    { nameof(Revision).ToLowerInvariant(), Revision.ToString() },
                    { nameof(Vim).ToLowerInvariant(), $"{Vim.Major}.{Vim.Minor}.{Vim.Build}" },
                    { nameof(Created).ToLowerInvariant(), Created.ToString(DateTimeFormat) },
                    { nameof(Generator).ToLowerInvariant(), Generator },
                };

            public override string ToString()
                => VimFourCC + string.Join("\n", ToDictionary().Select(kv => $"{kv.Key}={kv.Value}"));

            public VimDocumentHeader(IReadOnlyDictionary<string, string> d, List<ValidationResult> validationResults)
            {
                if (d == null)
                {
                    validationResults.Add(ValidationResult.ErrorMissingHeader);
                    return;
                }

                // Get the VIM version 
                if (d.TryGetValue(nameof(Vim).ToLowerInvariant(), out var vim))
                    if (!Version.TryParse(vim, out Vim))
                        validationResults.Add(ValidationResult.ErrorInvalidVimVersion);

                // Get the ID
                if (d.TryGetValue(nameof(Id).ToLowerInvariant(), out var id))
                    if (!Guid.TryParse(id, out Id))
                        validationResults.Add(ValidationResult.WarningInvalidIdGuid);

                // Get the revision GUID
                if (d.TryGetValue(nameof(Revision).ToLowerInvariant(), out var revision))
                    if (!Guid.TryParse(revision, out Revision))
                        validationResults.Add(ValidationResult.WarningInvalidRevisionGuid);

                // Get the created date
                if (d.TryGetValue(nameof(Created).ToLowerInvariant(), out var created))
                    if (!DateTime.TryParseExact(created, DateTimeFormat, null, DateTimeStyles.RoundtripKind, out Created))
                        validationResults.Add(ValidationResult.WarningInvalidCreatedDate);

                // Get the generator 
                d.TryGetValue(nameof(Generator).ToLowerInvariant(), out var generator);
            }
        }

        public VimDocumentHeader Header { get; }
        public G3D Geometry { get; }
        public string[] Strings { get; }
        public EntityTable[] Entities { get; }
        public IReadOnlyDictionary<string, IBuffer> Assets { get; }
        public string FileName { get; }

        public enum ValidationResult
        {
            WarningLargerThan2GB,
            WarningVertexBufferLargerThan2GB,
            WarningInvalidRevisionGuid,
            WarningInvalidIdGuid,
            WarningInvalidCreatedDate,
            WarningHeaderKeysNotLowercase,

            WarningMissingGroupMaterialAttribute,
            WarningMissingVertexUvAttribute,
            WarningMissingVertexNormalAttribute,
            WarningMissingNodeParentAttribute,
            WarningMissingMaterialColorAttribute,
            WarningMissingMaterialNameAttribute,
            WarningMissingMaterialTextureAttribute,
       
            WarningExtraBuffers,
            WarningExtraAttributes,

            ErrorInvalidBFast,
            ErrorInvalidEndianess,
            ErrorMissingHeader,
            ErrorMissingFourCharacterCode,
            ErrorInvalidHeader,
            ErrorDuplicateKeysInHeader,
            ErrorInvalidVimVersion,
            ErrorInvalidGeometry,
            ErrorMissingVimVersion,
            ErrorVertexOffsetsNotIncreasing,
            ErrorIndexOffsetsNotIncreasing,
            ErrorInvalidMaterialIndex,
            ErrorInvalidVertexIndex,
            ErrorStringsNotOrdered,

            ErrorMissingVertexPositionAttribute,
            ErrorMissingCornerIndexAttribute,
            ErrorMissingGroupIndexOffsetAttribute,
            ErrorMissingGroupVertexOffsetAttribute,
            ErrorMissingInstanceGroupAttribute,
            ErrorMissingInstanceTransformAttribute,

            ErrorInvalidVertexPositionAttribute,
            ErrorInvalidCornerIndexAttribute,
            ErrorInvalidGroupIndexOffsetAttribute,
            ErrorInvalidGroupVertexOffsetAttribute,
            ErrorInvalidInstanceGroupAttribute,
            ErrorInvalidInstanceTransformAttribute,

            ErrorInvalidGroupMaterialAttribute,
            ErrorInvalidVertexUvAttribute,
            ErrorInvalidVertexNormalAttribute,
            ErrorInvalidNodeParentAttribute,
            ErrorInvalidMaterialColorAttribute,
            ErrorInvalidMaterialNameAttribute,
            ErrorInvalidMaterialTextureAttribute,
        }

        public enum VersionQueryResult
        {
            NewerMajor,
            NewerMinor,
            NewerPatch,
            OlderMajor,
            OlderMinor,
            OlderPatch,
        }

        public static VimDocument Load(string filePath, List<ValidationResult> validationResults)
        {
            using (var stream = File.OpenRead(filePath))
            {
                BFastReader reader; 
                try
                {
                    var binaryReader = new BinaryReader(stream);
                    reader = BFastReader.Create(binaryReader);
                }
                // reader 
                catch
                {
                    validationResults.Add(ValidationResult.ErrorInvalidBFast);
                    return null;
                }
                return Load(reader, validationResults);
            }
        }

        public static VimDocument Load(BFastReader reader, List<ValidationResult> validationResults)
        {
            var headerBuffer = reader.ReadBufferIfPresent(BufferNames.header);
            var geometryBuffer = reader.ReadBufferIfPresent(BufferNames.geometry);
            var assetsBuffer = reader.ReadBufferIfPresent(BufferNames.assets);
            var stringsBuffer = reader.ReadBufferIfPresent(BufferNames.strings);
            var entitiesBuffer = reader.ReadBufferIfPresent(BufferNames.entities);

            var header = ParseHeader(headerBuffer, validationResults);
            return new VimDocument(header, null, null, null, null, validationResults);
        }

        public static IReadOnlyDictionary<string, string> ParseHeader(IBuffer buffer, List<ValidationResult> validationResults)
            => ParseHeader(buffer.GetUtf8String(), validationResults);

        public static IReadOnlyDictionary<string, string> ParseHeader(string text, List<ValidationResult> validationResults)
        {
            var r = new Dictionary<string, string>();

            if (!text.StartsWith(VimFourCC))
            {
                validationResults.Add(ValidationResult.ErrorMissingFourCharacterCode);
                return r;
            }

            if (VimFourCC.Length != 4)
                throw new Exception("Internal error: four character code is not four characters");
            text = text.Substring(4);

            var lines = text.Split('\n').Select(x => x.Trim());
            foreach (var line in lines)
            {
                if (line.Length == 0) continue;
                if (line.StartsWith(";")) continue;
                var sepIndex = line.IndexOf('=');
                if (sepIndex < 0)
                {
                    validationResults.Add(ValidationResult.ErrorInvalidHeader);
                    continue;
                }
                var key = line.Substring(0, sepIndex).Trim();
                if (key.ToLowerInvariant() != key)
                {
                    validationResults.Add(ValidationResult.WarningHeaderKeysNotLowercase);
                    key = key.ToLowerInvariant();
                }
                var val = line.Substring(sepIndex + 1).Trim();
                if (r.ContainsKey(key))
                    validationResults.Add(ValidationResult.ErrorDuplicateKeysInHeader);
                else
                    r.Add(key, val);
            }
            return r;
        }

        public static bool IsWarning(ValidationResult r)
            => Enum.GetName(typeof(ValidationResult), r).StartsWith("Warning");

        public static bool IsError(ValidationResult r)
            => Enum.GetName(typeof(ValidationResult), r).StartsWith("Error");

        public VimDocument(
            IReadOnlyDictionary<string, string> header,
            G3D geometry,
            string[] strings,
            EntityTable[] entities,
            IReadOnlyDictionary<string, IBuffer> assets,
            List<ValidationResult> validationResults,
            bool fastValidate = false)
        {
            Header = new VimDocumentHeader(header, validationResults);

#if DEBUG
            {
                // During DEBUG builds we should check that we can round-trip the header
                var tmpResults = new List<ValidationResult>();

                // Convert the header to a dictionary
                var tmpDict = Header.ToDictionary();

                // Try to create a new document header from that dictionary (should work fine)
                var tmpHeader = new VimDocumentHeader(tmpDict, tmpResults);
                Debug.Assert(tmpResults.Count == 0);

                // Convert the header to text
                var tmpHeaderText = Header.ToString();

                // See if the text representation can also be parsed without incident
                var tmpDict2 = ParseHeader(tmpHeaderText, tmpResults);
                Debug.Assert(tmpResults.Count == 0);
                Debug.Assert(tmpDict.Count == tmpDict2.Count);
                foreach (var kv in tmpDict)
                {
                    Debug.Assert(tmpDict2.ContainsKey(kv.Key));
                    Debug.Assert(tmpDict2[kv.Key] == kv.Value);
                }

                // And just check that we can create another header again
                var tmpHeader2 = new VimDocumentHeader(tmpDict2, tmpResults);
                Debug.Assert(tmpResults.Count == 0);
            }
#endif

            Geometry = geometry ?? G3D.Create();
            Strings = strings ?? new string[0];
            Assets = assets ?? new Dictionary<string, IBuffer>();
            Entities = entities ?? new EntityTable[0];

            // Check that strings are ordered, and that no string is the same as previous.
            // This could be long, so it is skipped when fastValidate is turned on.
            if (!fastValidate)
                if (!Strings.AsParallel().Select((x, i) => i == 0 || x.CompareTo(Strings[i - 1]) > 0).All(x => x))
                    validationResults.Add(ValidationResult.ErrorStringsNotOrdered);
        }        
    }

    // TODO: this will go to Serializer when it is it time.
    public static class Serializer 
    {

        public static VimDocument Load(string filePath, List<ValidationResult> validationResults)
        {
            using (var stream = File.OpenRead(filePath))
            {
                BFastReader reader; 
                try
                {
                    var binaryReader = new BinaryReader(stream);
                    reader = BFastReader.Create(binaryReader);
                }
                // reader 
                catch
                {
                    validationResults.Add(ValidationResult.ErrorInvalidBFast);
                    return null;
                }
                return Load(reader, validationResults);
            }
        }

        public static VimDocument Load(BFastReader reader, List<ValidationResult> validationResults)
        {
            var headerBuffer = reader.ReadBufferIfPresent(BufferNames.header);
            var geometryBuffer = reader.ReadBufferIfPresent(BufferNames.geometry);
            var assetsBuffer = reader.ReadBufferIfPresent(BufferNames.assets);
            var stringsBuffer = reader.ReadBufferIfPresent(BufferNames.strings);
            var entitiesBuffer = reader.ReadBufferIfPresent(BufferNames.entities);

            var header = ParseHeader(headerBuffer, validationResults);
            return new VimDocument(header, null, null, null, null, validationResults);
        }    }
    */
}
