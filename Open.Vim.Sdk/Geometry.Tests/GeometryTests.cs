using System;
using System.Linq;
using NUnit.Framework;
using Vim.G3d;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.Geometry.Tests
{
    public static class GeometryTests
    {
        public static IMesh XYTriangle = new[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(1f, 0f, 0f) }.ToIArray().TriMesh(3.Range());
        public static IMesh XYQuad = new[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(1f, 1f, 0f), new Vector3(1f, 0f, 0f) }.ToIArray().QuadMesh(4.Range());
        public static IMesh XYQuadFromFunc = Primitives.QuadMesh(uv => uv.ToVector3(), 1, 1);
        public static IMesh XYQuad2x2 = Primitives.QuadMesh(uv => uv.ToVector3(), 2, 2);
        public static IMesh XYTriangleTwice = XYTriangle.Merge(XYTriangle.Translate(new Vector3(1, 0, 0)));

        public static readonly Vector3[] TestTetrahedronVertices = { Vector3.Zero, Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };
        public static readonly int[] TestTetrahedronIndices = { 0, 1, 2, 0, 3, 1, 1, 3, 2, 2, 3, 0 };

        public static IMesh Tetrahedron =
            TestTetrahedronVertices.ToIArray().TriMesh(TestTetrahedronIndices.ToIArray());

        public static IMesh Torus = Primitives.Torus(10, 0.2f, 10, 24);

        static IMesh RevolvedVerticalCylinder(float height, float radius, int verticalSegments, int radialSegments)
            => (Vector3.UnitZ * height).ToLine().Interpolate(verticalSegments).Add(-radius.AlongX()).RevolveAroundAxis(Vector3.UnitZ, radialSegments);

        public static IMesh Cylinder = RevolvedVerticalCylinder(5, 1, 4, 12);

        public static IMesh[] AllGeometries = {
            XYTriangle, // 0
            XYQuad, // 1
            XYQuadFromFunc, // 2
            XYQuad2x2, // 3
            Tetrahedron, // 4
            Torus, // 5
            Cylinder, // 6
            XYTriangleTwice, // 7
        };

        public static double SmallTolerance = 0.0001;

        public static void AssertEquals(IMesh g1, IMesh g2)
        {
            Assert.AreEqual(g1.NumFaces, g2.NumFaces);
            Assert.AreEqual(g1.NumCorners, g2.NumCorners);
            Assert.AreEqual(g1.NumVertices, g2.NumVertices);
            for (int i = 0; i < g1.Indices.Count; i++)
            {
                var v1 = g1.Vertices[g1.Indices[i]];
                var v2 = g2.Vertices[g2.Indices[i]];
                Assert.IsTrue(v1.AlmostEquals(v2, (float)SmallTolerance));
            }
        }

        public static void AssertEqualsWithAttributes(IMesh g1, IMesh g2)
        {
            AssertEquals(g1, g2);

            Assert.AreEqual(
                g1.Attributes.Select(attr => attr.Name).ToArray(),
                g2.Attributes.Select(attr => attr.Name).ToArray());

            Assert.AreEqual(
                g1.Attributes.Select(attr => attr.ElementCount).ToArray(),
                g2.Attributes.Select(attr => attr.ElementCount).ToArray());
        }

        public static void OutputTriangleStats(Triangle t)
        {
            Console.WriteLine($"Vertices: {t.A} {t.B} {t.C}");
            Console.WriteLine($"Area: {t.Area} Perimeter: {t.Perimeter} Midpoint: {t.MidPoint}");
            Console.WriteLine($"Bounding box: {t.BoundingBox}");
            Console.WriteLine($"Bounding sphere: {t.BoundingSphere}");
            Console.WriteLine($"Normal: {t.Normal}, normal direction {t.NormalDirection}");
            Console.WriteLine($"Lengths: {t.LengthA} {t.LengthB} {t.LengthC}");
        }

        public static void OutputTriangleStatsSummary(IMesh g)
        {
            var triangles = g.Triangles();
            for (var i = 0; i < Math.Min(3, triangles.Count); ++i)
            {
                Console.WriteLine($"Triangle {i}");
                OutputTriangleStats(triangles[i]);
            }

            if (triangles.Count > 3)
            {
                Console.WriteLine("...");
                Console.WriteLine($"Triangle {triangles.Count - 1}");
                OutputTriangleStats(triangles.Last());
            }
        }

        public static void OutputIMeshStats(IMesh g)
        {
            ValidateGeometry(g);
            foreach (var attr in g.Attributes.ToEnumerable())
                Console.WriteLine($"{attr.Descriptor} elementCount={attr.ElementCount}");
        }

        public static void GeometryNullOps(IMesh g)
        {
            AssertEqualsWithAttributes(g, g);
            AssertEqualsWithAttributes(g, g.Attributes.ToIMesh());
            AssertEqualsWithAttributes(g, g.Translate(Vector3.Zero));
            AssertEqualsWithAttributes(g, g.Scale(1.0f));
            AssertEqualsWithAttributes(g, g.Transform(Matrix4x4.Identity));

            AssertEquals(g, g.CopyFaces(0, g.NumFaces).ToIMesh());
        }


        [Test]
        public static void BasicTests()
        {
            var nMesh = 0;
            foreach (var g in AllGeometries)
            {
                Console.WriteLine($"Testing mesh {nMesh++}");
                ValidateGeometry(g);
                //ValidateGeometry(g.ToTriMesh());
            }

            Assert.AreEqual(3, XYTriangle.NumCornersPerFace);
            Assert.AreEqual(1, XYTriangle.NumFaces);
            Assert.AreEqual(3, XYTriangle.Vertices.Count);
            Assert.AreEqual(3, XYTriangle.Indices.Count);
            Assert.AreEqual(1, XYTriangle.Triangles().Count);
            Assert.AreEqual(0.5, XYTriangle.Area(), SmallTolerance);
            Assert.IsTrue(XYTriangle.Planar());
            Assert.AreEqual(new[] { 0, 1, 2 }, XYTriangle.Indices.ToArray());

            Assert.AreEqual(3, XYQuad.NumCornersPerFace);
            Assert.AreEqual(2, XYQuad.NumFaces);
            Assert.AreEqual(4, XYQuad.Vertices.Count);
            Assert.AreEqual(6, XYQuad.Indices.Count);

            Assert.IsTrue(XYQuad.Planar());
            Assert.AreEqual(new[] { 0, 1, 2, 0, 2, 3 }, XYQuad.Indices.ToArray());

            Assert.AreEqual(3, XYQuadFromFunc.NumCornersPerFace);
            Assert.AreEqual(2, XYQuadFromFunc.NumFaces);
            Assert.AreEqual(4, XYQuadFromFunc.Vertices.Count);
            Assert.AreEqual(6, XYQuadFromFunc.Indices.Count);

            Assert.AreEqual(3, XYQuad2x2.NumCornersPerFace);
            Assert.AreEqual(8, XYQuad2x2.NumFaces);
            Assert.AreEqual(9, XYQuad2x2.Vertices.Count);
            Assert.AreEqual(24, XYQuad2x2.Indices.Count);

            Assert.AreEqual(3, Tetrahedron.NumCornersPerFace);
            Assert.AreEqual(4, Tetrahedron.NumFaces);
            Assert.AreEqual(4, Tetrahedron.Vertices.Count);
            Assert.AreEqual(12, Tetrahedron.Indices.Count);
            Assert.AreEqual(TestTetrahedronIndices, Tetrahedron.Indices.ToArray());

            Assert.AreEqual(3, XYTriangleTwice.NumCornersPerFace);
            Assert.AreEqual(2, XYTriangleTwice.NumFaces);
            Assert.AreEqual(6, XYTriangleTwice.Vertices.Count);
            Assert.AreEqual(6, XYTriangleTwice.Indices.Count);
            Assert.AreEqual(2, XYTriangleTwice.Triangles().Count);
            Assert.AreEqual(1.0, XYTriangleTwice.Area(), SmallTolerance);
            Assert.IsTrue(XYTriangleTwice.Planar());
            Assert.AreEqual(new[] { 0, 1, 2, 3, 4, 5 }, XYTriangleTwice.Indices.ToArray());
        }

        [Test]
        public static void BasicManipulationTests()
        {
            foreach (var g in AllGeometries)
                GeometryNullOps(g);
        }

        public static void ValidateGeometry(IMesh g)
        {
            Assert.IsTrue(g.Indices.All(i => i >= 0 && i < g.NumVertices));
        }

        [Test]
        public static void OutputGeometryData()
        {
            var n = 0;
            foreach (var g in AllGeometries)
            {
                Console.WriteLine($"Geometry {n++}");
                for (var i = 0; i < g.Vertices.Count && i < 10; ++i)
                {
                    Console.WriteLine($"Vertex {i} {g.Vertices[i]}");
                }

                if (g.Vertices.Count > 10)
                {
                    var last = g.Vertices.Count - 1;
                    Console.WriteLine("...");
                    Console.WriteLine($"Vertex {last} {g.Vertices[last]}");
                }

                for (var i = 0; i < g.NumFaces && i < 10; ++i)
                {
                    Console.WriteLine($"Face {i}: {g.Triangle(i)}");
                }

                if (g.Vertices.Count > 10)
                {
                    var last = g.NumFaces - 1;
                    Console.WriteLine("...");
                    Console.WriteLine($"Face {last}: {g.Triangle(last)}");
                }
            }
        }

        [Test]
        public static void TriangleSerializationTest()
        {
            // Serialize a triangle g3d to a bfast as bytes and read it back.
            var vertices = new[]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 1)
            };

            var indices = new[] { 0, 1, 2 };
            var materialIds = new[] { 0 };
            var faceGroupIds = new[] { 0 };

            var g3d = new G3DBuilder()
                .AddVertices(vertices.ToIArray())
                .AddIndices(indices.ToIArray())
                .Add(faceGroupIds.ToIArray().ToFaceGroupAttribute())
                .Add(materialIds.ToIArray().ToFaceMaterialIdAttribute())
                .ToG3D();

            var bfastBytes = g3d.WriteToBytes();
            var readG3d = G3D.Read(bfastBytes);

            Assert.IsNotNull(readG3d);
            var mesh = readG3d.ToIMesh();
            ValidateGeometry(mesh);

            Assert.AreEqual(3, mesh.NumVertices);
            Assert.AreEqual(new Vector3(0, 0, 0), mesh.Vertices[0]);
            Assert.AreEqual(new Vector3(0, 1, 0), mesh.Vertices[1]);
            Assert.AreEqual(new Vector3(0, 1, 1), mesh.Vertices[2]);
            Assert.AreEqual(1, mesh.NumFaces);
            Assert.AreEqual(0, mesh.FaceGroups.First());
            Assert.AreEqual(0, mesh.FaceMaterialIds.First());
        }


        // TODO: reinstate the CatmullClark algorithm
        /*
        [Test]
        public static void CatmullClark()
        {
            var geometry = G3DExtensions.ToG3D(4,
                new[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 1, 0),
                }.ToVertexAttribute(),
                new[] { 0, 1, 3, 2 }.ToIndexAttribute(),
                new[] { 0 }.ToIArray().ToGroupMaterialIdsAttribute(),
                new[] { 0 }.ToIArray().ToGroupIndexOffsetAttribute(),
                new[] { 0 }.ToIArray().ToGroupVertexOffsetAttribute()
            ).ToIMesh();

            var newGeometry = geometry.CatmullClark(1.0f);

            var testIndices = new int[] { 4, 5, 1, 6, 4, 6, 3, 7, 4, 7, 2, 8, 4, 8, 0, 5 };
            var testVertices = new Vector3[] {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(0.5f, 0.5f, 0),
                new Vector3(0.5f, 0, 0),
                new Vector3(1, 0.5f, 0),
                new Vector3(0.5f, 1, 0),
                new Vector3(0, 0.5f, 0)
            };

            for (int i = 0; i < newGeometry.Indices.Count; i++)
            {
                Assert.IsTrue(newGeometry.Indices[i] == testIndices[i]);
            }

            Console.WriteLine();

            for (int i = 0; i < newGeometry.Vertices.Count; i++)
            {
                Assert.IsTrue(newGeometry.Vertices[i] == testVertices[i]);
            }
        }
        */
    }
}
