using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vim.DataFormat;
using Vim.DotNetUtilities;
using Vim.Math3d;

namespace Vim.SceneBuilder
{
    public static class NodeInstanceExpansion
    {
        /// <summary>
        /// Defines a node instance.
        /// </summary>
        public interface INodeInstance<out N>
        {
            N Value { get; }
            int Parent { get; }
            int Instance { get; }
            Matrix4x4 Transform { get; }
            int Geometry { get; }
        }

        /// <summary>
        /// A delegate which defines the function signature for creating a new node during the expansion process.
        /// </summary>
        public delegate T CreateNewNodeFunc<N, out T>(
            N sourceNode,
            int nodeIndex,
            Matrix4x4 nodeTransform,
            int geometryIndex);

        /// <summary>
        /// Returns true if the given node is an instance.
        /// </summary>
        public static bool IsInstance<N>(this INodeInstance<N> node)
            => node.Instance >= 0;

        /// <summary>
        /// [LEGACY - object model 2.0] Collects nodes and expands node instance subtrees. The geometry is assumed to be defined in world space.
        /// </summary>
        private static void CollectNodesFromWorldSpaceGeometry<N,T>(
            IReadOnlyList<Tree<INodeInstance<N>>> flattenedTreeNodes,
            IEnumerable<Tree<INodeInstance<N>>> treeNodesToProcess,
            Matrix4x4 parentBasis,
            CreateNewNodeFunc<N,T> createNewNodeFunc,
            List<T> result)
        {
            // For each instance, check if the source node has children. If so, expand those children locally.
            foreach (var treeNode in treeNodesToProcess)
            {
                var node = treeNode.Value;
                if (node.IsInstance())
                {
                    var sourceTreeNode = flattenedTreeNodes[node.Instance];
                    var sourceNode = sourceTreeNode.Value;

                    // Can't proceed unless the source node transform can be inverted.
                    if (!Matrix4x4.Invert(sourceNode.Transform, out var sourceNodeInverse))
                    {
                        continue;
                    }

                    var localBasis = sourceNodeInverse * node.Transform * parentBasis;

                    // Add the source node world space info.
                    result.Add(createNewNodeFunc(node.Value, result.Count, localBasis, sourceNode.Geometry));

                    // Expand the source node's children, if any.
                    if (sourceTreeNode.Children.Count > 0)
                    {
                        var descendents = sourceTreeNode.AllNodes().Where(tn => tn != sourceTreeNode);
                        CollectNodesFromWorldSpaceGeometry(flattenedTreeNodes, descendents, localBasis, createNewNodeFunc, result);
                    }
                }
                else
                {
                    // Non-instances are presumed to have their geometry baked in world space.
                    result.Add(createNewNodeFunc(node.Value, result.Count, parentBasis, node.Geometry));
                }
            }
        }

        /// <summary>
        /// Collects nodes and expands node instance subtrees. The geometry is assumed to be defined in local space.
        /// </summary>
        private static void CollectNodesFromLocalSpaceGeometry<N,T>(
            IReadOnlyList<Tree<INodeInstance<N>>> flattenedTreeNodes,
            IEnumerable<Tree<INodeInstance<N>>> treeNodesToProcess,
            Matrix4x4 parentBasis,
            CreateNewNodeFunc<N,T> createNewNodeFunc,
            List<T> result)
        {
            // For each instance, check if the source node has children. If so, expand those children locally.
            foreach (var treeNode in treeNodesToProcess)
            {
                var node = treeNode.Value;
                if (node.IsInstance())
                {
                    var sourceTreeNode = flattenedTreeNodes[node.Instance];
                    var sourceNode = sourceTreeNode.Value;
                    var instanceBasis = node.Transform * parentBasis;

                    // Add the source node world space info.
                    result.Add(createNewNodeFunc(node.Value, result.Count, instanceBasis, sourceNode.Geometry));

                    // Expand the source node's children (if any) into the local basis.
                    if (sourceTreeNode.Children.Count > 0)
                    {
                        var descendents = sourceTreeNode.AllNodes().Where(tn => tn != sourceTreeNode);

                        // Invert the source node's basis to transform that node hierarchy to the origin, then transform it to the instance basis.
                        if (Matrix4x4.Invert(sourceNode.Transform, out var sourceNodeInverse))
                        {
                            var instanceChildrenBasis = sourceNodeInverse * instanceBasis;
                            CollectNodesFromLocalSpaceGeometry(flattenedTreeNodes, descendents, instanceChildrenBasis, createNewNodeFunc, result);
                        }
                        else
                        {
                            Debug.WriteLine("Failed to invert node");
                        }
                    }
                }
                else
                {
                    result.Add(createNewNodeFunc(node.Value, result.Count, node.Transform * parentBasis, node.Geometry));
                }
            }
        }

        /// <summary>
        /// Converts the list of nodes (containing instanced subtrees) into a flattened list of new objects.
        /// </summary>
        public static List<T> ExpandNodeInstanceSubtrees<N,T>(
            this IReadOnlyList<INodeInstance<N>> nodes,
            ExpansionMode expansionMode,
            CreateNewNodeFunc<N,T> createNewNodeFunc)
        {
            // Start with a flat collection of tree nodes.
            var treeNodes = nodes.Select(n => new Tree<INodeInstance<N>> { Value = n }).ToArray();

            // Establish the parent/child hierarchy.
            foreach (var treeNode in treeNodes)
            {
                var parentIndex = treeNode.Value.Parent;
                if (parentIndex < 0 || parentIndex >= treeNodes.Length)
                {
                    continue;
                }

                treeNodes[parentIndex].AddChild(treeNode);
            }

            var result = new List<T>();
            switch (expansionMode)
            {
                case ExpansionMode.WorldSpaceGeometry:
                    // DEPRECATED: This is a legacy stopgap to allow support for objectmodel version 2.
                    // objectmodel version 3 and onward expects local space geometry.
                    CollectNodesFromWorldSpaceGeometry(treeNodes, treeNodes, Matrix4x4.Identity, createNewNodeFunc, result);
                    break;

                case ExpansionMode.LocalGeometry:
                    CollectNodesFromLocalSpaceGeometry(treeNodes, treeNodes, Matrix4x4.Identity, createNewNodeFunc, result);
                    break;
            }

            return result;
        }

        /// <summary>
        /// A wrapper for working with SerializableSceneNodes as INodeInstances
        /// </summary>
        public class SerializableSceneNodeInstanceAdapter : INodeInstance<SerializableSceneNode>
        {
            public SerializableSceneNode Value { get; }
            public int Parent => Value.Parent;
            public int Instance => Value.Instance;
            public Matrix4x4 Transform => Value.Transform;
            public int Geometry => Value.Geometry;
            public SerializableSceneNodeInstanceAdapter(SerializableSceneNode node)
                => Value = node;
        }

        /// <summary>
        /// An overload for expanding a list of SerializableSceneNodes.
        /// </summary>
        public static List<T> ExpandNodeInstanceSubtrees<T>(
            this IReadOnlyList<SerializableSceneNode> nodes,
            ExpansionMode expansionMode,
            CreateNewNodeFunc<SerializableSceneNode,T> createNewNodeFunc)
            => ExpandNodeInstanceSubtrees(
                nodes.Select(n => new SerializableSceneNodeInstanceAdapter(n)).ToList(),
                expansionMode,
                createNewNodeFunc);
    }
}
