using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.Geometry
{
    /// <summary>
    /// An IScene is a generic representation of a 3D scene graph.
    /// </summary>
    public interface IScene
    {
        IArray<ISceneNode> Nodes { get; }
        IArray<IMesh> Geometries { get; }
    }

    /// <summary>
    /// A node in a scene graph. 
    /// </summary>
    public interface ISceneNode
    {
        int Id { get; }
        IScene Scene { get; }
        Matrix4x4 Transform { get; }
        int GeometryIndex { get; }
        IMesh GetGeometry();
        ISceneNode Parent { get; }
    }
}
