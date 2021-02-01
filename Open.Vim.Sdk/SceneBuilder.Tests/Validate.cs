using NUnit.Framework;
using Vim.Geometry;
using Vim.LinqArray;

namespace Vim.SceneBuilder.Tests
{
    public static class Validate
    {
        /// <summary>
        /// Test the two IMesh for equality.  This function ignores differences in
        /// non-mesh attributes.
        /// </summary>
        public static bool MeshAlmostEquals(IMesh left, IMesh right, float tolerance = 1e-04f)
        {
            if (left.Vertices.Count != right.Vertices.Count)
                return false;

            if (!left.Vertices.SequenceAlmostEquals(right.Vertices, tolerance))
                return false;

            if (!left.Indices.SequenceEquals(right.Indices))
                return false;

            // UV's are not affected by transforms, so we can expect them to be identical
            if (!left.VertexUvs.SequenceEquals(right.VertexUvs))
                return false;

            return left.FaceGroups.SequenceEquals(right.FaceGroups);
        }

        public static void Equivalent(VimScene left, VimScene right)
        {
            Assert.AreEqual(left.Nodes.Count, right.Nodes.Count);
            // TODO: Validate that other assets are equivalent (all tables etc)
            // can we cast a DocumentBuilder to a
            for (var i = 0; i < left.Nodes.Count; i++)
            {
                var ln = left.Nodes[i] as VimSceneNode;
                var rn = right.Nodes[i] as VimSceneNode;
                Assert.AreEqual(ln.ElementIndex, rn.ElementIndex);
                // These two meshes should be more-or-less the same
                var lg = ln.TransformedGeometry();
                var rg = rn.TransformedGeometry();
                MeshAlmostEquals(lg, rg);
            }
        }
    }
}
