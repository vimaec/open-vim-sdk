using System.Collections.Generic;
using System.Linq;
using System;
using Vim.BFast;
using Vim.G3d;
using System.IO;
using Vim.LinqArray;
using System.Diagnostics;
using Vim.DotNetUtilities;
using System.Text;

namespace Vim.DataFormat
{
    public static class Serializer
    {
        public static class BufferNames
        {
            public const string Header  = "header";
            public const string Assets = "assets";
            public const string Entities = "entities";
            public const string Strings = "strings";
            public const string Geometry = "geometry";
            public const string Nodes = "nodes";
        }

        public static List<INamedBuffer> ToBuffers(this SerializableEntityTable table)
        {
            var r = new List<INamedBuffer>();
            r.AddRange(table.NumericColumns.Select(b
                => b.ToNamedBuffer(VimConstants.NumberColumnNamePrefix + b.Name)));
            r.AddRange(table.IndexColumns.Select(b
                => b.ToNamedBuffer(VimConstants.IndexColumnNamePrefix + b.Name)));
            r.AddRange(table.StringColumns.Select(b
                => b.ToNamedBuffer(VimConstants.StringColumnNamePrefix + b.Name)));
            r.Add(table.Properties.ToNamedBuffer(VimConstants.PropertiesBufferName));
            return r;
        }

        public static INamedBuffer StripPrefix(this INamedBuffer b)
            => b.ToNamedBuffer(b.Name.SimplifiedName());

        public static SerializableEntityTable ReadEntityTable(this Stream stream)
        {
            var et = new SerializableEntityTable();
            stream.ReadBFast((_stream, name, size) =>
            {
                // Strip prefix 
                var simpName = name.SimplifiedName();

                // Switch based on the name
                if (name.StartsWith(VimConstants.IndexColumnNamePrefix))
                {
                    var buffer = stream.ReadBufferFromNumberOfBytes<int>(size);
                    et.IndexColumns.Add(buffer.ToNamedBuffer(simpName));
                }
                else if (name.StartsWith(VimConstants.NumberColumnNamePrefix))
                {
                    var buffer = stream.ReadBufferFromNumberOfBytes<double>(size);
                    et.NumericColumns.Add(buffer.ToNamedBuffer(simpName));
                }
                else if (name.StartsWith(VimConstants.StringColumnNamePrefix))
                {
                    var buffer = stream.ReadBufferFromNumberOfBytes<int>(size);
                    et.StringColumns.Add(buffer.ToNamedBuffer(simpName));
                }
                else if (name == VimConstants.PropertiesBufferName)
                {
                    et.Properties = stream.ReadArrayFromNumberOfBytes<SerializableProperty>(size);
                }
                else
                {
                    Debug.Assert(false, $"{name} is not a recognized entity table buffer");
                }
                return et;
            });
            return et;
        }

        public static BFastBuilder ToBFastBuilder(this List<SerializableEntityTable> entityTables)
        {
            var bldr = new BFastBuilder();
            foreach (var et in entityTables)
                bldr.Add(VimConstants.TableName + et.Name, et.ToBuffers());
            return bldr;
        }

        public static void Serialize(this SerializableDocument doc, Stream stream)
        {
            var bldr = new BFastBuilder();
            var headerBuffer = doc.Header.ToString().ToBytesUtf8();
            bldr.Add(BufferNames.Header, headerBuffer.ToBuffer());
            bldr.Add(BufferNames.Assets, doc.Assets);
            bldr.Add(BufferNames.Entities, doc.EntityTables.ToBFastBuilder());
            bldr.Add(BufferNames.Strings, doc.StringTable.PackStrings().ToBuffer());
            bldr.Add(BufferNames.Geometry, doc.Geometry.ToG3DWriter());
            bldr.Add(BufferNames.Nodes, doc.Nodes.ToBuffer());
            bldr.Write(stream);
        }

        public static void Serialize(this SerializableDocument document, string filePath)
        {
            using (var stream = File.OpenWrite(filePath))
                document.Serialize(stream);
        }

        public static List<SerializableEntityTable> ReadEntityTables(this Stream stream)
            => stream.ReadBFast((_stream, name, size) =>
            {
                var et = stream.ReadEntityTable();
                et.Name = name;
                return et;
            }).Select(pair => pair.Item2).ToList();

        public static SerializableDocument ReadBuffer(this SerializableDocument doc, Stream stream, string name, long numBytes)
        {
            Debug.WriteLine($"Reading buffer {name} of size {Util.BytesToString(numBytes)}");
            switch (name)
            {
                case BufferNames.Header:
                    var bytes = stream.ReadArray<byte>((int)numBytes);
                    doc.Header = SerializableHeader.Parse(Encoding.UTF8.GetString(bytes));
                    return doc;

                case BufferNames.Assets:
                    if (doc.Options?.SkipAssets == true)
                    {
                        Debug.WriteLine("Skipping assets");
                        stream.Advance(numBytes);
                        return doc;
                    }
                    doc.Assets = stream.ReadBFast().ToArray();
                    return doc;

                case BufferNames.Strings:
                    var stringBytes = stream.ReadArray<byte>((int)numBytes);
                    var joinedStringTable = Encoding.UTF8.GetString(stringBytes);
                    doc.StringTable = joinedStringTable.Split('\0');
                    return doc;

                case BufferNames.Geometry:
                    if (doc.Options?.SkipGeometry == true)
                    {
                        Debug.WriteLine("Skipping geometry");
                        stream.Advance(numBytes);
                        return doc;
                    }
                    doc.Geometry = G3D.Read(stream);
                    return doc;

                case BufferNames.Nodes:
                    if (doc.Options?.SkipGeometry == true)
                    {
                        Debug.WriteLine("Skipping nodes");
                        stream.Advance(numBytes);
                        return doc;
                    }
                    var cnt = (int)(numBytes / SerializableSceneNode.Size);
                    Debug.Assert(numBytes % SerializableSceneNode.Size == 0, $"Number of bytes is not divisible by {SerializableSceneNode.Size}");
                    if (cnt < 0)
                        throw new Exception($"More than {int.MaxValue} items in array");
                    doc.Nodes = stream.ReadArray<SerializableSceneNode>(cnt);
                    return doc;

                case BufferNames.Entities:
                    doc.EntityTables = ReadEntityTables(stream);
                    return doc;
            }

            // NOTE: unrecognized buffers are not an error. 
            Debug.WriteLine($"Unrecognized buffer {name}");
            stream.ReadArray<byte>((int)numBytes);
            return doc;
        }

        public static SerializableDocument Deserialize(Stream stream, LoadOptions loadOptions = null)
        {
            var doc = new SerializableDocument { Options = loadOptions };
            stream.ReadBFast(doc.ReadBuffer).Count();
            return doc;
        }

        public static SerializableDocument Deserialize(string filePath, LoadOptions loadOptions = null)
        {
            using (var stream = File.OpenRead(filePath))
                return Deserialize(stream, loadOptions).SetFileName(filePath);
        }

    }
}
