using System;
using System.Collections.Generic;
using System.Linq;
using Vim.Math3d;
using Vim.G3d;
using Vim.LinqArray;
using Vim.BFast;
using System.IO;
using Vim.DotNetUtilities;
using static Vim.DataFormat.Serializer;

namespace Vim.DataFormat
{
    public class DocumentBuilder
    {
        public Dictionary<string, TableBuilder> Tables = new Dictionary<string, TableBuilder>();
        public Dictionary<string, byte[]> Assets = new Dictionary<string, byte[]>();
        public static SerializableHeader Header = new SerializableHeader();
        public List<SerializableSceneNode> Nodes = new List<SerializableSceneNode>();
        public List<GeometryBuilder> Geometries = new List<GeometryBuilder>();
        public bool UseColors { get; }

        public DocumentBuilder(bool useColors = false)
        {
            UseColors = useColors;
        }

        public TableBuilder GetTableBuilderOrCreate(string name)
        {
            if (!Tables.ContainsKey(name))
                Tables.Add(name, new TableBuilder(name));
            return Tables[name];
        }

        public TableBuilder CreateTableBuilder(string name)
        {
            if (Tables.ContainsKey(name))
                throw new Exception($"Table {name} already exists");
            return GetTableBuilderOrCreate(name);
        }

        public DocumentBuilder AddAsset(string name, byte[] asset)
        {
            if (!Assets.ContainsKey(name))
                Assets.Add(name, asset);
            return this;
        }

        public DocumentBuilder AddGeometry(GeometryBuilder g)
        {
            Geometries.Add(g);
            return this;
        }

        public DocumentBuilder AddGeometries(IEnumerable<GeometryBuilder> gs)
            => gs.Aggregate(this, (db, g) => db.AddGeometry(g));

        public DocumentBuilder AddAsset(INamedBuffer b)
            => AddAsset(b.Name, b.ToBytes());

        public DocumentBuilder AddNode(Matrix4x4 transform, int geometryIndex, int instanceId = -1, int parentId = -1)
        {
            Nodes.Add(new SerializableSceneNode {
                Transform = transform,
                Geometry = geometryIndex,
                Instance = instanceId,
                Parent = parentId });
            return this;
        }

        public IEnumerable<string> GetAllStrings()
            => Tables.Values.SelectMany(tb => tb.GetAllStrings());

        public Dictionary<string, int> ComputeStringLookup()
        {
            var r = new Dictionary<string, int>() { { "", 0 } };
            foreach (var x in GetAllStrings())
                r.AddIfNotPresent(x, 0);
            return r;
        }

        public IEnumerable<string> ComputeStrings(Dictionary<string, int> stringLookup)
        {
            var id = 0;
            foreach (var k in stringLookup.Keys.ToArray())
                stringLookup[k] = id++;
            return stringLookup.OrderBy(kv => kv.Value).Select(kv => kv.Key);
        }

        public List<SerializableEntityTable> ComputeEntityTables(Dictionary<string, int> stringLookup)
        {
            // Create the new Entity tables
            var tableList = new List<SerializableEntityTable>();

            // Create the geometry table 
            {
                var tb = GetTableBuilderOrCreate(TableNames.Geometry);
                tb.Clear();
                // We have to force evaluation because as an enumerable the bounding box could be evaluated 6 times more
                tb.AddField(Geometries.Select(g => AABox.Create(g.Vertices)).ToArray(), "Box");
                tb.AddField(Geometries.Select(g => g.Vertices.Count), "VertexCount");
                tb.AddField(Geometries.Select(g => g.Indices.Count / 3), "FaceCount");
            }

            // TODO: add bounding box information to the nodes 

            foreach (var tb in Tables.Values)
            {
                var table = new SerializableEntityTable
                {
                    // Set the table name
                    Name = tb.Name,

                    // Convert the columns to named buffers 
                    IndexColumns = tb.IndexColumns
                        .Select(kv => kv.Value.ToNamedBuffer(kv.Key))
                        .ToList(),
                    NumericColumns = tb.NumericColumns
                        .Select(kv => kv.Value.ToNamedBuffer(kv.Key))
                        .ToList(),
                    StringColumns = tb.StringColumns
                        .Select(kv => kv.Value
                            .Select(s => stringLookup[s ?? string.Empty])
                            .ToArray()
                            .ToNamedBuffer(kv.Key))
                        .ToList()
                };

                // Assure that all columns are the same length
                var nRows = -1;
                foreach (var c in table.IndexColumns)
                {
                    var n = c.NumElements();
                    if (nRows < 0) nRows = n;
                    else if (nRows != n) throw new Exception($"Invalid number of rows {n} expected {nRows}");
                }
                foreach (var c in table.NumericColumns)
                {
                    var n = c.NumElements();
                    if (nRows < 0) nRows = n;
                    else if (nRows != n) throw new Exception($"Invalid number of rows {n} expected {nRows}");
                }
                foreach (var c in table.StringColumns)
                {
                    var n = c.NumElements();
                    if (nRows < 0) nRows = n;
                    else if (nRows != n) throw new Exception($"Invalid number of rows {n} expected {nRows}");
                }

                // Properties 
                table.Properties = tb.Properties.Select(p => new SerializableProperty
                {
                    EntityIndex = p.EntityId,
                    Name = stringLookup[p.Name],
                    Value = stringLookup[p.Value]
                }).ToArray();

                tableList.Add(table);
            }


            return tableList;
        }

        /// <summary>
        /// This is a helper class for writing the really big G3Ds needed in a VIM
        /// </summary>
        public class BigG3dWriter : IBFastComponent
        {
            public INamedBuffer Meta { get; }
            public string[] Names { get; }
            public long[] Sizes { get; }
            public BFastHeader Header { get; }
            public List<GeometryBuilder> Geometries { get; }
            public int[] VertexOffsets { get; }
            public int[] IndexOffsets { get; }
            public bool UseColors { get; }

            public BigG3dWriter(List<GeometryBuilder> geometries, G3dHeader? header = null, bool useColors = false)
            {
                UseColors = useColors;
                Geometries = geometries;

                // Compute the Vertex offsets and index offsets 
                VertexOffsets = new int[geometries.Count];
                IndexOffsets = new int[geometries.Count];

                var n = geometries.Count;

                for (var i = 1; i < n; ++i)
                {
                    VertexOffsets[i] = VertexOffsets[i - 1] + geometries[i - 1].Vertices.Count;
                    IndexOffsets[i] = IndexOffsets[i - 1] + geometries[i - 1].Indices.Count;
                }

                var totalVertices = n == 0 ? 0 : VertexOffsets[n - 1] + geometries[n - 1].Vertices.Count;
                var totalIndices = n == 0 ? 0 : IndexOffsets[n - 1] + geometries[n - 1].Indices.Count;
                var totalFaces = totalIndices / 3;

                Meta = (header ?? G3dHeader.Default).ToBytes().ToNamedBuffer("meta");


                var names = new List<string> { Meta.Name }.Concat(VimConstants.G3dLegacySchema.Attributes).ToList();
                var sizes = new List<long>() {
                    Meta.NumBytes(),
                    12 * (long)totalVertices, // Position,
                    4 * (long)totalIndices, // Index,
                    8 * (long)totalVertices, // Uv,
                    4 * (long)totalFaces, // MaterialId,
                    4 * (long)totalFaces, // GroupId,
                    4 * (long)IndexOffsets.Length, // IndexOffsets
                    4 * (long)VertexOffsets.Length, // VertexOffsets 
                };

                if (useColors)
                {
                    names.Add("g3d:vertex:color:0:float32:4");
                    sizes.Add(16 * (long)totalVertices); // colors
                }

                Names = names.ToArray();
                Sizes = sizes.ToArray();

                Header = BFast.BFast.CreateBFastHeader(Sizes, Names);
            }

            public long GetSize()
                => BFast.BFast.ComputeNextAlignment(Header.Preamble.DataEnd);

            public void Write(Stream stream)
            {
                // TODO: validate in debug mode that this is producing the current data model. Look at the schema!

                stream.WriteBFastHeader(Header);
                stream.WriteBFastBody(Header, Names, Sizes, (_stream, index, name, size) =>
                {
                    switch (name)
                    {
                        case "meta":
                            _stream.Write(Meta);
                            break;
                        case VimConstants.G3dLegacySchema.Position:
                            Geometries.ForEach(g => stream.Write(g.Vertices.ToArray()));
                            break;
                        case "g3d:vertex:color:0:float32:4":
                            if (UseColors)
                            {
                                Geometries.ForEach(g => stream.Write(g.Colors.ToArray()));
                            }
                            break;
                        case VimConstants.G3dLegacySchema.Index:
                            for (var i = 0; i < Geometries.Count; ++i)
                            {
                                var g = Geometries[i];
                                var offset = VertexOffsets[i];
                                stream.Write(g.Indices.Select(idx => idx + offset).ToArray());
                            }
                            break;
                        case VimConstants.G3dLegacySchema.Uv:
                            Geometries.ForEach(g => stream.Write(g.UVs.ToArray()));
                            break;
                        case VimConstants.G3dLegacySchema.MaterialId:
                            Geometries.ForEach(g => stream.Write(g.MaterialIds.ToArray()));
                            break;
                        case VimConstants.G3dLegacySchema.GroupId:
                            Geometries.ForEach(g => stream.Write(g.FaceGroupIds.ToArray()));
                            break;
                        case VimConstants.G3dLegacySchema.IndexOffset:
                            stream.Write(IndexOffsets);
                            break;
                        case VimConstants.G3dLegacySchema.VertexOffset:
                            stream.Write(VertexOffsets);
                            break;
                        default:
                            throw new Exception($"Not a recognized geometry buffer: {name}");
                    }
                    return size;
                });
            }
        }

        public void Write(string filePath)
        {
            using (var stream = File.OpenWrite(filePath))
                Write(stream);
        }

        public void Write(Stream stream)
        {
            var bldr = new BFastBuilder();

            // Add buffer 
            bldr.Add(BufferNames.Header, Header.ToString().ToBytesUtf8().ToBuffer());

            // add String table
            var stringLookup = ComputeStringLookup();
            bldr.Add(BufferNames.Strings, ComputeStrings(stringLookup).PackStrings().ToBuffer());

            // Add Assets
            bldr.Add(BufferNames.Assets, Assets.Select(kv => kv.Value.ToNamedBuffer(kv.Key)));

            // Add entity tables
            var entityTables = ComputeEntityTables(stringLookup);
            bldr.Add(BufferNames.Entities, entityTables.ToBFastBuilder());

            // Write the geometry 
            bldr.Add(BufferNames.Geometry, new BigG3dWriter(Geometries, null, UseColors));

            // Add the nodes 
            bldr.Add(BufferNames.Nodes, Nodes.ToArray().ToBuffer());

            // Now we can write everything out 
            bldr.Write(stream);
        }
    }
}
