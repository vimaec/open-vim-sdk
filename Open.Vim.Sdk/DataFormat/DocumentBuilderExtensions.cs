using System;
using System.Collections.Generic;
using System.Linq;
using Vim.DotNetUtilities;
using Vim.Geometry;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.DataFormat
{
    public static class DocumentBuilderExtensions
    {
        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<int> values, string name)
            => tb.AddColumn(name, values);

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<byte> values, string name)
            => tb.AddColumn(name, values);

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<float> values, string name)
            => tb.AddColumn(name, values);

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<double> values, string name)
            => tb.AddColumn(name, values);

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<Vector2> values, string name)
            => tb.AddField(values.Select(x => x.X), name + ".X")
                .AddField(values.Select(x => x.Y), name + ".Y");

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<DVector2> values, string name)
            => tb.AddField(values.Select(x => x.X), name + ".X")
                .AddField(values.Select(x => x.Y), name + ".Y");

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<Vector3> values, string name)
            => tb.AddField(values.Select(x => x.X), name + ".X")
                .AddField(values.Select(x => x.Y), name + ".Y")
                .AddField(values.Select(x => x.Z), name + ".Z");

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<DVector3> values, string name)
            => tb.AddField(values.Select(x => x.X), name + ".X")
                .AddField(values.Select(x => x.Y), name + ".Y")
                .AddField(values.Select(x => x.Z), name + ".Z");

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<DAABox> values, string name)
            => tb.AddField(values.Select(x => x.Min), name + ".Min")
                .AddField(values.Select(x => x.Max), name + ".Max");

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<AABox> values, string name)
            => tb.AddField(values.Select(x => x.Min), name + ".Min")
                .AddField(values.Select(x => x.Max), name + ".Max");

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<string> values, string name)
            => tb.AddColumn(name, values.ToArray());

        public static TableBuilder AddField(this TableBuilder tb, IEnumerable<bool> values, string name)
            => tb.AddColumn(name, values);

        public static TableBuilder AddRelation(this TableBuilder tb, IEnumerable<int> values, string tableName, string fieldName)
            => tb.AddIndexColumn(tableName, fieldName, values);

        public static void AddNumericValues(this TableBuilder tb, string name, IEnumerable<double> vals)
        {
            if (tb.NumericColumns.ContainsKey(name))
                tb.NumericColumns[name] = tb.NumericColumns[name].Concat(vals).ToArray();
            else
                tb.NumericColumns.Add(name, Enumerable.Repeat(0.0, tb.NumRows).Concat(vals).ToArray());
        }

        public static void AddStringValues(this TableBuilder tb, string name, IEnumerable<string> vals)
        {
            if (tb.StringColumns.ContainsKey(name))
                tb.StringColumns[name] = tb.StringColumns[name].Concat(vals).ToArray();
            else
                tb.StringColumns.Add(name, Enumerable.Repeat("", tb.NumRows).Concat(vals).ToArray());
        }

        public static Dictionary<string, int> GetOriginalEntityCounts(this DocumentBuilder db)
            => db.Tables.ToDictionary(kv => kv.Key, kv => kv.Value.NumRows);

        public static void AddIndexValues(this TableBuilder tb, string name, IEnumerable<int> vals, Dictionary<string, int> originalEntityCounts)
        {
            var tableName = DocumentExtensions.GetRelatedTableNameFromColumnName(name);
            var offset = 0;
            if (originalEntityCounts.ContainsKey(name))
                offset = originalEntityCounts[name];
            var offsetVals = vals.Select(v => v + offset);

            if (tb.IndexColumns.ContainsKey(name))
                tb.IndexColumns[name] = tb.IndexColumns[name].Concat(offsetVals).ToArray();
            else
                tb.IndexColumns.Add(name, Enumerable.Repeat(-1, tb.NumRows).Concat(offsetVals).ToArray());
        }

        public static void AddEntityProps(this TableBuilder tb, EntityTable et, int entityOffset)
        {
            foreach (var p in et.PropertyLists.SelectMany(x => x.Value))
                tb.AddProperty(p.Id + entityOffset, p.Name, p.Value);
        }

        public static IMesh ToIMesh(this GeometryBuilder gb)
            => Primitives.TriMesh(
                gb.Vertices.ToIArray(),
                gb.Indices.ToIArray(),
                gb.UVs.ToIArray(),
                gb.Colors.ToIArray(),
                gb.MaterialIds.ToIArray(),
                gb.FaceGroupIds.ToIArray());

        public static GeometryBuilder ToGeometryBuilder(this IMesh m)
            => new GeometryBuilder
            {
                Vertices = m.Vertices.ToList(),
                Indices = m.Indices.ToList(),
                UVs = m.VertexUvs?.ToList() ?? Vector2.Zero.Repeat(m.NumVertices).ToList(),
                Colors = m.VertexColors?.ToList() ?? Vector4.Zero.Repeat(m.NumVertices).ToList(),
                MaterialIds = m.FaceMaterialIds?.ToList() ?? 0.Repeat(m.NumFaces).ToList(),
                FaceGroupIds = m.FaceGroups?.ToList() ?? 0.Repeat(m.NumFaces).ToList()
            };

        public static GeometryBuilder AddMesh(this DocumentBuilder db, IMesh m)
        {
            var gb = m.ToGeometryBuilder();
            db.AddGeometry(gb);
            return gb;
        }

        // TODO: it seems to me that the DocumentBuilder should track the "IMesh" as it is added. 
        // However I have to take into account the fact that the DocumentBUilder is aware of GeometryBuilder but not IMesh
        public static int AddNode(this DocumentBuilder db, Matrix4x4 transform, IMesh mesh, IndexedSet<IMesh> meshes)
        {
            var gid = mesh != null ? meshes.GetOrAdd(mesh) : -1;
            db.AddNode(transform, gid, -1, -1);
            return db.Nodes.Count - 1;
        }

        public static T[] RemapData<T>(this T[] self, List<int> remapping = null)
            => remapping?.Select(x => self[x])?.ToArray() ?? self;

        public static TableBuilder CreateTableCopy(this DocumentBuilder db, EntityTable table, List<int> remapping = null)
        {
            var name = table.Name.SimplifiedName();
            var tb = db.CreateTableBuilder(name);

            foreach (var col in table.IndexColumns.Values.ToEnumerable())
                tb.AddIndexColumn(col.GetRelatedTableName(), col.GetFieldName(), col.GetTypedData().RemapData(remapping));

            foreach (var col in table.NumericColumns.Values.ToEnumerable())
                tb.AddColumn(col.Name, col.GetTypedData().RemapData(remapping));

            foreach (var col in table.StringColumns.Values.ToEnumerable())
            {
                var strings = col.GetTypedData().Select(i => table.Document.StringTable.ElementAtOrDefault(i, null));
                tb.AddColumn(col.Name, strings.ToArray().RemapData(remapping));
            }

            // TODO: the remapping is non-trivial. Any table that is remapped, suggest that any references to the old table
            // also need to be remapped. I am not doing that right now, because this is a patch used only for Node tables.
            // Eventually I need to do something more sophisticated. I can't assume that all tables are remapped tables.
            // A simpler and more robust system would be to create tables from objects. There could be a big performance or
            // memory impact of that approach if I am not careful. It would be more robust right up to the use case of
            // merging data of data models, not yet anticipated. 

            if (remapping != null && table.Properties.Any())
                throw new Exception("Currently can't remap tables with properties");

            foreach (var p in table.Properties)
                tb.AddProperty(p.Id, p.Name, p.Value);

            return tb;
        }

        public static DocumentBuilder CopyTablesFrom(this DocumentBuilder db, Document doc, List<int> nodeIndexRemapping = null)
        {
            foreach (var table in doc.EntityTables.Values.ToEnumerable())
            {
                var name = table.Name.SimplifiedName();

                // Don't copy tables that are computed automatically
                if (VimConstants.ComputedTableNames.Contains(name))
                    continue;

                var remapping = name == TableNames.Node
                    ? nodeIndexRemapping
                    : null;

                db.CreateTableCopy(table, remapping);
            }

            return db;
        }
    }
}
