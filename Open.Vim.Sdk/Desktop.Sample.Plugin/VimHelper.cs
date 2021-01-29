using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vim.Desktop.Api;
using Vim.DotNetUtilities;
using Vim.LinqArray;
using Vim.ObjectModel;

namespace Vim.Explorer.Plugin
{
    public class VimHelper
    {
        public VimScene Vim { get; }
        public IRenderApi Api { get; }

        public Dictionary<int, int> NodeIndexToInstanceIndex { get; }
            = new Dictionary<int, int>();

        public Dictionary<IntPtr, int> InstanceToNodeIndex { get; }
            = new Dictionary<IntPtr, int>();

        public Dictionary<int, VimSceneNode> ElementIndexToSceneNode { get; }
            = new Dictionary<int, VimSceneNode>();

        /// <summary>
        /// Used to indicate "hidden" or "highlighting"
        /// </summary>
        public bool[] Flags;

        /// <summary>
        /// Indices of nodes, that are actually used by the rendering system 
        /// </summary>
        public int[] NodeIndices;

        /// <summary>
        /// InstancePtrs arranged how the renderer likes them arranged.
        /// Does not have a one-to-one corrspondence to int ptrs.
        /// </summary>
        public IntPtr[] Instances;

        public Element GetElementFromId(int id)
            => Vim.ElementFromId(id);

        public Element GetElementFromInstance(IntPtr p)
            => GetVimNodeFromInstance(p)?.Element;

        public VimSceneNode GetVimNodeFromInstance(IntPtr p)
        {
            var index = InstanceToNodeIndex.GetOrDefault(p, -1);
            if (index <= 0) return null;
            return Vim.VimNodes[index];
        }

        public VimSceneNode GetVimNodeFromId(int id)
            => ElementIndexToSceneNode.GetOrDefault(Vim.ElementIndexFromId(id));

        public IntPtr GetInstanceFromNodeIndex(int index)
        {
            var instanceIndex = NodeIndexToInstanceIndex.GetOrDefault(index, -1);
            if (instanceIndex <= 0)
                return IntPtr.Zero;
            return Instances[instanceIndex];
        }
 
        public VimHelper(IRenderApi api, VimScene vim)
        {
            (Vim, Api) = (vim, api);
            Api.Scene.GetNodeToInstanceTable(ref NodeIndices, ref Instances);
            Debug.Assert(NodeIndices.Length == Instances.Length);
            Flags = new bool[NodeIndices.Length];
            for (var i = 0; i < NodeIndices.Length; ++i)
            {
                NodeIndexToInstanceIndex.Add(NodeIndices[i], i);
                InstanceToNodeIndex.Add(Instances[i], NodeIndices[i]);
            }

            ElementIndexToSceneNode = Vim.GetElementIndexToSceneNodeMap();
        }

        public void IsolateNodes(IEnumerable<VimSceneNode> nodes)
        {
            for (var i = 0; i < Flags.Length; ++i)
                Flags[i] = true;
            foreach (var n in nodes) { 
                var index = NodeIndexToInstanceIndex.GetOrDefault(n.Id, -1);
                if (index >= 0)
                    Flags[index] = false;
            }
            UpdateVisibility();
        }

        public void ShowNodes(Func<VimSceneNode, bool> f)
            => IsolateNodes(Vim.VimNodes.Where(f));

        public void HighlightNodes(IEnumerable<VimSceneNode> nodes, int colorIndex)
        {
            for (var i = 0; i < Flags.Length; ++i)
                Flags[i] = false;
            foreach (var n in nodes)
            {
                var index = NodeIndexToInstanceIndex.GetOrDefault(n.Id, -1);
                if (index >= 0)
                    Flags[index] = true;
            }
            UpdateHighlighting(colorIndex);
        }

        public void HighlightNodes(Func<VimSceneNode, bool> f, int colorIndex)
            => HighlightNodes(Vim.VimNodes.Where(f), colorIndex);

        public void HideAll()
        {
            for (var i = 0; i < Flags.Length; ++i)
                Flags[i] = true;
            UpdateVisibility();
        }

        public void ShowAll()
        {
            for (var i = 0; i < Flags.Length; ++i)
                Flags[i] = false;
            UpdateVisibility();
        }

        public void UnhighlightAll()
        {
            for (var i = 0; i < Flags.Length; ++i)
                Flags[i] = false;
            UpdateHighlighting(0);
        }

        public void UpdateVisibility()
            => Api.Scene.HideInstances(Instances, Flags);

        public void UpdateHighlighting(int colorIndex)
            => Api.Scene.HighlightInstances(Instances, Flags, colorIndex);
    }
}
