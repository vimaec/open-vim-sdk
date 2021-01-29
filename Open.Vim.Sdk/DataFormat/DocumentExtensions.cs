using System;
using System.Collections.Generic;
using System.Linq;
using Vim.Math3d;
using Vim.LinqArray;
using static Vim.LinqArray.LinqArray;
using Vim.DotNetUtilities;
using Vim.BFast;

namespace Vim.DataFormat
{
    public enum ExpansionMode
    {
        WorldSpaceGeometry,
        LocalGeometry
    }

    public static class DocumentExtensions
    {
        public static ExpansionMode GetExpansionMode(this SerializableHeader header)
            => header.ObjectModelVersion.Major <= 2
                ? ExpansionMode.WorldSpaceGeometry
                : ExpansionMode.LocalGeometry;

        public static ExpansionMode GetExpansionMode(this Document doc)
            => doc.Header.GetExpansionMode();

        public static ExpansionMode GetExpansionMode(this SerializableDocument doc)
            => doc.Header.GetExpansionMode();

        public static Document ToDocument(this SerializableDocument document)
            => new Document(document);

        public static EntityTable ToEntityTable(this SerializableEntityTable entityTable, Document document)
            => new EntityTable(document, entityTable);

        public static IArray<string> GetColumnNames(this EntityTable table)
            => table.Columns.Select(b => b.Name);

        public static IArray<string> GetShortColumnNames(this EntityTable table)
            => table.Columns.Select(c => c.GetShortName());

        public static IArray<string> GetQualifiedColumnNames(this EntityTable table)
            => table.Columns.Select(c => c.GetTableQualifiedName(table));

        public static void ValidateRelations(this Document doc)
        {
            foreach (var et in doc.EntityTables.Values.ToEnumerable())
            {
                foreach (var ic in et.IndexColumns.Values.ToEnumerable())
                {
                    var relatedTable = ic.GetRelatedTable(doc);
                    var maxValue = relatedTable.NumRows;
                    var data = ic.GetTypedData();
                    for (var i = 0; i < data.Length; ++i)
                    {
                        var v = data[i];
                        if (v < -1 || v > maxValue)
                        {
                            throw new Exception($"Invalid relation {v} out of range of -1 to {maxValue}");
                        }
                    }
                }
            }
        }

        public static (string, string) GetSplitIndexName(string name)
        {
            var parts = name.Split(':');
            if (parts.Length != 2)
                throw new Exception($"Index column name {name} does not have two parts");
            return (parts[0], parts[1]);
        }

        public static string GetRelatedTableNameFromColumnName(string name)
            => GetSplitIndexName(name).Item1; 

        public static string GetFieldNameFromColumnName(string name)
            => GetSplitIndexName(name).Item2;

        public static string GetFieldName(this INamedBuffer<int> ic)
            => GetFieldNameFromColumnName(ic.Name);

        public static string GetRelatedTableName(this INamedBuffer<int> ic)
            => GetRelatedTableNameFromColumnName(ic.Name);

        public static EntityTable GetRelatedTable(this INamedBuffer<int> ic, Document doc)
            => doc.GetTable(ic.GetRelatedTableName());

        public static IArray<EntityTable> GetRelatedTables(this EntityTable et, Document doc)
            => et.IndexColumns.Values.Select(v => GetRelatedTable(v, doc));

        public static IArray<INamedBuffer> GetExpandedColumns(this EntityTable et)
            => et.Columns.Concatenate(et.GetRelatedTables(et.Document).Select(t => t.Columns).Flatten());

        /// <summary>
        /// Used for converting entity table names that start with "table:".
        /// </summary>
        public static string SimplifiedName(this string s)
            => s.Substring(s.IndexOf(':') + 1);

        public static string GetShortName(this INamedBuffer c)
            => c.Name.SimplifiedName();

        public static string GetTableQualifiedName(this INamedBuffer c, EntityTable table)
            => $"{table.Name}:{c.GetShortName()}";

        public static string GetTableKeyFromTableName(string name)
            => name.Substring(Math.Max(name.LastIndexOf('.'), name.LastIndexOf(':')) + 1);

        public static EntityTable GetTable(this Document doc, string name)
            => doc.EntityTables[GetTableKeyFromTableName(name)];

        public static SerializableSceneNode Translate(this SerializableSceneNode node, Vector3 v)
            => new SerializableSceneNode
            {
                Geometry = node.Geometry,
                Instance = node.Instance,
                Parent = node.Parent,
                Transform = node.Transform.Translate(v)
            };

        /// <summary>
        /// Returns the substring after the last forward or backward slash. If no slashes
        /// exist, returns the provided value.
        /// </summary>
        public static string GetPlatformInvariantFilenameAndExtension(string value)
        {
            var lastIndex = value.LastIndexOfAny(new[] { '\\', '/' });

            return lastIndex < 0 || lastIndex >= value.Length - 1
                ? value
                : value.Substring(lastIndex + 1);
        }

        public static string GetTextureFileName(this INamedBuffer buffer)
            => GetPlatformInvariantFilenameAndExtension(buffer.Name);

        public static readonly string[] TextureExtensions =
        {
            ".png",
            ".jpg",
            ".jpeg",
            ".bmp"
        };

        /// <summary>
        /// Returns true if the given extension (with a leading period, ex: ".png") is a texture. 
        /// </summary>
        public static bool IsTextureExtension(string ext)
            => TextureExtensions.Any(t => string.Equals(ext, t, StringComparison.OrdinalIgnoreCase));

        // TODO: move this into IDocument 
        public static string TexturePrefix = "textures\\";

        public static bool IsTexture(this INamedBuffer buffer)
            => buffer.Name.StartsWith(TexturePrefix);

        public static INamedBuffer GetTexture(this Document doc, string textureName)
            => doc.Assets.GetOrDefault(TexturePrefix + textureName);

        public static IEnumerable<INamedBuffer> GetTextures(this Document d)
            => d.Assets.Values.Where(k => IsTexture(k));        

        public static SerializableEntityTable FindEntityTable(this SerializableDocument doc, string name)
        {
            // Not very efficient ... but there are not very many entity tables
            foreach (var et in doc.EntityTables)
                if (et.Name == name)
                    return et;
            return null;
        }

        public static IArray<int> FindStringColumn(this SerializableEntityTable table, string name)
        {
            foreach (var col in table.StringColumns)
                if (col.Name == name)
                    return col.GetTypedData().ToIArray();
            return null;
        }

        public static SerializableDocument SetFileName(this SerializableDocument doc, string fileName)
        {
            doc.FileName = fileName;
            return doc;
        }

        /* TODO: once this has been validated to be not important, we can remove it.

        public static IArray<object> GetRelatedRow(this INamedBuffer<int> ec, int n, Document doc)
            => ec.GetRelatedTable(doc)?.GetRow(ec.GetTypedData()[n]) ?? Empty<object>();

        public static IArray<IArray<object>> GetRelatedRows(this EntityTable table, int n)
            => table.IndexColumns.Values.Select(c => c.GetRelatedRow(n));

        public static IArray<object> GetExpandedRow(this EntityTable et, int n)
            => et.GetRow(n).Concatenate(et.GetRelatedRows(n).Flatten());

        public static IArray<IArray<object>> GetExpandedRows(this EntityTable et)
            => et.NumRows.Select(et.GetExpandedRow);
        */
    }
}
