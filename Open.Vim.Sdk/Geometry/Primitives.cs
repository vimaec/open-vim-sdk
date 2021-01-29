using System;
using System.Collections.Generic;
using System.Linq;
using Vim.G3d;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.Geometry
{
    public static class Primitives
    {
        public static IMesh TriMesh(IEnumerable<GeometryAttribute> attributes)
            => attributes.Where(x => x != null).ToIMesh();

        public static IMesh TriMesh(params GeometryAttribute[] attributes)
            => TriMesh(attributes.AsEnumerable());

        public static IMesh TriMesh(this IArray<Vector3> vertices, IArray<int> indices = null, IArray<Vector2> uvs = null, IArray<Vector4> colors = null, IArray<int> materialIds = null, IArray<int> groupdIds = null)
            => TriMesh(
                vertices?.ToPositionAttribute(),
                indices?.ToIndexAttribute(),
                uvs?.ToVertexUvAttribute(),
                materialIds?.ToFaceMaterialIdAttribute(),
                colors?.ToVertexColorAttribute(),
                groupdIds?.ToFaceGroupAttribute());

        public static IMesh TriMesh(this IArray<Vector3> vertices, IArray<int> indices = null, params GeometryAttribute[] attributes)
            => new GeometryAttribute[] {
                vertices?.ToPositionAttribute(),
                indices?.ToIndexAttribute(),
            }.Concat(attributes).ToIMesh();

        public static IMesh QuadMesh(params GeometryAttribute[] attributes)
            => QuadMesh(attributes.AsEnumerable());

        public static IMesh QuadMesh(this IEnumerable<GeometryAttribute> attributes)
            => new QuadMesh(attributes.Where(x => x != null)).ToTriMesh();

        public static IMesh QuadMesh(this IArray<Vector3> vertices, IArray<int> indices = null, IArray<Vector2> uvs = null, IArray<int> materialIds = null, IArray<int> objectIds = null)
            => QuadMesh(
                vertices.ToPositionAttribute(),
                indices?.ToIndexAttribute(),
                uvs?.ToVertexUvAttribute(),
                materialIds?.ToFaceMaterialIdAttribute(),
                objectIds?.ToFaceGroupAttribute()
                );

        public static IMesh QuadMesh(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
            => QuadMesh(new[] { a, b, c, d }.ToIArray());

        public static IMesh Cube
        {
            get
            {
                var vertices = new[] {
                    // front
                    new Vector3(-0.5f, -0.5f,  0.5f),
                    new Vector3(0.5f, -0.5f,  0.5f),
                    new Vector3(0.5f,  0.5f,  0.5f),
                    new Vector3(-0.5f,  0.5f,  0.5f),
                    // back
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f,  0.5f, -0.5f),
                    new Vector3(-0.5f,  0.5f, -0.5f)
                }.ToIArray();

                var indices = new[] {
                    // front
                    0, 1, 2,
                    2, 3, 0,
                    // right
                    1, 5, 6,
                    6, 2, 1,
                    // back
                    7, 6, 5,
                    5, 4, 7,
                    // left
                    4, 0, 3,
                    3, 7, 4,
                    // bottom
                    4, 5, 1,
                    1, 0, 4,
                    // top
                    3, 2, 6,
                    6, 7, 3
                }.ToIArray();

                return vertices.TriMesh(indices);
            }
        }

        public static IMesh ToIMesh(this AABox box)
            => Cube.Scale(box.Extent).Translate(box.Center);

        public static float Sqrt2 = 2.0f.Sqrt();

        public static readonly IMesh Tetrahedron
            = TriMesh(LinqArray.LinqArray.Create(
                new Vector3(1f, 0.0f, -1f / Sqrt2),
                new Vector3(-1f, 0.0f, -1f / Sqrt2),
                new Vector3(0.0f, 1f, 1f / Sqrt2),
                new Vector3(0.0f, -1f, 1f / Sqrt2)),
            LinqArray.LinqArray.Create(0, 1, 2, 1, 0, 3, 0, 2, 3, 1, 3, 2));

        public static readonly IMesh Square
            = LinqArray.LinqArray.Create(
                new Vector2(-0.5f, -0.5f),
                new Vector2(-0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, -0.5f)).Select(x => x.ToVector3()).QuadMesh();

        public static readonly IMesh Octahedron
            = Square.Vertices.Append(Vector3.UnitZ, -Vector3.UnitZ).Normalize().TriMesh(
                LinqArray.LinqArray.Create(
                    0, 1, 4, 1, 2, 4, 2, 3, 4,
                    3, 2, 5, 2, 1, 5, 1, 0, 5));

        // see: https://github.com/mrdoob/three.js/blob/9ef27d1af7809fa4d9943f8d4c4644e365ab6d2d/src/geometries/TorusBufferGeometry.js#L52
        public static Vector3 TorusFunction(Vector2 uv, float radius, float tube)
        {
            uv = uv * Constants.TwoPi;
            return new Vector3(
                (radius + tube * uv.Y.Cos()) * uv.X.Cos(),
                (radius + tube * uv.Y.Cos()) * uv.X.Sin(),
                tube * uv.Y.Sin());
        }

        public static IMesh Torus(float radius, float tubeRadius, int uSegs, int vSegs)
            => QuadMesh(uv => TorusFunction(uv, radius, tubeRadius), uSegs, vSegs);

        // see: https://github.com/mrdoob/three.js/blob/9ef27d1af7809fa4d9943f8d4c4644e365ab6d2d/src/geometries/SphereBufferGeometry.js#L76
        public static Vector3 SphereFunction(Vector2 uv, float radius)
            => new Vector3(
                (float) (-radius * Math.Cos(uv.X * Constants.TwoPi) * Math.Sin(uv.Y * Constants.Pi)),
                (float) (radius * Math.Cos(uv.Y * Constants.Pi)),
                (float) (radius * Math.Sin(uv.X * Constants.TwoPi) * Math.Sin(uv.Y * Constants.Pi)));

        public static IMesh Sphere(float radius, int uSegs, int vSegs)
            => QuadMesh(uv => SphereFunction(uv, radius), uSegs, vSegs);

        /// <summary>
        /// Creates a TriMesh from four points. 
        /// </summary>
        public static IMesh TriMeshFromQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
            => TriMesh(new[] { a, b, c, c, d, a }.ToIArray());

        // Icosahedron, Dodecahedron,

        /// <summary>
        /// Returns a collection of circular points.
        /// </summary>
        public static IArray<Vector2> CirclePoints(float radius, int numPoints)
        {
            var aroundCircle = Constants.TwoPi / numPoints;

            return numPoints.Range().Select(i =>
            {
                var angle = i * aroundCircle;
                return new Vector2(radius * angle.Cos(), radius * angle.Sin());
            });
        }

        /// <summary>
        /// Computes the indices of a quad mesh strip. u is left to right, and v is bottom up. 
        /// </summary>
        public static IArray<int> ComputeQuadMeshStripIndices(int usegs, int vsegs, bool wrapUSegs = false, bool wrapVSegs = false)
        {
            var indices = new List<int>();

            var maxUSegs = wrapUSegs ? usegs : usegs + 1;
            var maxVSegs = wrapVSegs ? vsegs : vsegs + 1;

            for (var i = 0; i < vsegs; ++i)
            {
                var rowA = i * maxUSegs;
                var rowB = ((i + 1) % maxVSegs) * maxUSegs;

                for (var j = 0; j < usegs; ++j)
                {
                    var colA = j;
                    var colB = (j + 1) % maxUSegs;

                    indices.Add(rowA + colA);
                    indices.Add(rowA + colB);
                    indices.Add(rowB + colB);
                    indices.Add(rowB + colA);
                }
            }

            return indices.ToIArray();
        }

        public static IArray<int> TriMeshCylinderCapIndices(int numEdgeVertices)
        {            
            var indices = new List<int>();
            for (var i=0; i < numEdgeVertices - 1; ++i)
            {
                indices.Add(0);
                indices.Add(i + 1);
                indices.Add(i + 2 == numEdgeVertices ? 1 : i + 2);
            }
            return indices.ToIArray();
        }

        /// <summary>
        /// Creates a quad mesh given a mapping from 2 space to 3 space 
        /// </summary>
        public static IMesh QuadMesh(this Func<Vector2, Vector3> f, int segs)
            => QuadMesh(f, segs, segs);

        /// <summary>
        /// Creates a quad mesh given a mapping from 2 space to 3 space 
        /// </summary>
        public static IMesh QuadMesh(this Func<Vector2, Vector3> f, int usegs, int vsegs, bool wrapUSegs = false, bool wrapVSegs = false)
        {
            var verts = new List<Vector3>();
            var maxUSegs = wrapUSegs ? usegs : usegs + 1;
            var maxVSegs = wrapVSegs ? vsegs : vsegs + 1;

            for (var i = 0; i < maxVSegs; ++i)
            {
                var v = (float)i / vsegs;
                for (var j = 0; j < maxUSegs; ++j)
                {
                    var u = (float)j / usegs;
                    verts.Add(f(new Vector2(u, v)));
                }
            }
          
            return QuadMesh(verts.ToIArray(), ComputeQuadMeshStripIndices(usegs, vsegs, wrapUSegs, wrapVSegs));
        }

        /// <summary>
        /// Creates a revolved face ... note that the last points are on top of the original 
        /// </summary>
        public static IMesh RevolveAroundAxis(this IArray<Vector3> points, Vector3 axis, int segments = 4)
        {
            var verts = new List<Vector3>();
            for (var i = 0; i < segments; ++i)
            {
                var angle = Constants.TwoPi / segments;
                points.Rotate(axis, angle).AddTo(verts);
            }

            return QuadMesh(verts.ToIArray(), ComputeQuadMeshStripIndices(segments - 1, points.Count - 1));
        }
    }
}
