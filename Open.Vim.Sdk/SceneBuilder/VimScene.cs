using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vim.DataFormat;
using Vim.DotNetUtilities;
using Vim.DotNetUtilities.Logging;
using Vim.G3d;
using Vim.Geometry;
using Vim.LinqArray;
using Vim.Math3d;
using Vim.ObjectModel;

namespace Vim
{
    // TODO: add property cache lookup

    /// <summary>
    /// This is the top-level class of a loaded VIM file.
    /// </summary>
    public class VimScene : IScene
    {
        public static VimScene LoadVim(string f, ICancelableProgressLogger progress = null, bool skipGeometry = false, bool skipAssets = false)
            => new VimScene(Serializer.Deserialize(f,
                new LoadOptions { SkipGeometry = skipGeometry, SkipAssets = skipAssets }),
                progress);

        public static VimScene LoadVim(Stream stream, ICancelableProgressLogger progress = null, bool skipGeometry = false, bool skipAssets = false)
            => new VimScene(Serializer.Deserialize(stream,
                new LoadOptions { SkipGeometry = skipGeometry, SkipAssets = skipAssets }),
                progress);

        public IArray<IMesh> Geometries { get; private set; }
        public IArray<ISceneNode> Nodes { get; private set; }
        public IArray<VimSceneNode> VimNodes { get; private set; }

        public SerializableDocument _SerializableDocument { get; }
        public Document Document { get; private set; }
        public DocumentModel Model { get; private set; }

        // TODO: check if element ids are unique
        public Dictionary<int, int> ElementIndexFromElementId { get; private set; } = new Dictionary<int, int>();

        // Materials store ids, not indices in the geometry
        public Dictionary<int, int> MaterialIndexFromMaterialId { get; private set; } = new Dictionary<int, int>();

        // There is one of these for each entity that has an element (derives from EntityWithElement).
        // TODO: It is assumed that there is a one to one mapping. But there isn't
        public Dictionary<int, int> FamilyInstanceIndexFromElementIndex { get; private set; } = new Dictionary<int, int>();
        public Dictionary<int, int> FamilyTypeIndexFromElementIndex { get; private set; } = new Dictionary<int, int>();
        public Dictionary<int, int> FamilyIndexFromElementIndex { get; private set; } = new Dictionary<int, int>();
        public Dictionary<int, int> ViewIndexFromElementIndex { get; private set; } = new Dictionary<int, int>();
        public Dictionary<int, int> AssemblyIndexFromElementIndex { get; private set; } = new Dictionary<int, int>();
        public Dictionary<int, int> DesignOptionIndexFromElementIndex { get; private set; } = new Dictionary<int, int>();
        public Dictionary<int, int> LevelIndexFromElementIndex { get; private set; } = new Dictionary<int, int>();
        public Dictionary<int, int> PhaseIndexFromElementIndex { get; private set; } = new Dictionary<int, int>();
        public Dictionary<int, int> RoomIndexFromElementIndex { get; private set; } = new Dictionary<int, int>();

        public DictionaryOfLists<int, int> NodeIndexesFromElementIndex { get; private set; } = new DictionaryOfLists<int, int>();
        public DictionaryOfLists<int, int> NodeIndexesromGeometryIndex { get; private set; } = new DictionaryOfLists<int, int>();

        public DictionaryOfLists<int, int> FamilyTypeIndexesFromRoomIndexes { get; private set; } = new DictionaryOfLists<int, int>();
        public DictionaryOfLists<int, int> FamilyTypeIndexesFromFamilyIndexes { get; private set; } = new DictionaryOfLists<int, int>();
        public DictionaryOfLists<int, int> FamilyInstanceIndexesFromFamilyTypeIndexes { get; private set; } = new DictionaryOfLists<int, int>();

        public DictionaryOfLists<int, int> ElementIndexesFromLevelIndex { get; private set; } = new DictionaryOfLists<int, int>();
        public DictionaryOfLists<int, int> ElementIndexesFromCategoryIndex { get; private set; } = new DictionaryOfLists<int, int>();

        // TODO: consider material lookups? Face lookups?
        public DictionaryOfLists<int, int> MaterialIndexesFromGeometryIndex { get; private set; } = new DictionaryOfLists<int, int>();

        public int ElementIndexFromId(int id)
            => ElementIndexFromElementId.GetOrDefault(id, -1);

        public Element ElementFromId(int id)
            => Model.ElementList.ElementAtOrDefault(ElementIndexFromId(id));

        public Material MaterialFromId(int id)
            => Model.MaterialList.ElementAtOrDefault(MaterialIndexFromMaterialId.GetOrDefault(id, -1));

        public FamilyInstance FamilyInstanceFromId(int id)
            => Model.FamilyInstanceList.ElementAtOrDefault(FamilyInstanceIndexFromElementIndex.GetOrDefault(ElementIndexFromId(id), -1));

        public IEnumerable<Material> MaterialsFromGeometry(IMesh m)
            => m.FaceMaterialIds.ToEnumerable().Distinct().Select(MaterialFromId).WhereNotNull();

        public ISceneNode NodeFromId(int id)
            => Nodes.Where(n => n.Id == id).FirstOrDefault();

        public static Dictionary<int, int> CreateEntityIdToIndexLookup(EntityTable et)
        {
            var r = new Dictionary<int, int>();
            if (et == null)
                return r;
            var ids = et.NumericColumns["Id"].GetTypedData();
            for (var i = 0; i < ids.Length; ++i)
                r.TryAdd((int)ids[i], i);
            return r;
        }

        public static Dictionary<int, int> CreateEntityIndexToIndexLookup(EntityTable et)
        {
            var r = new Dictionary<int, int>();
            var ids = et?.IndexColumns["Element"]?.GetTypedData();
            if (ids == null)
                return r;
            for (var i = 0; i < ids.Length; ++i)
                r.TryAdd((int)ids[i], i);
            return r;
        }

        public static IMesh ToIMesh(G3dSubGeometry g3d)
            => Primitives.TriMesh(
                g3d.Vertices.ToPositionAttribute(),
                g3d.Indices.ToIndexAttribute(),
                g3d.VertexUvs?.ToVertexUvAttribute(),
                g3d.FaceMaterialIds?.ToFaceMaterialIdAttribute(),
                g3d.FaceGroups?.ToFaceGroupAttribute());

        public VimScene(SerializableDocument doc, ICancelableProgressLogger progress = null, bool inParallel = false)
        {
            progress?.LogProgress($"Creating scene from {doc.FileName}", 0.0);
            _SerializableDocument = doc;

            var createPropertyLookupActions = new Action[]
            {
                () => ElementIndexFromElementId = CreateEntityIdToIndexLookup(Model.ElementEntityTable),
                () => MaterialIndexFromMaterialId = CreateEntityIdToIndexLookup(Model.MaterialEntityTable),
                () => FamilyInstanceIndexFromElementIndex = CreateEntityIndexToIndexLookup(Model.FamilyInstanceEntityTable),
                () => FamilyTypeIndexFromElementIndex = CreateEntityIndexToIndexLookup(Model.FamilyTypeEntityTable),
                () => FamilyIndexFromElementIndex = CreateEntityIndexToIndexLookup(Model.FamilyEntityTable),
                () => ViewIndexFromElementIndex = CreateEntityIndexToIndexLookup(Model.ViewEntityTable),
                () => AssemblyIndexFromElementIndex = CreateEntityIndexToIndexLookup(Model.AssemblyInstanceEntityTable),
                () => DesignOptionIndexFromElementIndex = CreateEntityIndexToIndexLookup(Model.DesignOptionEntityTable),
                () => LevelIndexFromElementIndex = CreateEntityIndexToIndexLookup(Model.LevelEntityTable),
                () => PhaseIndexFromElementIndex = CreateEntityIndexToIndexLookup(Model.PhaseEntityTable),
                () => RoomIndexFromElementIndex = CreateEntityIndexToIndexLookup(Model.RoomEntityTable),
                // TODO: I think there are more entity tables missing
            };

            if (inParallel)
            {
                Parallel.Invoke(
                    () =>
                    {
                        progress?.LogProgress("Creating Document", 0.1);
                        Document = _SerializableDocument.ToDocument();

                        progress?.LogProgress("Creating Model", 0.3);
                        Model = new DocumentModel(Document);

                        progress?.LogProgress("Creating lookups", 0.7);
                        Parallel.Invoke(createPropertyLookupActions);
                    },
                    () =>
                    {
                        progress?.LogProgress("Unpacking geometries", 0.1);
                        var srcGeo = _SerializableDocument.Geometry;
                        Geometries = srcGeo?.SubGeometries.Select(ToIMesh).Evaluate() ?? ((IMesh)null).Repeat(0);

                        progress?.LogProgress("Creating scene", 0.8);
                        VimNodes = GetExpandedVimSceneNodes(this, _SerializableDocument.Nodes, _SerializableDocument.GetExpansionMode()).ToIArray();
                        Nodes = VimNodes.Select(n => n as ISceneNode);
                    });
            }
            else
            {
                progress?.LogProgress("Creating IDocument", 0);
                Document = _SerializableDocument.ToDocument();

                progress?.LogProgress("Creating Model", 0.1);
                Model = new DocumentModel(Document);

                progress?.LogProgress("Unpacking geometries", 0.2);
                var srcGeo = _SerializableDocument.Geometry;
                Geometries = srcGeo?.SubGeometries.Select(ToIMesh).Evaluate() ?? ((IMesh)null).Repeat(0);

                progress?.LogProgress("Creating scene", 0.3);
                VimNodes = GetExpandedVimSceneNodes(this, _SerializableDocument.Nodes, _SerializableDocument.GetExpansionMode()).ToIArray();
                Nodes = VimNodes.Select(n => n as ISceneNode);

                progress?.LogProgress("Creating element lookup", 0.5);
                ElementIndexFromElementId = CreateEntityIdToIndexLookup(Model.ElementEntityTable);

                progress?.LogProgress("Creating property lookup", 0.6);
                foreach (var action in createPropertyLookupActions)
                {
                    action.Invoke();
                }
            }

            progress?.LogProgress("Completed creating scene", 1.0);
        }

        private static void CollectVimSceneNodesFromWorldSpaceGeometry(
            VimScene scene,
            IReadOnlyList<Tree<SerializableSceneNode>> flattenedTreeNodes,
            IEnumerable<Tree<SerializableSceneNode>> treeNodesToProcess,
            Matrix4x4 parentBasis,
            List<VimSceneNode> result)
        {
            // For each instance, check if the source node has children. If so, expand those children locally.
            foreach (var treeNode in treeNodesToProcess)
            {
                var node = treeNode.Value;
                if (node.Instance >= 0)
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
                    result.Add(new VimSceneNode(scene, node, result.Count, localBasis));

                    // Expand the source node's children, if any.
                    if (sourceTreeNode.Children.Count > 0)
                    {
                        var descendents = sourceTreeNode.AllNodes().Where(tn => tn != sourceTreeNode);
                        CollectVimSceneNodesFromWorldSpaceGeometry(scene, flattenedTreeNodes, descendents, localBasis, result);
                    }
                }
                else
                {
                    // Non-instances are presumed to have their geometry baked in world space.
                    result.Add(new VimSceneNode(scene, node, result.Count, parentBasis));
                }
            }
        }

        private static void CollectVimSceneNodesFromLocalSpaceGeometry(
            VimScene scene,
            IReadOnlyList<Tree<SerializableSceneNode>> flattenedTreeNodes,
            IEnumerable<Tree<SerializableSceneNode>> treeNodesToProcess,
            Matrix4x4 parentBasis,
            List<VimSceneNode> result)
        {
            // For each instance, check if the source node has children. If so, expand those children locally.
            foreach (var treeNode in treeNodesToProcess)
            {
                var node = treeNode.Value;
                if (node.Instance >= 0)
                {
                    var sourceTreeNode = flattenedTreeNodes[node.Instance];
                    var sourceNode = sourceTreeNode.Value;
                    var instanceBasis = node.Transform * parentBasis;

                    // Add the source node world space info.
                    result.Add(new VimSceneNode(scene, node, result.Count, instanceBasis));

                    // Expand the source node's children (if any) into the local basis.
                    if (sourceTreeNode.Children.Count > 0)
                    {
                        var descendents = sourceTreeNode.AllNodes().Where(tn => tn != sourceTreeNode);

                        // Invert the source node's basis to transform that node hierarchy to the origin, then transform it to the instance basis.
                        if (Matrix4x4.Invert(sourceNode.Transform, out var sourceNodeInverse))
                        {
                            var instanceChildrenBasis = sourceNodeInverse * instanceBasis;
                            CollectVimSceneNodesFromLocalSpaceGeometry(scene, flattenedTreeNodes, descendents, instanceChildrenBasis, result);
                        }
                        else
                        {
                            Debug.WriteLine("Failed to invert node");
                        }
                    }
                }
                else
                {
                    result.Add(new VimSceneNode(scene, node, result.Count, node.Transform * parentBasis));
                }
            }
        }

        // TODO: TECH DEBT - A DUPLICATE OF SerializableSceneNodeExtensions >> GetExpandedGeometryIdsAndTransforms
        public static List<VimSceneNode> GetExpandedVimSceneNodes(
            VimScene scene,
            IReadOnlyList<SerializableSceneNode> nodes,
            ExpansionMode expansionMode)
        {
            // Start with a flat collection of tree nodes.
            var treeNodes = nodes.Select(n => new Tree<SerializableSceneNode> { Value = n }).ToArray();

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

            var result = new List<VimSceneNode>();
            switch (expansionMode)
            {
                case ExpansionMode.WorldSpaceGeometry:
                    // DEPRECATED: This is a legacy stopgap to allow support for objectmodel version 2.
                    // objectmodel version 3 and onward expects local space geometry.
                    CollectVimSceneNodesFromWorldSpaceGeometry(scene, treeNodes, treeNodes, Matrix4x4.Identity, result);
                    break;

                case ExpansionMode.LocalGeometry:
                    CollectVimSceneNodesFromLocalSpaceGeometry(scene, treeNodes, treeNodes, Matrix4x4.Identity, result);
                    break;
            }

            return result;
        }

        public static int FaceModelIndexFromNode(ISceneNode node)
            => node?.GetGeometry()?.FaceGroups?.ElementAtOrDefault(0, -1) ?? -1;

        public Face FaceModelFromNode(ISceneNode node)
        {
            var n = FaceModelIndexFromNode(node);
            if (n < 0) return null;
            return Model.FaceList?[n];
        }

        public FamilyInstance GetFamilyInstance(Element e)
            => FamilyInstanceFromId(e?.Id ?? -1);

        public string GetFamilyName(Element e)
            => GetFamilyInstance(e)?.GetFamily()?.Element?.Name ?? e?.FamilyName ?? "";

        public void Save(string filePath)
            => _SerializableDocument.Serialize(filePath);

        public Dictionary<int, VimSceneNode> GetElementIndexToSceneNodeMap()
        {
            var d = new Dictionary<int, VimSceneNode>();
            for (var i=0; i < VimNodes.Count; ++i)
            {
                var n = VimNodes[i];
                if (!d.ContainsKey(n.ElementIndex))
                    d.Add(n.ElementIndex, n);
            }
            return d;
        }

        public DictionaryOfLists<IMesh, VimSceneNode> GroupNodesByGeometry()
            => VimNodesWithGeometry().ToDictionaryOfLists(n => n.GetGeometry());

        public IEnumerable<VimSceneNode> VimNodesWithGeometry()
            => VimNodes.ToEnumerable().Where(n => n.GetGeometry() != null);

        public string FileName => _SerializableDocument.FileName;

        public void TransformSceneInPlace(Func<IMesh, IMesh> meshTransform = null, Func<VimSceneNode, VimSceneNode> nodeTransform = null)
        {
            if (meshTransform != null)
                Geometries = Geometries.Select(meshTransform).EvaluateInParallel();
            if (nodeTransform != null)
                VimNodes = VimNodes.Select(nodeTransform).EvaluateInParallel();
        }
    }
}
