using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vim.DotNetUtilities;
using Vim.G3d;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.Geometry
{
    public static class MeshExtensions
    {
        #region constructors
        public static IMesh ToIMesh(this IArray<GeometryAttribute> self)
            => self.ToEnumerable().ToIMesh();

        public static IMesh ToIMesh(this IEnumerable<GeometryAttribute> self)
        {
            var attrs = self.Where(x => x != null).ToArray();
            var tmp = new GeometryAttributes(self);
            switch (tmp.NumCornersPerFace)
            {
                case 3:
                    return new TriMesh(tmp.Attributes.ToEnumerable());
                case 4:
                    return new QuadMesh(tmp.Attributes.ToEnumerable()).ToTriMesh();
                default:
                    throw new Exception($"Can not convert a geometry with {tmp.NumCornersPerFace} to a triangle mesh: only quad meshes");
            }
        }

        public static IMesh ToIMesh(this IGeometryAttributes g)
            => g is IMesh m ? m : g is QuadMesh q ? q.ToIMesh() : g.Attributes.ToIMesh();
        #endregion

        // Computes the topology: this is a slow O(N) operation
        public static Topology ComputeTopology(this IMesh g)
            => new Topology(g);

        public static double Area(this IMesh self)
            => self.Triangles().Sum(t => t.Area);

        #region validation
        public static bool IsDegenerateVertexIndices(this Int3 vertexIndices)
            => vertexIndices.X == vertexIndices.Y || vertexIndices.X == vertexIndices.Z || vertexIndices.Y == vertexIndices.Z;

        public static bool HasDegenerateFaceVertexIndices(this IMesh self)
            => self.AllFaceVertexIndices().Any(IsDegenerateVertexIndices);
        #endregion

        // TODO: find a better location for this function. DotNetUtilties doesn't know about IArray unfortunately, so maybe this project needs its own Utility class.
        public static DictionaryOfLists<U, T> GroupBy<T, U>(this IArray<T> xs, Func<int, U> groupingFunc)
        {
            var r = new DictionaryOfLists<U, T>();
            for (var i = 0; i < xs.Count; ++i)
                r.Add(groupingFunc(i), xs[i]);
            return r;
        }

        public static DictionaryOfLists<int, int> IndicesByMaterialId(this IMesh mesh)
            => mesh.Indices.GroupBy(i => mesh.FaceMaterialIds[i / 3]);

        public static IEnumerable<IGrouping<T, int>> GroupFaces<T>(this IMesh mesh, Func<int, T> grouping)
            => mesh.FaceMaterialIds
                .ZipWithIndex()
                .GroupBy(pair => grouping(pair.value), pair => pair.index);

        public static IEnumerable<IGrouping<int, int>> FacesByMaterialId(this IMesh mesh)
            => mesh.FaceMaterialIds
                .ZipWithIndex()
                .GroupBy(pair => pair.value, pair => pair.index);

        public static IMesh Merge(this IArray<IMesh> meshes)
            => meshes.Select(m => (IGeometryAttributes)m).Merge().ToIMesh();

        public static IMesh Merge(this IEnumerable<IMesh> geometries)
            => geometries.ToIArray().Merge();

        public static IMesh Merge(this IMesh g, params IMesh[] others)
        {
            var gs = others.ToList();
            gs.Insert(0, g);
            return gs.Merge();
        }

        public static IGeometryAttributes DeleteUnusedVertices(this IMesh g)
        {
            var tmp = new bool[g.Vertices.Count];
            for (var i = 0; i < g.Indices.Count; ++i)
                tmp[g.Indices[i]] = true;

            var remap = new List<int>();
            for (var i = 0; i < tmp.Length; ++i)
                if (tmp[i])
                    remap.Add(i);

            return g.RemapVertices(remap.ToIArray());
        }

        public static bool GeometryEquals(this IMesh g1, IMesh g2, float tolerance = Constants.Tolerance)
        {
            if (g1.NumFaces != g2.NumFaces)
                return false;
            return g1.Triangles().Zip(g2.Triangles(), (t1, t2) => t1.AlmostEquals(t2, tolerance)).All(x => x);
        }

        public static IMesh SimplePolygonTessellate(this IEnumerable<Vector3> points)
        {
            var pts = points.ToList();
            var cnt = pts.Count;
            var sum = Vector3.Zero;
            var idxs = new List<int>(pts.Count * 3);
            for (var i = 0; i < pts.Count; ++i) 
            {
                idxs.Add(i);
                idxs.Add(i + 1 % cnt);
                idxs.Add(cnt);
                sum += pts[i];
            }

            var midPoint = sum / pts.Count;
            pts.Add(midPoint);

            return Primitives.TriMesh(pts.ToIArray(), idxs.ToIArray());
        }

        public static IGeometryAttributes ReverseWindingOrder(this IMesh self)
        {
            var n = self.Indices.Count;
            var r = new int[n];
            for (var i=0; i < n; i += 3)
            {
                r[i + 0] = self.Indices[i + 2];
                r[i + 1] = self.Indices[i + 1];
                r[i + 2] = self.Indices[i + 0];
            }
            return self.SetAttribute(r.ToIArray().ToIndexAttribute());
        }

        /// <summary>
        /// Returns the closest point in a sequence of points
        /// </summary>
        public static Vector3 NearestPoint(this IEnumerable<Vector3> points, Vector3 x)
            => points.Minimize(float.MaxValue, p => p.DistanceSquared(x));

        /// <summary>
        /// Returns the closest point in a sequence of points
        /// </summary>
        public static Vector3 NearestPoint(this IArray<Vector3> points, Vector3 x)
            => points.ToEnumerable().NearestPoint(x);

        /// <summary>
        /// Returns the closest point in a geometry
        /// </summary>
        public static Vector3 NearestPoint(this IMesh g, Vector3 x)
            => g.Vertices.NearestPoint(x);

        public static Vector3 FurthestPoint(this IMesh g, Vector3 x0, Vector3 x1)
            => g.Vertices.FurthestPoint(x0, x1);

        public static Vector3 FurthestPoint(this IArray<Vector3> points, Vector3 x0, Vector3 x1)
            => points.ToEnumerable().FurthestPoint(x0, x1);

        public static Vector3 FurthestPoint(this IEnumerable<Vector3> points, Vector3 x0, Vector3 x1)
            => points.Maximize(float.MinValue, v => v.Distance(x0).Min(v.Distance(x1)));

        public static Vector3 FurthestPoint(this IMesh g, Vector3 x)
            => g.Vertices.FurthestPoint(x);

        public static Vector3 FurthestPoint(this IArray<Vector3> points, Vector3 x)
            => points.ToEnumerable().FurthestPoint(x);

        public static Vector3 FurthestPoint(this IEnumerable<Vector3> points, Vector3 x)
            => points.Maximize(float.MinValue, v => v.Distance(x));

        public static IGeometryAttributes SnapPoints(this IMesh g, float snapSize)
            => snapSize.Abs() >= Constants.Tolerance
                ? g.Deform(v => (v * snapSize.Inverse()).Truncate() * snapSize)
                : g.Deform(v => Vector3.Zero);

        /// <summary>
        /// Returns the vertices organized by face corner. 
        /// </summary>
        public static IArray<Vector3> VerticesByIndex(this IMesh m)
            => m.Vertices.SelectByIndex(m.Indices);

        /// <summary>
        /// Returns the vertices organized by face corner, normalized to the first position.
        /// This is useful for detecting if two meshes are the same except offset by 
        /// position.
        /// </summary>
        public static IArray<Vector3> NormalizedVerticesByCorner(this IMesh m)
        {
            if (m.NumCorners == 0)
                return Vector3.Zero.Repeat(0);
            var firstVertex = m.Vertices[m.Indices[0]];
            return m.VerticesByIndex().Select(v => v - firstVertex);
        }

        /// <summary>
        /// Compares the face positions of two meshes normalized by the vertex buffer, returning the maximum distance, or null
        /// if the meshes have different topology. 
        /// </summary>
        public static float? MaxNormalizedDistance(this IMesh a, IMesh b)
        {
            var xs = a.NormalizedVerticesByCorner();
            var ys = b.NormalizedVerticesByCorner();
            if (xs.Count != ys.Count)
                return null;
            return xs.Zip(ys, (x, y) => x.Distance(y)).Max();
        }

        public static AABox BoundingBox(this IMesh self)
            => AABox.Create(self.Vertices.ToEnumerable());

        public static Sphere BoundingSphere(this IMesh self)
            => self.BoundingBox().ToSphere();

        public static float BoundingRadius(this IMesh self)
            => self.BoundingSphere().Radius;

        public static Vector3 Center(this IMesh self)
            => self.BoundingBox().Center;

        public static Vector3 Centroid(this IMesh self)
            => self.Vertices.Aggregate(Vector3.Zero, (x, y) => x + y) / self.Vertices.Count;

        public static bool AreIndicesValid(this IMesh g)
            => g.Indices.All(i => i >= 0 && i < g.Vertices.Count);

        public static bool AreAllVerticesUsed(this IMesh self)
        {
            var used = new bool[self.Vertices.Count];
            self.Indices.ForEach(idx => used[idx] = true);
            return used.All(b => b);
        }

        public static IMesh ResetPivot(this IMesh self)
            => self.Translate(-self.BoundingBox().CenterBottom);

        #region Face operations

        /// <summary>
        /// Given an array of face data, converts it to corner data, and calls CornerDataToVertexData
        /// </summary>
        //public static IArray<T> FaceDataToVertexData<T>(this IMesh self, IArray<T> data)
        //    => self.CornerDataToVertexData(self.FaceDataToCornerData(data));

        /// <summary>
        /// Given an array of face data, creates an array of indexed data to match vertices
        /// </summary>
        public static IArray<T> FaceDataToVertexData<T>(this IMesh g, IArray<T> data)
        {
            if (data.Count != g.NumFaces)
                throw new Exception("Cannot match input Face data to existing faces");

            var vertexData = new T[g.NumVertices];
            for (var i = 0; i < g.Indices.Count; ++i)
                vertexData[g.Indices[i]] = data[i / 3];
            return vertexData.ToIArray();
        }

        public static IArray<Int3> AllFaceVertexIndices(this IMesh self)
            => self.NumFaces.Select(self.FaceVertexIndices);

        public static Int3 FaceVertexIndices(this IMesh self, int faceIndex)
            => new Int3(self.Indices[faceIndex * 3], self.Indices[faceIndex * 3 + 1], self.Indices[faceIndex * 3 + 2]);

        public static Triangle VertexIndicesToTriangle(this IMesh self, Int3 indices)
            => new Triangle(self.Vertices[indices.X], self.Vertices[indices.Y], self.Vertices[indices.Z]);

        public static Triangle Triangle(this IMesh self, int face)
            => self.VertexIndicesToTriangle(self.FaceVertexIndices(face));

        public static IArray<Triangle> Triangles(this IMesh self)
            => self.NumFaces.Select(self.Triangle);

        public static IArray<Line> GetAllEdgesAsLines(this IMesh mesh)
            => mesh.Triangles().SelectMany(tri => Tuple.Create(tri.AB, tri.BC, tri.CA));

        public static IArray<Vector3> ComputedNormals(this IMesh self)
            => self.Triangles().Select(t => t.Normal);

        public static bool Planar(this IMesh self, float tolerance = Constants.Tolerance)
        {
            if (self.NumFaces <= 1) return true;
            var normal = self.Triangle(0).Normal;
            return self.ComputedNormals().All(n => n.AlmostEquals(normal, tolerance));
        }

        public static IArray<Vector3> MidPoints(this IMesh self)
            => self.Triangles().Select(t => t.MidPoint);

        public static IArray<int> FacesToCorners(this IMesh self)
            => self.NumFaces.Select(i => i * 3);

        public static IArray<T> FaceDataToCornerData<T>(this IMesh self, IArray<T> data)
            => self.NumCorners.Select(i => data[i / 3]);

        public static IArray<Vector3> GetOrComputeFaceNormals(this IMesh self)
            => self.GetAttributeFaceNormal()?.Data ?? self.ComputedNormals();

        public static IArray<Vector3> GetOrComputeVertexNormals(this IMesh self)
            => self.VertexNormals ?? self.ComputeTopology().GetOrComputeVertexNormals();

        /// <summary>
        /// Returns vertex normals if present, otherwise computes vertex normals naively by averaging them.
        /// Given a pre-computed topology, will-leverage that.
        /// A more sophisticated algorithm would compute the weighted normal 
        /// based on an angle.
        /// </summary>
        public static IArray<Vector3> GetOrComputeVertexNormals(this Topology topo)
        {
            var mesh = topo.Mesh;
            var r = mesh.VertexNormals;
            if (r != null) return r;
            var faceNormals = mesh.GetOrComputeFaceNormals().ToArray();
            return mesh
                .NumVertices
                .Select(vi =>
                {
                    var tmp = topo
                        .FacesFromVertexIndex(vi)
                        .Select(fi => faceNormals[fi])
                        .Average();
                    if (tmp.IsNaN())
                        return Vector3.Zero;
                    return tmp.SafeNormalize();
                });
        }

        public static IMesh CopyFaces(this IMesh self, Func<int, bool> predicate)
            => (self as IGeometryAttributes).CopyFaces(predicate).ToIMesh();

        public static IMesh CopyFaces(this IMesh self, IArray<bool> keep)
            => self.CopyFaces(i => keep[i]);

        public static IMesh CopyFaces(this IMesh self, IArray<int> keep)
            => self.RemapFaces(keep).ToIMesh();

        public static IMesh DeleteFaces(this IMesh self, Func<int, bool> predicate)
            => self.CopyFaces(f => !predicate(f));
        #endregion

        #region Corner extensions
        /// <summary>
        /// Given an array of data associated with corners, return an array of data associated with
        /// vertices. If a vertex is not referenced, no data is returned. If a vertex is referenced
        /// multiple times, the last reference is used.
        /// TODO: supplement with a proper interpolation system.
        /// </summary>
        public static IArray<T> CornerDataToVertexData<T>(this IMesh g, IArray<T> data)
        {
            var vertexData = new T[g.NumVertices];
            for (var i = 0; i < data.Count; ++i)
                vertexData[g.Indices[i]] = data[i];
            return vertexData.ToIArray();
        }
        #endregion
    }
}
