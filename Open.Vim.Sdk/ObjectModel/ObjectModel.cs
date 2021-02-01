using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vim.DataFormat;
using Vim.DotNetUtilities;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.ObjectModel
{
    public partial class TableNameAttribute : Attribute
    {
        public string Name { get; }
        public TableNameAttribute(string name)
            => Name = name;
    }

    public static class AssetPaths
    {
        public const string Assets = "assets";
        public const string Textures = Assets + "/textures";
        public const string Renders = Assets + "/renders";
    }

    public class Relation<T>
    {
        public int Index;
        public Func<int, T> Getter;
        public T Value => Getter(Index);
        public Relation(int index, Func<int, T> getter)
            => (Index, Getter) = (index, getter);
    }

    public partial class Entity 
    {
        public int Index;
        public Document Document;

        public override int GetHashCode()
            => Hash.Combine(Index, Document.GetHashCode());

        /// <summary>
        /// This provides a fast comparison for the purpose of dictionary lookups. 
        /// </summary>
        public override bool Equals(object obj)
            => obj is Entity e
            && Index == e.Index
            && Document == e.Document
            && GetType() == obj.GetType();        
    }

    public partial class EntityWithElement : Entity
    {
        public Relation<Element> _Element;
    }

    [TableName(TableNames.Element)]
    public partial class Element : Entity
    {
        public int Id;
        public string Type;
        public string Name;
        public DVector3 Location;
        public string FamilyName;
        public Relation<Level> _Level;

        public Relation<Phase> _Phase;
        public Relation<Category> _Category;
        public Relation<Workset> _Workset;
        public Relation<DesignOption> _DesignOption;
        public Relation<View> _OwnerView;
        public Relation<Group> _Group;
        public Relation<AssemblyInstance> _AssemblyInstance;
        public Relation<Model> _Model;
        public Relation<Room> _Room;
    }

    [TableName(TableNames.Workset)]
    public partial class Workset : EntityWithElement
    {
        public string Kind;
    }

    [TableName(TableNames.AssemblyInstance)]
    public partial class AssemblyInstance : EntityWithElement
    {
        public string AssemblyTypeName;
        public DVector3 Position;
    }

    [TableName(TableNames.Group)]
    public partial class Group : EntityWithElement
    {
        public string GroupType;
        public DVector3 Position;
    }

    [TableName(TableNames.DesignOption)]
    public partial class DesignOption : EntityWithElement
    {
        public bool IsPrimary;
    }

    [TableName(TableNames.Level)]
    public partial class Level : EntityWithElement
    {
        /// <summary>Retrieves or changes the elevation above or below the ground level.
        ///    This property retrieves or changes the elevation above or below the ground level of the
        ///    project. If the Elevation Base parameter is set to Project, the elevation is relative to project origin.
        ///    If the Elevation Base parameter is set to Shared, the elevation is relative to shared origin which can
        ///    be changed by relocate operation. The value is given in decimal feet.
        /// </summary>
        public double Elevation;
    }

    [TableName(TableNames.Phase)]
    public partial class Phase : EntityWithElement
    {
    }

    [TableName(TableNames.Room)]
    public partial class Room : EntityWithElement
    {
        public double BaseOffset;
        public double LimitOffset;
        public double UnboundedHeight;
        public double Volume;
        public double Perimeter;
        public double Area;
        public string Number;         
        public Relation<Level> _UpperLimit;
    }

    [TableName(TableNames.Model)]
    public partial class Model : Entity
    {
        public string Title;
        public bool IsMetric;
        public string Guid;
        public int NumSaves;
        public bool IsLinked;
        public bool IsDetached;
        public bool IsWorkshared;
        public string PathName;
        public double Latitude;
        public double Longitude;
        public double TimeZone;
        public string PlaceName;
        public string WeatherStationName;
        public double Elevation;
        public string ProjectLocation;
        public string IssueDate;
        public string Status;
        public string ClientName;
        public string Address;
        public string Name;
        public string Number;
        public string Author;
        public string BuildingName;
        public string OrganizationName;
        public string OrganizationDescription;
        public string Product;
        public string Version;
        public string User;
        public Relation<View> _ActiveView;
        public Relation<Family> _OwnerFamily;
    }

    [TableName(TableNames.Category)]
    public partial class Category : Entity
    {
        public string Name;
        public int Id;
        public string CategoryType;
        public DVector3 LineColor;

        public Relation<Category> _Parent;
        public Relation<Material> _Material;
    }

    [TableName(TableNames.Face)]
    public partial class Face : EntityWithElement
    {
        public string Type;
        public double PeriodU;
        public double PeriodV;
        public double Radius1;
        public double Radius2;
        public double MinU;
        public double MinV;
        public double MaxU;
        public double MaxV;
        public double Area;

        public Relation<Material> _Material;
    }

    /// <summary>
    /// The Family element represents the entire family that consists of a collection of types, such as an 'I Beam'.
    /// You can think of that object as representing the entire family file. 
    /// </summary>
    [TableName(TableNames.Family)]
    public partial class Family : EntityWithElement
    {
        public string StructuralMaterialType;
        public string StructuralSectionShape;
        public Relation<Category> _FamilyCategory;
    }

    /// <summary>
    /// The Family object contains a number of FamilySymbol elements. The
    /// The FamilySymbol object represents a specific set of family settings within that Family and
    /// represents what is known in the Revit user interface as a Type, such as 'W14x32'. 
    /// </summary>
    [TableName(TableNames.FamilyType)]
    public partial class FamilyType : EntityWithElement
    {
        public Relation<Family> _Family;
    }

    /// <summary>
    /// The FamilyInstance object represents an actual instance of that
    /// type placed the Autodesk Revit project. For example the would
    /// be a single instance of a W14x32 column within the project. 
    /// </summary>
    [TableName(TableNames.FamilyInstance)]
    public partial class FamilyInstance : EntityWithElement
    {
        public bool FacingFlipped;
        public DVector3 FacingOrientation;
        public bool HandFlipped;
        public bool Mirrored;
        public bool HasModifiedGeometry;
        public double Scale;
        public DVector3 BasisX;
        public DVector3 BasisY;
        public DVector3 BasisZ;
        public DVector3 Translation;
        public DVector3 HandOrientation;
        public Relation<FamilyType> _FamilyType;
        public Relation<Element> _Host;
        public Relation<Room> _FromRoom;
        public Relation<Room> _ToRoom;
    }

    [TableName(TableNames.View)]
    public partial class View : EntityWithElement
    {
        public string Title;
        public DVector3 Up;
        public DVector3 Right;
        public DVector3 Origin;
        public DVector3 ViewDirection;
        public DVector3 ViewPosition;

        public Relation<Camera> _Camera;
    }

    [TableName(TableNames.Camera)]
    public partial class Camera : Entity
    {
        public int Id;
        /// <summary>Identifies whether the projection is orthographic 0 or perspective 1</summary>
        public int IsPerspective; 

        /// <summary>Distance between top and bottom planes on the target plane.</summary>
        public double VerticalExtent;

        /// <summary>Distance between left and right planes on the target plane.</summary>
        public double HorizontalExtent;

        /// <summary>
        ///    Distance from eye point to far plane of view frustum along the view direction.
        ///    This property together with NearDistance determines the depth restrictions of a view frustum.
        /// </summary>
        public double FarDistance;

        /// <summary>
        ///    Distance from eye point to near plane of view frustum along the view direction.
        ///    This property together with FarDistance determines the depth restrictions of a view frustum.
        /// </summary>
        public double NearDistance;

        /// <summary>Distance from eye point along view direction to target plane.
        ///    This value is appropriate for perspective views only.
        ///    Attempts to get this value for an orthographic view can
        ///    be made, but the obtained value is to be ignored.
        /// </summary>
        public double TargetDistance;

        /// <summary>
        ///    Distance that the target plane is offset towards the right
        ///    where right is normal to both Up direction and View direction.
        ///    This offset shifts both left and right planes.
        /// </summary>
        public double RightOffset;

        /// <summary>
        ///    Distance that the target plane is offset in the direction of
        ///    the Up direction. This offset shifts both top and bottom planes.
        /// </summary>
        public double UpOffset;
    }

    [TableName(TableNames.Material)]
    public partial class Material : Entity
    {
        /// <summary>
        /// Material ID. Note that a material is not an Element.
        /// </summary>
        public int Id;
        
        /// <summary>
        /// Material name
        /// </summary>
        public string Name;

        /// <summary>
        /// The type of the category.
        /// </summary>
        public string MaterialCategory;

        /// <summary>
        /// The diffuse (albedo) color.
        /// </summary>
        public DVector3 Color;

        /// <summary>
        /// The path of the diffuse (albedo) color texture.
        /// </summary>
        public string ColorTextureFile;

        /// <summary>
        /// The UV scaling factor of the diffuse (albedo) color texture.
        /// </summary>
        public DVector2 ColorUvScaling;

        /// <summary>
        /// The UV offset of the diffuse (albedo) color texture.
        /// </summary>
        public DVector2 ColorUvOffset;

        /// <summary>The level of glossiness of the materialNode</summary>
        public int Glossiness;

        /// <summary>
        /// The magnitude of the normal texture effect.
        /// </summary>
        public double NormalAmount;

        /// <summary>
        /// The path of the normal (bump) texture.
        /// </summary>
        public string NormalTextureFile;

        /// <summary>
        /// The UV scaling factor of the normal (bump) texture.
        /// </summary>
        public DVector2 NormalUvScaling;

        /// <summary>
        /// The UV offset of the normal (bump) texture.
        /// </summary>
        public DVector2 NormalUvOffset;

        /// <summary>The level of smoothness of the materialNode.</summary>
        public int Smoothness;

        /// <summary>The value of transparency the materialNode is being rendered with</summary>
        public double Transparency;
    }

    [TableName(TableNames.Node)]
    public partial class Node : EntityWithElement
    {
    }

    [TableName(TableNames.Geometry)]
    public partial class Geometry : Entity
    {
        public DAABox Box;
        public int VertexCount;
        public int FaceCount;
    }

    /// <summary>
    /// Helper functions for performing reflection over the object model
    /// </summary>
    public static class ObjectModelReflection
    {
        public static bool IsRelationType(this Type t)
            => t.Name == "Relation`1";

        public static IEnumerable<FieldInfo> GetEntityFields(this Type t)
            => t.GetFields().Where(fi =>
                !fi.FieldType.Equals(typeof(Document))
                && fi.Name != "Index"
                && fi.Name != "Properties"
                && !IsRelationType(fi.FieldType));

        public static Type RelationTypeParameter(this Type t)
            => t.GenericTypeArguments[0];

        public static IEnumerable<FieldInfo> GetRelationFields(this Type t)
            => t.GetFields().Where(fi => IsRelationType(fi.FieldType));

        public static bool IsEntityType(Type t)
            => typeof(Entity).IsAssignableFrom(t) && t.GetCustomAttribute(typeof(TableNameAttribute)) != null;

        public static IEnumerable<Type> GetEntityTypes()
            => Util.GetAllSubclassesOf(typeof(Entity).Assembly, typeof(Entity)).Where(IsEntityType);

        public static string GetEntityTableName(this Type t)
            => (t.GetCustomAttribute(typeof(TableNameAttribute)) as TableNameAttribute)?.Name;

        public static string GetRelationColumnName(this FieldInfo fi)
            => $"{fi.FieldType.RelationTypeParameter().Name}:{fi.Name.Substring(1)}";

        public static VimSchema GetCurrentSchema()
        {
            var vs = new VimSchema();
            vs.Version = VimConstants.ObjectModelVersion.ToString();
            foreach (var et in GetEntityTypes())
            {
                var t = vs.AddTable(GetEntityTableName(et).SimplifiedName());
                foreach (var f in GetEntityFields(et))
                    t.AddColumns(f.Name, f.FieldType);

                foreach (var f in GetRelationFields(et))
                    t.AddColumn(f.GetRelationColumnName(), VimSchema.ColumnType.Index);
            }
            return vs;
        }
    }
}
