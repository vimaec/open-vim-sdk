using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Vim.BFast;
using Vim.DotNetUtilities;
using Vim.Geometry;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.DataFormat.Tests
{
    public static class DataFormatTests
    {
        public static void PrintStrings(this IArray<string> data)
        {
            var d = new Dictionary<string, int>();
            foreach (var x in data.Select(x => x ?? "_NULL_").ToEnumerable())
            {
                if (!d.ContainsKey(x))
                    d.Add(x, 0);
                d[x] += 1;
            }

            foreach (var kv in d.OrderBy(kv => kv.Key))
                Console.WriteLine($"String {kv.Key} has {kv.Value} instances");
        }

        public static SerializableSceneNode RemoveGeometry(this SerializableSceneNode node)
            => new SerializableSceneNode
            {
                Geometry = -1,
                Transform = node.Transform,
                Instance = -1,
                Parent = node.Parent,
            };

        public static void AssertEquals(INamedBuffer b1, INamedBuffer b2)
        {
            Assert.AreEqual(b1.Name, b2.Name);
            // We can't expect all buffers to be the same, because of non-determinism of parallelism when generating string table.
            //Assert.AreEqual(b1.Bytes.Length, b2.Bytes.Length);
            //Assert.AreEqual(b1.Bytes.ToArray(), b2.Bytes.ToArray());
        }

        public static void AssertEquals(SerializableEntityTable t1, SerializableEntityTable t2)
        {
            Assert.AreEqual(t1.Name, t2.Name);
            Assert.AreEqual(t1.IndexColumns.Count, t2.IndexColumns.Count);
            for (var i = 0; i < t1.IndexColumns.Count; ++i)
                AssertEquals(t1.IndexColumns[i], t2.IndexColumns[i]);
            Assert.AreEqual(t1.NumericColumns.Count, t2.NumericColumns.Count);
            for (var i = 0; i < t1.NumericColumns.Count; ++i)
                AssertEquals(t1.NumericColumns[i], t2.NumericColumns[i]);
            Assert.AreEqual(t1.StringColumns.Count, t2.StringColumns.Count);
            for (var i = 0; i < t1.StringColumns.Count; ++i)
                AssertEquals(t1.StringColumns[i], t2.StringColumns[i]);

            Assert.AreEqual(t1.Properties.Length, t2.Properties.Length);

            /* Can't expect the numerical values to be precise, because of non-determinism of parallelism when generating string table.
            for (var i=0; i < t1.Properties.Length; ++i)
            {
                var p1 = t1.Properties[i];
                var p2 = t2.Properties[i];
                Assert.AreEqual(p1.EntityId, p2.EntityId);
                Assert.AreEqual(p1.Name, p2.Name);
                Assert.AreEqual(p1.Value, p2.Value);
            }
            */
        }

        public static void AssertEquals(SerializableDocument d1, SerializableDocument d2, bool compareStringTables = true)
        {
            Assert.AreEqual(d1.EntityTables.Count, d2.EntityTables.Count);
            Assert.AreEqual(d1.Header, d2.Header);
            Assert.AreEqual(d1.Nodes, d2.Nodes);
            if (compareStringTables)
            {
                var strings1 = d1.StringTable.OrderBy(x => x).ToArray();
                var strings2 = d2.StringTable.OrderBy(x => x).ToArray();
                Assert.AreEqual(strings1, strings2);
            }

            // TODO: upgrade to a proper Geometry comparer
            //Assert.AreEqual(d1.Geometry.FaceCount(), d2.Geometry.FaceCount());
            //Assert.AreEqual(d1.Geometry.VertexCount(), d2.Geometry.VertexCount());

            Assert.AreEqual(d1.Assets.Length, d2.Assets.Length);

            for (var i = 0; i < d1.EntityTables.Count; ++i)
            {
                AssertEquals(d1.EntityTables[i], d2.EntityTables[i]);
            }
        }

        public static void AssertEquals(EntityTable et1, EntityTable et2)
        {
            Assert.AreEqual(et1.Name, et2.Name);
            Assert.AreEqual(et1.NumericColumns.Keys.ToArray(), et2.NumericColumns.Keys.ToArray());          
            Assert.AreEqual(et1.IndexColumns.Keys.ToArray(), et2.IndexColumns.Keys.ToArray());
            Assert.AreEqual(et1.StringColumns.Keys.ToArray(), et2.StringColumns.Keys.ToArray());
            Assert.AreEqual(et1.Properties.Select(p => p.Id).ToArray(), et2.Properties.Select(p => p.Id).ToArray());
            Assert.AreEqual(et1.Properties.Select(p => p.Name).ToArray(), et2.Properties.Select(p => p.Name).ToArray());
            Assert.AreEqual(et1.Properties.Select(p => p.Value).ToArray(), et2.Properties.Select(p => p.Value).ToArray());

            Assert.AreEqual(
                et1.Columns.Select(ec => ec.Name).ToArray(),
                et2.Columns.Select(ec => ec.Name).ToArray());

            Assert.AreEqual(
                et1.Columns.Select(ec => ec.NumElements()).ToArray(),
                et2.Columns.Select(ec => ec.NumElements()).ToArray());

            Assert.AreEqual(
                et1.Columns.Select(ec => ec.NumBytes()).ToArray(),
                et2.Columns.Select(ec => ec.NumBytes()).ToArray());
        }

        public static void AssertEquals(Document d1, Document d2, bool skipGeometryAndNodes = false)
        {
            var schema1 = VimSchema.Create(d1);
            var schema2 = VimSchema.Create(d2);
            Assert.IsTrue(VimSchema.SameSchema(schema1, schema2));

            Assert.AreEqual(d1.EntityTables.Keys.ToArray(), d2.EntityTables.Keys.ToArray());
            foreach (var k in d1.EntityTables.Keys.ToEnumerable())
            {
                if (skipGeometryAndNodes && (k.ToLowerInvariant().Contains("node") || k.ToLowerInvariant().Contains("geometry")))
                    continue;
                AssertEquals(d1.EntityTables[k], d2.EntityTables[k]);
            }
        }

        public static void AssertEquals<T>(IArray<T> x1, IArray<T> x2, Action<T, T> compareFunc)
        {
            Assert.AreEqual(x1.Count, x2.Count);
            for (var i = 0; i < x1.Count; ++i)
                compareFunc(x1[i], x2[i]);
        }

        public static void AssertEquals<T>(IList<T> x1, IList<T> x2, Action<T, T> compareFunc)
        {
            Assert.AreEqual(x1.Count, x2.Count);
            for (var i = 0; i < x1.Count; ++i)
                compareFunc(x1[i], x2[i]);
        }

        public static void AssertEquals(ISceneNode node1, ISceneNode node2)
        {
            Assert.AreEqual(node1.Id, node2.Id);
            //Assert.AreEqual(node1.Transform, node2.Transform);
        }

        public static string ProjectFolder = Path.Combine(Assembly.GetExecutingAssembly().Location, "..", "..", "..", "..");
        public static string DataFolder = Path.Combine(ProjectFolder, "data");
        public static string WolfordFile = Path.Combine(DataFolder, "Wolford_Residence.v3-1-0.vim");

        [Test]
        public static void TestLoad()
        {
            var sd = Serializer.Deserialize(WolfordFile);
            var d = new Document(sd);
            Assert.IsNotNull(d);
            foreach (var attr in d.Geometry.Attributes.ToEnumerable())
                Console.WriteLine(attr.Name);

            Assert.AreEqual("g3d:all:facesize:0:int32:1", d.Geometry.Attributes[0].Name);
            Assert.AreEqual("g3d:corner:index:0:int32:1", d.Geometry.Attributes[1].Name);
            Assert.AreEqual("g3d:face:groupid:0:int32:1", d.Geometry.Attributes[2].Name);
            Assert.AreEqual("g3d:face:materialid:0:int32:1", d.Geometry.Attributes[3].Name);
            Assert.AreEqual("g3d:subgeometry:indexoffset:0:int32:1", d.Geometry.Attributes[4].Name);
            Assert.AreEqual("g3d:subgeometry:vertexoffset:0:int32:1", d.Geometry.Attributes[5].Name);
            Assert.AreEqual("g3d:vertex:position:0:float32:3", d.Geometry.Attributes[6].Name);
            Assert.AreEqual("g3d:vertex:uv:0:float32:2", d.Geometry.Attributes[7].Name);
        }

        // TODO:
        public static void TestRemovingGeometry()
        {
            // Remove some of the nodes
        }

        public static DocumentBuilder FilterOutliers(this DocumentBuilder db, float maxDist = 100000)
        {
            // Filter outliers (temp computation)
            var stats = db.Nodes.Select(n => n.Transform.Translation).Stats();
            var avg = stats.Average();
            db.Nodes = db.Nodes.Where(n => n.Transform.Translation.Distance(avg) < maxDist).ToList();
            return db;
        }
    }
}
