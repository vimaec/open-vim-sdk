using System;
using System.Collections.Generic;
using System.Linq;
using Vim.Math3d;

namespace Vim.DataFormat
{
    public class GeometryBuilder
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector2> UVs = new List<Vector2>();
        public List<Vector4> Colors = new List<Vector4>();
        public List<int> Indices = new List<int>();
        public List<int> MaterialIds = new List<int>();
        public List<int> FaceGroupIds = new List<int>();
        public int NumFaces => Indices.Count / 3;

        public GeometryBuilder AddVertex(float x, float y, float z)
            => AddVertex(new Vector3(x, y, z));

        public GeometryBuilder AddVertex(Vector3 v)
        {
            Vertices.Add(v);
            return this;
        }

        public GeometryBuilder AddUv(float x, float y)
            => AddUv(new Vector2(x, y));

        public GeometryBuilder AddUv(Vector2 v)
        {
            UVs.Add(v);
            return this;
        }

        public GeometryBuilder AddFace(int a, int b, int c)
        {
            Indices.Add(a);
            Indices.Add(b);
            Indices.Add(c);
            return this;
        }

        public GeometryBuilder AddMaterialId(int id)
        {
            MaterialIds.Add(id);
            return this;
        }

        public GeometryBuilder AddFaceGroupId(int id)
        {
            FaceGroupIds.Add(id);
            return this;
        }

        public GeometryBuilder Finish()
        {
            while (UVs.Count < Vertices.Count)
                AddUv(Vector2.Zero);

            while (MaterialIds.Count < NumFaces)
                AddMaterialId(-1);

            while (FaceGroupIds.Count < NumFaces)
                AddFaceGroupId(-1);

            if (Indices.Count % 3 != 0)
                throw new Exception($"Number of indices {Indices.Count} is not divisible by three");

            if (UVs.Count != Vertices.Count)
                throw new Exception($"Number of Uvs {UVs.Count} is not the same as number of vertices {Vertices.Count}");

            if (MaterialIds.Count != NumFaces)
                throw new Exception($"Number of Material Ids {MaterialIds.Count} is not the same as number of faces {NumFaces}");

            if (FaceGroupIds.Count != NumFaces)
                throw new Exception($"Number of Face group Ids {FaceGroupIds.Count} is not the same as number of faces {NumFaces}");

            if (Indices.Any(i => i < 0 || i >= Vertices.Count))
                throw new Exception($"Not all indices are in the range of [0 to {Vertices.Count - 1}]");
            return this;
        }

        public bool IsEquivalentTo(GeometryBuilder other)
            => Vertices.SequenceEqual(other.Vertices)
               && UVs.SequenceEqual(other.UVs)
               && Colors.SequenceEqual(other.Colors)
               && Indices.SequenceEqual(other.Indices)
               && MaterialIds.SequenceEqual(other.MaterialIds)
               && FaceGroupIds.SequenceEqual(other.FaceGroupIds);
    }
}
