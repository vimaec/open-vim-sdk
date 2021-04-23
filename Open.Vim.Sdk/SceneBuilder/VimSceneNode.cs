using Vim.DataFormat;
using Vim.Geometry;
using Vim.LinqArray;
using Vim.Math3d;
using Vim.ObjectModel;

namespace Vim
{
    public class VimSceneNode : ISceneNode, ITransformable3D<VimSceneNode>
    {
        public VimSceneNode(VimScene scene, SerializableSceneNode source, int index, Matrix4x4 transform)
        {
            _Scene = scene;
            _Source = source;
            Id = index;
            Transform = transform;
            GeometryIndex = source.Geometry;
            geometry = scene.Geometries.ElementAtOrDefault(GeometryIndex);
        }

        public SerializableSceneNode _Source { get; }
        public VimScene _Scene { get; }

        public IScene Scene => _Scene;
        public int Id { get; }
        public Matrix4x4 Transform { get; }

        private readonly IMesh geometry;

        public IMesh GetGeometry() => geometry;

        public int GeometryIndex { get; }

        public Node NodeModel => _Scene.Model.GetNode(Id);
        public ObjectModel.Geometry GeometryModel => _Scene.Model.GetGeometry(GeometryIndex);
        public Vector3 ModelCenter => GeometryModel.Box.AABox.Center.Transform(Transform);

        // TODO: I think this should be "IEnumerable<ISceneNode>" in the interface 
        public ISceneNode Parent => null;
        public IArray<ISceneNode> Children => LinqArray.LinqArray.Empty<ISceneNode>();

        // TODO: could a geometry contain multiple face IDs? 
        public int FaceId => GetGeometry()?.FaceGroups?.ElementAtOrDefault(0, -1) ?? -1;
        public int MaterialId => GetGeometry()?.FaceMaterialIds?.ElementAtOrDefault(0, -1) ?? -1;
        public int FaceCount => GetGeometry()?.NumFaces ?? 0;

        public Face Face => FaceId < 0 || FaceId >= _Scene.Model.FaceList.Count ? null : _Scene.Model.FaceList[FaceId];

        // TODO: this extra check should not be necessary, but it fails on some files like "rac_basic_sample_project.vim" and "B11.vim"
        public int ElementIndex => _Scene.Model.NodeElement?.ElementAtOrDefault(Id, -1) ?? Face?.Element?.Index ?? -1;
        public Element Element => _Scene.Model.GetElement(ElementIndex);
        public string ElementName => Element?.Name ?? "";

        public Category Category => Element?.Category;
        public string CategoryName => Category?.Name ?? "";
        public string DisciplineName => VimSceneHelpers.GetDisiplineFromCategory(CategoryName);

        public Material Material => _Scene.MaterialFromId(MaterialId);
        public string MaterialCategory => Material?.MaterialCategory ?? "";
        public string MaterialName => Material?.Name ?? "";
        public string MaterialColor => Material?.Color.ToString() ?? "";

        public FamilyInstance FamilyInstance => _Scene.GetFamilyInstance(Element);
        public FamilyType FamilyType => FamilyInstance?.FamilyType;
        public Family Family => FamilyInstance?.GetFamily();
        public string FamilyTypeName => FamilyType?.Element?.Name ?? "";
        public string FamilyName => _Scene.GetFamilyName(Element) ?? "";
        public string FamilyInstanceName => FamilyInstance?.Element?.Name ?? "";

        VimSceneNode ITransformable3D<VimSceneNode>.Transform(Matrix4x4 mat)
            => new VimSceneNode(_Scene, _Source, Id, mat * Transform);
    }
}
