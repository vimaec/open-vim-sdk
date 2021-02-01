// AUTO-GENERATED FILE, DO NOT MODIFY.
// ReSharper disable All
using System;
using System.Collections.Generic;
using System.Linq;
using Vim.DataFormat;
using Vim.Math3d;
using Vim.LinqArray;
using Vim.DotNetUtilities;

namespace Vim.ObjectModel {
    // AUTO-GENERATED
    public partial class Element
    {
        public Vim.ObjectModel.Level Level => _Level.Value;
        public Vim.ObjectModel.Phase Phase => _Phase.Value;
        public Vim.ObjectModel.Category Category => _Category.Value;
        public Vim.ObjectModel.Workset Workset => _Workset.Value;
        public Vim.ObjectModel.DesignOption DesignOption => _DesignOption.Value;
        public Vim.ObjectModel.View OwnerView => _OwnerView.Value;
        public Vim.ObjectModel.Group Group => _Group.Value;
        public Vim.ObjectModel.AssemblyInstance AssemblyInstance => _AssemblyInstance.Value;
        public Vim.ObjectModel.Model Model => _Model.Value;
        public Vim.ObjectModel.Room Room => _Room.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Workset
    {
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class AssemblyInstance
    {
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Group
    {
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class DesignOption
    {
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Level
    {
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Phase
    {
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Room
    {
        public Vim.ObjectModel.Level UpperLimit => _UpperLimit.Value;
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Model
    {
        public Vim.ObjectModel.View ActiveView => _ActiveView.Value;
        public Vim.ObjectModel.Family OwnerFamily => _OwnerFamily.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Category
    {
        public Vim.ObjectModel.Category Parent => _Parent.Value;
        public Vim.ObjectModel.Material Material => _Material.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Face
    {
        public Vim.ObjectModel.Material Material => _Material.Value;
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Family
    {
        public Vim.ObjectModel.Category FamilyCategory => _FamilyCategory.Value;
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class FamilyType
    {
        public Vim.ObjectModel.Family Family => _Family.Value;
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class FamilyInstance
    {
        public Vim.ObjectModel.FamilyType FamilyType => _FamilyType.Value;
        public Vim.ObjectModel.Element Host => _Host.Value;
        public Vim.ObjectModel.Room FromRoom => _FromRoom.Value;
        public Vim.ObjectModel.Room ToRoom => _ToRoom.Value;
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class View
    {
        public Vim.ObjectModel.Camera Camera => _Camera.Value;
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Camera
    {
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Material
    {
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Node
    {
        public Vim.ObjectModel.Element Element => _Element.Value;
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    // AUTO-GENERATED
    public partial class Geometry
    {
        public List<Property> Properties = new List<Property>();
    } // end of class
    
    public partial class DocumentModel
    {
        Document Document;
        public EntityTable ElementEntityTable => Document.GetTable("Rvt.Element");
        
        public IArray<Int32> ElementId => Document.GetColumnData<Int32>(ElementEntityTable, "Id");
        public IArray<String> ElementType => Document.GetColumnData<String>(ElementEntityTable, "Type");
        public IArray<String> ElementName => Document.GetColumnData<String>(ElementEntityTable, "Name");
        public IArray<DVector3> ElementLocation => Document.GetColumnData<DVector3>(ElementEntityTable, "Location");
        public IArray<String> ElementFamilyName => Document.GetColumnData<String>(ElementEntityTable, "FamilyName");
        public IArray<int> ElementLevel => Document.GetColumnData<int>(ElementEntityTable, "Level");
        public IArray<int> ElementPhase => Document.GetColumnData<int>(ElementEntityTable, "Phase");
        public IArray<int> ElementCategory => Document.GetColumnData<int>(ElementEntityTable, "Category");
        public IArray<int> ElementWorkset => Document.GetColumnData<int>(ElementEntityTable, "Workset");
        public IArray<int> ElementDesignOption => Document.GetColumnData<int>(ElementEntityTable, "DesignOption");
        public IArray<int> ElementOwnerView => Document.GetColumnData<int>(ElementEntityTable, "OwnerView");
        public IArray<int> ElementGroup => Document.GetColumnData<int>(ElementEntityTable, "Group");
        public IArray<int> ElementAssemblyInstance => Document.GetColumnData<int>(ElementEntityTable, "AssemblyInstance");
        public IArray<int> ElementModel => Document.GetColumnData<int>(ElementEntityTable, "Model");
        public IArray<int> ElementRoom => Document.GetColumnData<int>(ElementEntityTable, "Room");
        public int NumElement => ElementEntityTable?.NumRows ?? 0;
        public Element GetElement(int n)
        {
            if (n < 0) return null;
            var r = new Element();
            r.Document = Document;
            r.Index = n;
            r.Id = ElementId?[n] ?? default;
            r.Type = ElementType?[n] ?? default;
            r.Name = ElementName?[n] ?? default;
            r.Location = ElementLocation?[n] ?? default;
            r.FamilyName = ElementFamilyName?[n] ?? default;
            r._Level = new Relation<Vim.ObjectModel.Level>(ElementLevel?.ElementAtOrDefault(n, -1) ?? -1, GetLevel);
            r._Phase = new Relation<Vim.ObjectModel.Phase>(ElementPhase?.ElementAtOrDefault(n, -1) ?? -1, GetPhase);
            r._Category = new Relation<Vim.ObjectModel.Category>(ElementCategory?.ElementAtOrDefault(n, -1) ?? -1, GetCategory);
            r._Workset = new Relation<Vim.ObjectModel.Workset>(ElementWorkset?.ElementAtOrDefault(n, -1) ?? -1, GetWorkset);
            r._DesignOption = new Relation<Vim.ObjectModel.DesignOption>(ElementDesignOption?.ElementAtOrDefault(n, -1) ?? -1, GetDesignOption);
            r._OwnerView = new Relation<Vim.ObjectModel.View>(ElementOwnerView?.ElementAtOrDefault(n, -1) ?? -1, GetView);
            r._Group = new Relation<Vim.ObjectModel.Group>(ElementGroup?.ElementAtOrDefault(n, -1) ?? -1, GetGroup);
            r._AssemblyInstance = new Relation<Vim.ObjectModel.AssemblyInstance>(ElementAssemblyInstance?.ElementAtOrDefault(n, -1) ?? -1, GetAssemblyInstance);
            r._Model = new Relation<Vim.ObjectModel.Model>(ElementModel?.ElementAtOrDefault(n, -1) ?? -1, GetModel);
            r._Room = new Relation<Vim.ObjectModel.Room>(ElementRoom?.ElementAtOrDefault(n, -1) ?? -1, GetRoom);
            r.Properties = ElementPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable WorksetEntityTable => Document.GetTable("Rvt.Workset");
        
        public IArray<String> WorksetKind => Document.GetColumnData<String>(WorksetEntityTable, "Kind");
        public IArray<int> WorksetElement => Document.GetColumnData<int>(WorksetEntityTable, "Element");
        public int NumWorkset => WorksetEntityTable?.NumRows ?? 0;
        public Workset GetWorkset(int n)
        {
            if (n < 0) return null;
            var r = new Workset();
            r.Document = Document;
            r.Index = n;
            r.Kind = WorksetKind?[n] ?? default;
            r._Element = new Relation<Vim.ObjectModel.Element>(WorksetElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = WorksetPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable AssemblyInstanceEntityTable => Document.GetTable("Rvt.AssemblyInstance");
        
        public IArray<String> AssemblyInstanceAssemblyTypeName => Document.GetColumnData<String>(AssemblyInstanceEntityTable, "AssemblyTypeName");
        public IArray<DVector3> AssemblyInstancePosition => Document.GetColumnData<DVector3>(AssemblyInstanceEntityTable, "Position");
        public IArray<int> AssemblyInstanceElement => Document.GetColumnData<int>(AssemblyInstanceEntityTable, "Element");
        public int NumAssemblyInstance => AssemblyInstanceEntityTable?.NumRows ?? 0;
        public AssemblyInstance GetAssemblyInstance(int n)
        {
            if (n < 0) return null;
            var r = new AssemblyInstance();
            r.Document = Document;
            r.Index = n;
            r.AssemblyTypeName = AssemblyInstanceAssemblyTypeName?[n] ?? default;
            r.Position = AssemblyInstancePosition?[n] ?? default;
            r._Element = new Relation<Vim.ObjectModel.Element>(AssemblyInstanceElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = AssemblyInstancePropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable GroupEntityTable => Document.GetTable("Rvt.Group");
        
        public IArray<String> GroupGroupType => Document.GetColumnData<String>(GroupEntityTable, "GroupType");
        public IArray<DVector3> GroupPosition => Document.GetColumnData<DVector3>(GroupEntityTable, "Position");
        public IArray<int> GroupElement => Document.GetColumnData<int>(GroupEntityTable, "Element");
        public int NumGroup => GroupEntityTable?.NumRows ?? 0;
        public Group GetGroup(int n)
        {
            if (n < 0) return null;
            var r = new Group();
            r.Document = Document;
            r.Index = n;
            r.GroupType = GroupGroupType?[n] ?? default;
            r.Position = GroupPosition?[n] ?? default;
            r._Element = new Relation<Vim.ObjectModel.Element>(GroupElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = GroupPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable DesignOptionEntityTable => Document.GetTable("Rvt.DesignOption");
        
        public IArray<Boolean> DesignOptionIsPrimary => Document.GetColumnData<Boolean>(DesignOptionEntityTable, "IsPrimary");
        public IArray<int> DesignOptionElement => Document.GetColumnData<int>(DesignOptionEntityTable, "Element");
        public int NumDesignOption => DesignOptionEntityTable?.NumRows ?? 0;
        public DesignOption GetDesignOption(int n)
        {
            if (n < 0) return null;
            var r = new DesignOption();
            r.Document = Document;
            r.Index = n;
            r.IsPrimary = DesignOptionIsPrimary?[n] ?? default;
            r._Element = new Relation<Vim.ObjectModel.Element>(DesignOptionElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = DesignOptionPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable LevelEntityTable => Document.GetTable("Rvt.Level");
        
        public IArray<Double> LevelElevation => Document.GetColumnData<Double>(LevelEntityTable, "Elevation");
        public IArray<int> LevelElement => Document.GetColumnData<int>(LevelEntityTable, "Element");
        public int NumLevel => LevelEntityTable?.NumRows ?? 0;
        public Level GetLevel(int n)
        {
            if (n < 0) return null;
            var r = new Level();
            r.Document = Document;
            r.Index = n;
            r.Elevation = LevelElevation?[n] ?? default;
            r._Element = new Relation<Vim.ObjectModel.Element>(LevelElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = LevelPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable PhaseEntityTable => Document.GetTable("Rvt.Phase");
        
        public IArray<int> PhaseElement => Document.GetColumnData<int>(PhaseEntityTable, "Element");
        public int NumPhase => PhaseEntityTable?.NumRows ?? 0;
        public Phase GetPhase(int n)
        {
            if (n < 0) return null;
            var r = new Phase();
            r.Document = Document;
            r.Index = n;
            r._Element = new Relation<Vim.ObjectModel.Element>(PhaseElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = PhasePropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable RoomEntityTable => Document.GetTable("Rvt.Room");
        
        public IArray<Double> RoomBaseOffset => Document.GetColumnData<Double>(RoomEntityTable, "BaseOffset");
        public IArray<Double> RoomLimitOffset => Document.GetColumnData<Double>(RoomEntityTable, "LimitOffset");
        public IArray<Double> RoomUnboundedHeight => Document.GetColumnData<Double>(RoomEntityTable, "UnboundedHeight");
        public IArray<Double> RoomVolume => Document.GetColumnData<Double>(RoomEntityTable, "Volume");
        public IArray<Double> RoomPerimeter => Document.GetColumnData<Double>(RoomEntityTable, "Perimeter");
        public IArray<Double> RoomArea => Document.GetColumnData<Double>(RoomEntityTable, "Area");
        public IArray<String> RoomNumber => Document.GetColumnData<String>(RoomEntityTable, "Number");
        public IArray<int> RoomUpperLimit => Document.GetColumnData<int>(RoomEntityTable, "UpperLimit");
        public IArray<int> RoomElement => Document.GetColumnData<int>(RoomEntityTable, "Element");
        public int NumRoom => RoomEntityTable?.NumRows ?? 0;
        public Room GetRoom(int n)
        {
            if (n < 0) return null;
            var r = new Room();
            r.Document = Document;
            r.Index = n;
            r.BaseOffset = RoomBaseOffset?[n] ?? default;
            r.LimitOffset = RoomLimitOffset?[n] ?? default;
            r.UnboundedHeight = RoomUnboundedHeight?[n] ?? default;
            r.Volume = RoomVolume?[n] ?? default;
            r.Perimeter = RoomPerimeter?[n] ?? default;
            r.Area = RoomArea?[n] ?? default;
            r.Number = RoomNumber?[n] ?? default;
            r._UpperLimit = new Relation<Vim.ObjectModel.Level>(RoomUpperLimit?.ElementAtOrDefault(n, -1) ?? -1, GetLevel);
            r._Element = new Relation<Vim.ObjectModel.Element>(RoomElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = RoomPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable ModelEntityTable => Document.GetTable("Rvt.Model");
        
        public IArray<String> ModelTitle => Document.GetColumnData<String>(ModelEntityTable, "Title");
        public IArray<Boolean> ModelIsMetric => Document.GetColumnData<Boolean>(ModelEntityTable, "IsMetric");
        public IArray<String> ModelGuid => Document.GetColumnData<String>(ModelEntityTable, "Guid");
        public IArray<Int32> ModelNumSaves => Document.GetColumnData<Int32>(ModelEntityTable, "NumSaves");
        public IArray<Boolean> ModelIsLinked => Document.GetColumnData<Boolean>(ModelEntityTable, "IsLinked");
        public IArray<Boolean> ModelIsDetached => Document.GetColumnData<Boolean>(ModelEntityTable, "IsDetached");
        public IArray<Boolean> ModelIsWorkshared => Document.GetColumnData<Boolean>(ModelEntityTable, "IsWorkshared");
        public IArray<String> ModelPathName => Document.GetColumnData<String>(ModelEntityTable, "PathName");
        public IArray<Double> ModelLatitude => Document.GetColumnData<Double>(ModelEntityTable, "Latitude");
        public IArray<Double> ModelLongitude => Document.GetColumnData<Double>(ModelEntityTable, "Longitude");
        public IArray<Double> ModelTimeZone => Document.GetColumnData<Double>(ModelEntityTable, "TimeZone");
        public IArray<String> ModelPlaceName => Document.GetColumnData<String>(ModelEntityTable, "PlaceName");
        public IArray<String> ModelWeatherStationName => Document.GetColumnData<String>(ModelEntityTable, "WeatherStationName");
        public IArray<Double> ModelElevation => Document.GetColumnData<Double>(ModelEntityTable, "Elevation");
        public IArray<String> ModelProjectLocation => Document.GetColumnData<String>(ModelEntityTable, "ProjectLocation");
        public IArray<String> ModelIssueDate => Document.GetColumnData<String>(ModelEntityTable, "IssueDate");
        public IArray<String> ModelStatus => Document.GetColumnData<String>(ModelEntityTable, "Status");
        public IArray<String> ModelClientName => Document.GetColumnData<String>(ModelEntityTable, "ClientName");
        public IArray<String> ModelAddress => Document.GetColumnData<String>(ModelEntityTable, "Address");
        public IArray<String> ModelName => Document.GetColumnData<String>(ModelEntityTable, "Name");
        public IArray<String> ModelNumber => Document.GetColumnData<String>(ModelEntityTable, "Number");
        public IArray<String> ModelAuthor => Document.GetColumnData<String>(ModelEntityTable, "Author");
        public IArray<String> ModelBuildingName => Document.GetColumnData<String>(ModelEntityTable, "BuildingName");
        public IArray<String> ModelOrganizationName => Document.GetColumnData<String>(ModelEntityTable, "OrganizationName");
        public IArray<String> ModelOrganizationDescription => Document.GetColumnData<String>(ModelEntityTable, "OrganizationDescription");
        public IArray<String> ModelProduct => Document.GetColumnData<String>(ModelEntityTable, "Product");
        public IArray<String> ModelVersion => Document.GetColumnData<String>(ModelEntityTable, "Version");
        public IArray<String> ModelUser => Document.GetColumnData<String>(ModelEntityTable, "User");
        public IArray<int> ModelActiveView => Document.GetColumnData<int>(ModelEntityTable, "ActiveView");
        public IArray<int> ModelOwnerFamily => Document.GetColumnData<int>(ModelEntityTable, "OwnerFamily");
        public int NumModel => ModelEntityTable?.NumRows ?? 0;
        public Model GetModel(int n)
        {
            if (n < 0) return null;
            var r = new Model();
            r.Document = Document;
            r.Index = n;
            r.Title = ModelTitle?[n] ?? default;
            r.IsMetric = ModelIsMetric?[n] ?? default;
            r.Guid = ModelGuid?[n] ?? default;
            r.NumSaves = ModelNumSaves?[n] ?? default;
            r.IsLinked = ModelIsLinked?[n] ?? default;
            r.IsDetached = ModelIsDetached?[n] ?? default;
            r.IsWorkshared = ModelIsWorkshared?[n] ?? default;
            r.PathName = ModelPathName?[n] ?? default;
            r.Latitude = ModelLatitude?[n] ?? default;
            r.Longitude = ModelLongitude?[n] ?? default;
            r.TimeZone = ModelTimeZone?[n] ?? default;
            r.PlaceName = ModelPlaceName?[n] ?? default;
            r.WeatherStationName = ModelWeatherStationName?[n] ?? default;
            r.Elevation = ModelElevation?[n] ?? default;
            r.ProjectLocation = ModelProjectLocation?[n] ?? default;
            r.IssueDate = ModelIssueDate?[n] ?? default;
            r.Status = ModelStatus?[n] ?? default;
            r.ClientName = ModelClientName?[n] ?? default;
            r.Address = ModelAddress?[n] ?? default;
            r.Name = ModelName?[n] ?? default;
            r.Number = ModelNumber?[n] ?? default;
            r.Author = ModelAuthor?[n] ?? default;
            r.BuildingName = ModelBuildingName?[n] ?? default;
            r.OrganizationName = ModelOrganizationName?[n] ?? default;
            r.OrganizationDescription = ModelOrganizationDescription?[n] ?? default;
            r.Product = ModelProduct?[n] ?? default;
            r.Version = ModelVersion?[n] ?? default;
            r.User = ModelUser?[n] ?? default;
            r._ActiveView = new Relation<Vim.ObjectModel.View>(ModelActiveView?.ElementAtOrDefault(n, -1) ?? -1, GetView);
            r._OwnerFamily = new Relation<Vim.ObjectModel.Family>(ModelOwnerFamily?.ElementAtOrDefault(n, -1) ?? -1, GetFamily);
            r.Properties = ModelPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable CategoryEntityTable => Document.GetTable("Rvt.Category");
        
        public IArray<String> CategoryName => Document.GetColumnData<String>(CategoryEntityTable, "Name");
        public IArray<Int32> CategoryId => Document.GetColumnData<Int32>(CategoryEntityTable, "Id");
        public IArray<String> CategoryCategoryType => Document.GetColumnData<String>(CategoryEntityTable, "CategoryType");
        public IArray<DVector3> CategoryLineColor => Document.GetColumnData<DVector3>(CategoryEntityTable, "LineColor");
        public IArray<int> CategoryParent => Document.GetColumnData<int>(CategoryEntityTable, "Parent");
        public IArray<int> CategoryMaterial => Document.GetColumnData<int>(CategoryEntityTable, "Material");
        public int NumCategory => CategoryEntityTable?.NumRows ?? 0;
        public Category GetCategory(int n)
        {
            if (n < 0) return null;
            var r = new Category();
            r.Document = Document;
            r.Index = n;
            r.Name = CategoryName?[n] ?? default;
            r.Id = CategoryId?[n] ?? default;
            r.CategoryType = CategoryCategoryType?[n] ?? default;
            r.LineColor = CategoryLineColor?[n] ?? default;
            r._Parent = new Relation<Vim.ObjectModel.Category>(CategoryParent?.ElementAtOrDefault(n, -1) ?? -1, GetCategory);
            r._Material = new Relation<Vim.ObjectModel.Material>(CategoryMaterial?.ElementAtOrDefault(n, -1) ?? -1, GetMaterial);
            r.Properties = CategoryPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable FaceEntityTable => Document.GetTable("Rvt.Face");
        
        public IArray<String> FaceType => Document.GetColumnData<String>(FaceEntityTable, "Type");
        public IArray<Double> FacePeriodU => Document.GetColumnData<Double>(FaceEntityTable, "PeriodU");
        public IArray<Double> FacePeriodV => Document.GetColumnData<Double>(FaceEntityTable, "PeriodV");
        public IArray<Double> FaceRadius1 => Document.GetColumnData<Double>(FaceEntityTable, "Radius1");
        public IArray<Double> FaceRadius2 => Document.GetColumnData<Double>(FaceEntityTable, "Radius2");
        public IArray<Double> FaceMinU => Document.GetColumnData<Double>(FaceEntityTable, "MinU");
        public IArray<Double> FaceMinV => Document.GetColumnData<Double>(FaceEntityTable, "MinV");
        public IArray<Double> FaceMaxU => Document.GetColumnData<Double>(FaceEntityTable, "MaxU");
        public IArray<Double> FaceMaxV => Document.GetColumnData<Double>(FaceEntityTable, "MaxV");
        public IArray<Double> FaceArea => Document.GetColumnData<Double>(FaceEntityTable, "Area");
        public IArray<int> FaceMaterial => Document.GetColumnData<int>(FaceEntityTable, "Material");
        public IArray<int> FaceElement => Document.GetColumnData<int>(FaceEntityTable, "Element");
        public int NumFace => FaceEntityTable?.NumRows ?? 0;
        public Face GetFace(int n)
        {
            if (n < 0) return null;
            var r = new Face();
            r.Document = Document;
            r.Index = n;
            r.Type = FaceType?[n] ?? default;
            r.PeriodU = FacePeriodU?[n] ?? default;
            r.PeriodV = FacePeriodV?[n] ?? default;
            r.Radius1 = FaceRadius1?[n] ?? default;
            r.Radius2 = FaceRadius2?[n] ?? default;
            r.MinU = FaceMinU?[n] ?? default;
            r.MinV = FaceMinV?[n] ?? default;
            r.MaxU = FaceMaxU?[n] ?? default;
            r.MaxV = FaceMaxV?[n] ?? default;
            r.Area = FaceArea?[n] ?? default;
            r._Material = new Relation<Vim.ObjectModel.Material>(FaceMaterial?.ElementAtOrDefault(n, -1) ?? -1, GetMaterial);
            r._Element = new Relation<Vim.ObjectModel.Element>(FaceElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = FacePropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable FamilyEntityTable => Document.GetTable("Rvt.Family");
        
        public IArray<String> FamilyStructuralMaterialType => Document.GetColumnData<String>(FamilyEntityTable, "StructuralMaterialType");
        public IArray<String> FamilyStructuralSectionShape => Document.GetColumnData<String>(FamilyEntityTable, "StructuralSectionShape");
        public IArray<int> FamilyFamilyCategory => Document.GetColumnData<int>(FamilyEntityTable, "FamilyCategory");
        public IArray<int> FamilyElement => Document.GetColumnData<int>(FamilyEntityTable, "Element");
        public int NumFamily => FamilyEntityTable?.NumRows ?? 0;
        public Family GetFamily(int n)
        {
            if (n < 0) return null;
            var r = new Family();
            r.Document = Document;
            r.Index = n;
            r.StructuralMaterialType = FamilyStructuralMaterialType?[n] ?? default;
            r.StructuralSectionShape = FamilyStructuralSectionShape?[n] ?? default;
            r._FamilyCategory = new Relation<Vim.ObjectModel.Category>(FamilyFamilyCategory?.ElementAtOrDefault(n, -1) ?? -1, GetCategory);
            r._Element = new Relation<Vim.ObjectModel.Element>(FamilyElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = FamilyPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable FamilyTypeEntityTable => Document.GetTable("Rvt.FamilyType");
        
        public IArray<int> FamilyTypeFamily => Document.GetColumnData<int>(FamilyTypeEntityTable, "Family");
        public IArray<int> FamilyTypeElement => Document.GetColumnData<int>(FamilyTypeEntityTable, "Element");
        public int NumFamilyType => FamilyTypeEntityTable?.NumRows ?? 0;
        public FamilyType GetFamilyType(int n)
        {
            if (n < 0) return null;
            var r = new FamilyType();
            r.Document = Document;
            r.Index = n;
            r._Family = new Relation<Vim.ObjectModel.Family>(FamilyTypeFamily?.ElementAtOrDefault(n, -1) ?? -1, GetFamily);
            r._Element = new Relation<Vim.ObjectModel.Element>(FamilyTypeElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = FamilyTypePropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable FamilyInstanceEntityTable => Document.GetTable("Rvt.FamilyInstance");
        
        public IArray<Boolean> FamilyInstanceFacingFlipped => Document.GetColumnData<Boolean>(FamilyInstanceEntityTable, "FacingFlipped");
        public IArray<DVector3> FamilyInstanceFacingOrientation => Document.GetColumnData<DVector3>(FamilyInstanceEntityTable, "FacingOrientation");
        public IArray<Boolean> FamilyInstanceHandFlipped => Document.GetColumnData<Boolean>(FamilyInstanceEntityTable, "HandFlipped");
        public IArray<Boolean> FamilyInstanceMirrored => Document.GetColumnData<Boolean>(FamilyInstanceEntityTable, "Mirrored");
        public IArray<Boolean> FamilyInstanceHasModifiedGeometry => Document.GetColumnData<Boolean>(FamilyInstanceEntityTable, "HasModifiedGeometry");
        public IArray<Double> FamilyInstanceScale => Document.GetColumnData<Double>(FamilyInstanceEntityTable, "Scale");
        public IArray<DVector3> FamilyInstanceBasisX => Document.GetColumnData<DVector3>(FamilyInstanceEntityTable, "BasisX");
        public IArray<DVector3> FamilyInstanceBasisY => Document.GetColumnData<DVector3>(FamilyInstanceEntityTable, "BasisY");
        public IArray<DVector3> FamilyInstanceBasisZ => Document.GetColumnData<DVector3>(FamilyInstanceEntityTable, "BasisZ");
        public IArray<DVector3> FamilyInstanceTranslation => Document.GetColumnData<DVector3>(FamilyInstanceEntityTable, "Translation");
        public IArray<DVector3> FamilyInstanceHandOrientation => Document.GetColumnData<DVector3>(FamilyInstanceEntityTable, "HandOrientation");
        public IArray<int> FamilyInstanceFamilyType => Document.GetColumnData<int>(FamilyInstanceEntityTable, "FamilyType");
        public IArray<int> FamilyInstanceHost => Document.GetColumnData<int>(FamilyInstanceEntityTable, "Host");
        public IArray<int> FamilyInstanceFromRoom => Document.GetColumnData<int>(FamilyInstanceEntityTable, "FromRoom");
        public IArray<int> FamilyInstanceToRoom => Document.GetColumnData<int>(FamilyInstanceEntityTable, "ToRoom");
        public IArray<int> FamilyInstanceElement => Document.GetColumnData<int>(FamilyInstanceEntityTable, "Element");
        public int NumFamilyInstance => FamilyInstanceEntityTable?.NumRows ?? 0;
        public FamilyInstance GetFamilyInstance(int n)
        {
            if (n < 0) return null;
            var r = new FamilyInstance();
            r.Document = Document;
            r.Index = n;
            r.FacingFlipped = FamilyInstanceFacingFlipped?[n] ?? default;
            r.FacingOrientation = FamilyInstanceFacingOrientation?[n] ?? default;
            r.HandFlipped = FamilyInstanceHandFlipped?[n] ?? default;
            r.Mirrored = FamilyInstanceMirrored?[n] ?? default;
            r.HasModifiedGeometry = FamilyInstanceHasModifiedGeometry?[n] ?? default;
            r.Scale = FamilyInstanceScale?[n] ?? default;
            r.BasisX = FamilyInstanceBasisX?[n] ?? default;
            r.BasisY = FamilyInstanceBasisY?[n] ?? default;
            r.BasisZ = FamilyInstanceBasisZ?[n] ?? default;
            r.Translation = FamilyInstanceTranslation?[n] ?? default;
            r.HandOrientation = FamilyInstanceHandOrientation?[n] ?? default;
            r._FamilyType = new Relation<Vim.ObjectModel.FamilyType>(FamilyInstanceFamilyType?.ElementAtOrDefault(n, -1) ?? -1, GetFamilyType);
            r._Host = new Relation<Vim.ObjectModel.Element>(FamilyInstanceHost?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r._FromRoom = new Relation<Vim.ObjectModel.Room>(FamilyInstanceFromRoom?.ElementAtOrDefault(n, -1) ?? -1, GetRoom);
            r._ToRoom = new Relation<Vim.ObjectModel.Room>(FamilyInstanceToRoom?.ElementAtOrDefault(n, -1) ?? -1, GetRoom);
            r._Element = new Relation<Vim.ObjectModel.Element>(FamilyInstanceElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = FamilyInstancePropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable ViewEntityTable => Document.GetTable("Rvt.View");
        
        public IArray<String> ViewTitle => Document.GetColumnData<String>(ViewEntityTable, "Title");
        public IArray<DVector3> ViewUp => Document.GetColumnData<DVector3>(ViewEntityTable, "Up");
        public IArray<DVector3> ViewRight => Document.GetColumnData<DVector3>(ViewEntityTable, "Right");
        public IArray<DVector3> ViewOrigin => Document.GetColumnData<DVector3>(ViewEntityTable, "Origin");
        public IArray<DVector3> ViewViewDirection => Document.GetColumnData<DVector3>(ViewEntityTable, "ViewDirection");
        public IArray<DVector3> ViewViewPosition => Document.GetColumnData<DVector3>(ViewEntityTable, "ViewPosition");
        public IArray<int> ViewCamera => Document.GetColumnData<int>(ViewEntityTable, "Camera");
        public IArray<int> ViewElement => Document.GetColumnData<int>(ViewEntityTable, "Element");
        public int NumView => ViewEntityTable?.NumRows ?? 0;
        public View GetView(int n)
        {
            if (n < 0) return null;
            var r = new View();
            r.Document = Document;
            r.Index = n;
            r.Title = ViewTitle?[n] ?? default;
            r.Up = ViewUp?[n] ?? default;
            r.Right = ViewRight?[n] ?? default;
            r.Origin = ViewOrigin?[n] ?? default;
            r.ViewDirection = ViewViewDirection?[n] ?? default;
            r.ViewPosition = ViewViewPosition?[n] ?? default;
            r._Camera = new Relation<Vim.ObjectModel.Camera>(ViewCamera?.ElementAtOrDefault(n, -1) ?? -1, GetCamera);
            r._Element = new Relation<Vim.ObjectModel.Element>(ViewElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = ViewPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable CameraEntityTable => Document.GetTable("Rvt.Camera");
        
        public IArray<Int32> CameraId => Document.GetColumnData<Int32>(CameraEntityTable, "Id");
        public IArray<Int32> CameraIsPerspective => Document.GetColumnData<Int32>(CameraEntityTable, "IsPerspective");
        public IArray<Double> CameraVerticalExtent => Document.GetColumnData<Double>(CameraEntityTable, "VerticalExtent");
        public IArray<Double> CameraHorizontalExtent => Document.GetColumnData<Double>(CameraEntityTable, "HorizontalExtent");
        public IArray<Double> CameraFarDistance => Document.GetColumnData<Double>(CameraEntityTable, "FarDistance");
        public IArray<Double> CameraNearDistance => Document.GetColumnData<Double>(CameraEntityTable, "NearDistance");
        public IArray<Double> CameraTargetDistance => Document.GetColumnData<Double>(CameraEntityTable, "TargetDistance");
        public IArray<Double> CameraRightOffset => Document.GetColumnData<Double>(CameraEntityTable, "RightOffset");
        public IArray<Double> CameraUpOffset => Document.GetColumnData<Double>(CameraEntityTable, "UpOffset");
        public int NumCamera => CameraEntityTable?.NumRows ?? 0;
        public Camera GetCamera(int n)
        {
            if (n < 0) return null;
            var r = new Camera();
            r.Document = Document;
            r.Index = n;
            r.Id = CameraId?[n] ?? default;
            r.IsPerspective = CameraIsPerspective?[n] ?? default;
            r.VerticalExtent = CameraVerticalExtent?[n] ?? default;
            r.HorizontalExtent = CameraHorizontalExtent?[n] ?? default;
            r.FarDistance = CameraFarDistance?[n] ?? default;
            r.NearDistance = CameraNearDistance?[n] ?? default;
            r.TargetDistance = CameraTargetDistance?[n] ?? default;
            r.RightOffset = CameraRightOffset?[n] ?? default;
            r.UpOffset = CameraUpOffset?[n] ?? default;
            r.Properties = CameraPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable MaterialEntityTable => Document.GetTable("Rvt.Material");
        
        public IArray<Int32> MaterialId => Document.GetColumnData<Int32>(MaterialEntityTable, "Id");
        public IArray<String> MaterialName => Document.GetColumnData<String>(MaterialEntityTable, "Name");
        public IArray<String> MaterialMaterialCategory => Document.GetColumnData<String>(MaterialEntityTable, "MaterialCategory");
        public IArray<DVector3> MaterialColor => Document.GetColumnData<DVector3>(MaterialEntityTable, "Color");
        public IArray<String> MaterialColorTextureFile => Document.GetColumnData<String>(MaterialEntityTable, "ColorTextureFile");
        public IArray<DVector2> MaterialColorUvScaling => Document.GetColumnData<DVector2>(MaterialEntityTable, "ColorUvScaling");
        public IArray<DVector2> MaterialColorUvOffset => Document.GetColumnData<DVector2>(MaterialEntityTable, "ColorUvOffset");
        public IArray<Int32> MaterialGlossiness => Document.GetColumnData<Int32>(MaterialEntityTable, "Glossiness");
        public IArray<Double> MaterialNormalAmount => Document.GetColumnData<Double>(MaterialEntityTable, "NormalAmount");
        public IArray<String> MaterialNormalTextureFile => Document.GetColumnData<String>(MaterialEntityTable, "NormalTextureFile");
        public IArray<DVector2> MaterialNormalUvScaling => Document.GetColumnData<DVector2>(MaterialEntityTable, "NormalUvScaling");
        public IArray<DVector2> MaterialNormalUvOffset => Document.GetColumnData<DVector2>(MaterialEntityTable, "NormalUvOffset");
        public IArray<Int32> MaterialSmoothness => Document.GetColumnData<Int32>(MaterialEntityTable, "Smoothness");
        public IArray<Double> MaterialTransparency => Document.GetColumnData<Double>(MaterialEntityTable, "Transparency");
        public int NumMaterial => MaterialEntityTable?.NumRows ?? 0;
        public Material GetMaterial(int n)
        {
            if (n < 0) return null;
            var r = new Material();
            r.Document = Document;
            r.Index = n;
            r.Id = MaterialId?[n] ?? default;
            r.Name = MaterialName?[n] ?? default;
            r.MaterialCategory = MaterialMaterialCategory?[n] ?? default;
            r.Color = MaterialColor?[n] ?? default;
            r.ColorTextureFile = MaterialColorTextureFile?[n] ?? default;
            r.ColorUvScaling = MaterialColorUvScaling?[n] ?? default;
            r.ColorUvOffset = MaterialColorUvOffset?[n] ?? default;
            r.Glossiness = MaterialGlossiness?[n] ?? default;
            r.NormalAmount = MaterialNormalAmount?[n] ?? default;
            r.NormalTextureFile = MaterialNormalTextureFile?[n] ?? default;
            r.NormalUvScaling = MaterialNormalUvScaling?[n] ?? default;
            r.NormalUvOffset = MaterialNormalUvOffset?[n] ?? default;
            r.Smoothness = MaterialSmoothness?[n] ?? default;
            r.Transparency = MaterialTransparency?[n] ?? default;
            r.Properties = MaterialPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable NodeEntityTable => Document.GetTable("Vim.Node");
        
        public IArray<int> NodeElement => Document.GetColumnData<int>(NodeEntityTable, "Element");
        public int NumNode => NodeEntityTable?.NumRows ?? 0;
        public Node GetNode(int n)
        {
            if (n < 0) return null;
            var r = new Node();
            r.Document = Document;
            r.Index = n;
            r._Element = new Relation<Vim.ObjectModel.Element>(NodeElement?.ElementAtOrDefault(n, -1) ?? -1, GetElement);
            r.Properties = NodePropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        public EntityTable GeometryEntityTable => Document.GetTable("Vim.Geometry");
        
        public IArray<DAABox> GeometryBox => Document.GetColumnData<DAABox>(GeometryEntityTable, "Box");
        public IArray<Int32> GeometryVertexCount => Document.GetColumnData<Int32>(GeometryEntityTable, "VertexCount");
        public IArray<Int32> GeometryFaceCount => Document.GetColumnData<Int32>(GeometryEntityTable, "FaceCount");
        public int NumGeometry => GeometryEntityTable?.NumRows ?? 0;
        public Geometry GetGeometry(int n)
        {
            if (n < 0) return null;
            var r = new Geometry();
            r.Document = Document;
            r.Index = n;
            r.Box = GeometryBox?[n] ?? default;
            r.VertexCount = GeometryVertexCount?[n] ?? default;
            r.FaceCount = GeometryFaceCount?[n] ?? default;
            r.Properties = GeometryPropertyLists.GetOrDefault(n) ?? r.Properties;
            return r;
        }
        
        // Entity collections
        public readonly IArray<Element> ElementList;
        public readonly IArray<Workset> WorksetList;
        public readonly IArray<AssemblyInstance> AssemblyInstanceList;
        public readonly IArray<Group> GroupList;
        public readonly IArray<DesignOption> DesignOptionList;
        public readonly IArray<Level> LevelList;
        public readonly IArray<Phase> PhaseList;
        public readonly IArray<Room> RoomList;
        public readonly IArray<Model> ModelList;
        public readonly IArray<Category> CategoryList;
        public readonly IArray<Face> FaceList;
        public readonly IArray<Family> FamilyList;
        public readonly IArray<FamilyType> FamilyTypeList;
        public readonly IArray<FamilyInstance> FamilyInstanceList;
        public readonly IArray<View> ViewList;
        public readonly IArray<Camera> CameraList;
        public readonly IArray<Material> MaterialList;
        public readonly IArray<Node> NodeList;
        public readonly IArray<Geometry> GeometryList;
        
        // Entity properties
        public DictionaryOfLists<int, Property> ElementPropertyLists => ElementEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> WorksetPropertyLists => WorksetEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> AssemblyInstancePropertyLists => AssemblyInstanceEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> GroupPropertyLists => GroupEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> DesignOptionPropertyLists => DesignOptionEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> LevelPropertyLists => LevelEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> PhasePropertyLists => PhaseEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> RoomPropertyLists => RoomEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> ModelPropertyLists => ModelEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> CategoryPropertyLists => CategoryEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> FacePropertyLists => FaceEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> FamilyPropertyLists => FamilyEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> FamilyTypePropertyLists => FamilyTypeEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> FamilyInstancePropertyLists => FamilyInstanceEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> ViewPropertyLists => ViewEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> CameraPropertyLists => CameraEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> MaterialPropertyLists => MaterialEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> NodePropertyLists => NodeEntityTable.PropertyLists;
        public DictionaryOfLists<int, Property> GeometryPropertyLists => GeometryEntityTable.PropertyLists;
        
        public DocumentModel(Document d)
        {
            Document = d;
            // Initialize entity collections
            ElementList = NumElement.Select(i => GetElement(i));
            WorksetList = NumWorkset.Select(i => GetWorkset(i));
            AssemblyInstanceList = NumAssemblyInstance.Select(i => GetAssemblyInstance(i));
            GroupList = NumGroup.Select(i => GetGroup(i));
            DesignOptionList = NumDesignOption.Select(i => GetDesignOption(i));
            LevelList = NumLevel.Select(i => GetLevel(i));
            PhaseList = NumPhase.Select(i => GetPhase(i));
            RoomList = NumRoom.Select(i => GetRoom(i));
            ModelList = NumModel.Select(i => GetModel(i));
            CategoryList = NumCategory.Select(i => GetCategory(i));
            FaceList = NumFace.Select(i => GetFace(i));
            FamilyList = NumFamily.Select(i => GetFamily(i));
            FamilyTypeList = NumFamilyType.Select(i => GetFamilyType(i));
            FamilyInstanceList = NumFamilyInstance.Select(i => GetFamilyInstance(i));
            ViewList = NumView.Select(i => GetView(i));
            CameraList = NumCamera.Select(i => GetCamera(i));
            MaterialList = NumMaterial.Select(i => GetMaterial(i));
            NodeList = NumNode.Select(i => GetNode(i));
            GeometryList = NumGeometry.Select(i => GetGeometry(i));
            
        }
    } // Document class
    public static class DocumentBuilderExtensions
    {
        public static TableBuilder ToTableBuilder(this IEnumerable<Entity> entities)
        {
            var first = entities.FirstOrDefault();
            if (first == null) return null;
            if (first is Element) return entities.Cast<Element>().ToTableBuilder();
            if (first is Workset) return entities.Cast<Workset>().ToTableBuilder();
            if (first is AssemblyInstance) return entities.Cast<AssemblyInstance>().ToTableBuilder();
            if (first is Group) return entities.Cast<Group>().ToTableBuilder();
            if (first is DesignOption) return entities.Cast<DesignOption>().ToTableBuilder();
            if (first is Level) return entities.Cast<Level>().ToTableBuilder();
            if (first is Phase) return entities.Cast<Phase>().ToTableBuilder();
            if (first is Room) return entities.Cast<Room>().ToTableBuilder();
            if (first is Model) return entities.Cast<Model>().ToTableBuilder();
            if (first is Category) return entities.Cast<Category>().ToTableBuilder();
            if (first is Face) return entities.Cast<Face>().ToTableBuilder();
            if (first is Family) return entities.Cast<Family>().ToTableBuilder();
            if (first is FamilyType) return entities.Cast<FamilyType>().ToTableBuilder();
            if (first is FamilyInstance) return entities.Cast<FamilyInstance>().ToTableBuilder();
            if (first is View) return entities.Cast<View>().ToTableBuilder();
            if (first is Camera) return entities.Cast<Camera>().ToTableBuilder();
            if (first is Material) return entities.Cast<Material>().ToTableBuilder();
            if (first is Node) return entities.Cast<Node>().ToTableBuilder();
            if (first is Geometry) return entities.Cast<Geometry>().ToTableBuilder();
            throw new Exception($"Could not generate a TableBuilder for {first.GetType()}");
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Element> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Element");
            tb.AddColumn("Id", typedEntities.Select(x => x.Id));
            tb.AddColumn("Type", typedEntities.Select(x => x.Type));
            tb.AddColumn("Name", typedEntities.Select(x => x.Name));
            tb.AddColumn("Location", typedEntities.Select(x => x.Location));
            tb.AddColumn("FamilyName", typedEntities.Select(x => x.FamilyName));
            tb.AddIndexColumn("Rvt.Level", "Level", typedEntities.Select(x => x._Level.Index));
            tb.AddIndexColumn("Rvt.Phase", "Phase", typedEntities.Select(x => x._Phase.Index));
            tb.AddIndexColumn("Rvt.Category", "Category", typedEntities.Select(x => x._Category.Index));
            tb.AddIndexColumn("Rvt.Workset", "Workset", typedEntities.Select(x => x._Workset.Index));
            tb.AddIndexColumn("Rvt.DesignOption", "DesignOption", typedEntities.Select(x => x._DesignOption.Index));
            tb.AddIndexColumn("Rvt.View", "OwnerView", typedEntities.Select(x => x._OwnerView.Index));
            tb.AddIndexColumn("Rvt.Group", "Group", typedEntities.Select(x => x._Group.Index));
            tb.AddIndexColumn("Rvt.AssemblyInstance", "AssemblyInstance", typedEntities.Select(x => x._AssemblyInstance.Index));
            tb.AddIndexColumn("Rvt.Model", "Model", typedEntities.Select(x => x._Model.Index));
            tb.AddIndexColumn("Rvt.Room", "Room", typedEntities.Select(x => x._Room.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Workset> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Workset");
            tb.AddColumn("Kind", typedEntities.Select(x => x.Kind));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<AssemblyInstance> typedEntities)
        {
            var tb = new TableBuilder("Rvt.AssemblyInstance");
            tb.AddColumn("AssemblyTypeName", typedEntities.Select(x => x.AssemblyTypeName));
            tb.AddColumn("Position", typedEntities.Select(x => x.Position));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Group> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Group");
            tb.AddColumn("GroupType", typedEntities.Select(x => x.GroupType));
            tb.AddColumn("Position", typedEntities.Select(x => x.Position));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<DesignOption> typedEntities)
        {
            var tb = new TableBuilder("Rvt.DesignOption");
            tb.AddColumn("IsPrimary", typedEntities.Select(x => x.IsPrimary));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Level> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Level");
            tb.AddColumn("Elevation", typedEntities.Select(x => x.Elevation));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Phase> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Phase");
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Room> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Room");
            tb.AddColumn("BaseOffset", typedEntities.Select(x => x.BaseOffset));
            tb.AddColumn("LimitOffset", typedEntities.Select(x => x.LimitOffset));
            tb.AddColumn("UnboundedHeight", typedEntities.Select(x => x.UnboundedHeight));
            tb.AddColumn("Volume", typedEntities.Select(x => x.Volume));
            tb.AddColumn("Perimeter", typedEntities.Select(x => x.Perimeter));
            tb.AddColumn("Area", typedEntities.Select(x => x.Area));
            tb.AddColumn("Number", typedEntities.Select(x => x.Number));
            tb.AddIndexColumn("Rvt.Level", "UpperLimit", typedEntities.Select(x => x._UpperLimit.Index));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Model> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Model");
            tb.AddColumn("Title", typedEntities.Select(x => x.Title));
            tb.AddColumn("IsMetric", typedEntities.Select(x => x.IsMetric));
            tb.AddColumn("Guid", typedEntities.Select(x => x.Guid));
            tb.AddColumn("NumSaves", typedEntities.Select(x => x.NumSaves));
            tb.AddColumn("IsLinked", typedEntities.Select(x => x.IsLinked));
            tb.AddColumn("IsDetached", typedEntities.Select(x => x.IsDetached));
            tb.AddColumn("IsWorkshared", typedEntities.Select(x => x.IsWorkshared));
            tb.AddColumn("PathName", typedEntities.Select(x => x.PathName));
            tb.AddColumn("Latitude", typedEntities.Select(x => x.Latitude));
            tb.AddColumn("Longitude", typedEntities.Select(x => x.Longitude));
            tb.AddColumn("TimeZone", typedEntities.Select(x => x.TimeZone));
            tb.AddColumn("PlaceName", typedEntities.Select(x => x.PlaceName));
            tb.AddColumn("WeatherStationName", typedEntities.Select(x => x.WeatherStationName));
            tb.AddColumn("Elevation", typedEntities.Select(x => x.Elevation));
            tb.AddColumn("ProjectLocation", typedEntities.Select(x => x.ProjectLocation));
            tb.AddColumn("IssueDate", typedEntities.Select(x => x.IssueDate));
            tb.AddColumn("Status", typedEntities.Select(x => x.Status));
            tb.AddColumn("ClientName", typedEntities.Select(x => x.ClientName));
            tb.AddColumn("Address", typedEntities.Select(x => x.Address));
            tb.AddColumn("Name", typedEntities.Select(x => x.Name));
            tb.AddColumn("Number", typedEntities.Select(x => x.Number));
            tb.AddColumn("Author", typedEntities.Select(x => x.Author));
            tb.AddColumn("BuildingName", typedEntities.Select(x => x.BuildingName));
            tb.AddColumn("OrganizationName", typedEntities.Select(x => x.OrganizationName));
            tb.AddColumn("OrganizationDescription", typedEntities.Select(x => x.OrganizationDescription));
            tb.AddColumn("Product", typedEntities.Select(x => x.Product));
            tb.AddColumn("Version", typedEntities.Select(x => x.Version));
            tb.AddColumn("User", typedEntities.Select(x => x.User));
            tb.AddIndexColumn("Rvt.View", "ActiveView", typedEntities.Select(x => x._ActiveView.Index));
            tb.AddIndexColumn("Rvt.Family", "OwnerFamily", typedEntities.Select(x => x._OwnerFamily.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Category> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Category");
            tb.AddColumn("Name", typedEntities.Select(x => x.Name));
            tb.AddColumn("Id", typedEntities.Select(x => x.Id));
            tb.AddColumn("CategoryType", typedEntities.Select(x => x.CategoryType));
            tb.AddColumn("LineColor", typedEntities.Select(x => x.LineColor));
            tb.AddIndexColumn("Rvt.Category", "Parent", typedEntities.Select(x => x._Parent.Index));
            tb.AddIndexColumn("Rvt.Material", "Material", typedEntities.Select(x => x._Material.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Face> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Face");
            tb.AddColumn("Type", typedEntities.Select(x => x.Type));
            tb.AddColumn("PeriodU", typedEntities.Select(x => x.PeriodU));
            tb.AddColumn("PeriodV", typedEntities.Select(x => x.PeriodV));
            tb.AddColumn("Radius1", typedEntities.Select(x => x.Radius1));
            tb.AddColumn("Radius2", typedEntities.Select(x => x.Radius2));
            tb.AddColumn("MinU", typedEntities.Select(x => x.MinU));
            tb.AddColumn("MinV", typedEntities.Select(x => x.MinV));
            tb.AddColumn("MaxU", typedEntities.Select(x => x.MaxU));
            tb.AddColumn("MaxV", typedEntities.Select(x => x.MaxV));
            tb.AddColumn("Area", typedEntities.Select(x => x.Area));
            tb.AddIndexColumn("Rvt.Material", "Material", typedEntities.Select(x => x._Material.Index));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Family> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Family");
            tb.AddColumn("StructuralMaterialType", typedEntities.Select(x => x.StructuralMaterialType));
            tb.AddColumn("StructuralSectionShape", typedEntities.Select(x => x.StructuralSectionShape));
            tb.AddIndexColumn("Rvt.Category", "FamilyCategory", typedEntities.Select(x => x._FamilyCategory.Index));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<FamilyType> typedEntities)
        {
            var tb = new TableBuilder("Rvt.FamilyType");
            tb.AddIndexColumn("Rvt.Family", "Family", typedEntities.Select(x => x._Family.Index));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<FamilyInstance> typedEntities)
        {
            var tb = new TableBuilder("Rvt.FamilyInstance");
            tb.AddColumn("FacingFlipped", typedEntities.Select(x => x.FacingFlipped));
            tb.AddColumn("FacingOrientation", typedEntities.Select(x => x.FacingOrientation));
            tb.AddColumn("HandFlipped", typedEntities.Select(x => x.HandFlipped));
            tb.AddColumn("Mirrored", typedEntities.Select(x => x.Mirrored));
            tb.AddColumn("HasModifiedGeometry", typedEntities.Select(x => x.HasModifiedGeometry));
            tb.AddColumn("Scale", typedEntities.Select(x => x.Scale));
            tb.AddColumn("BasisX", typedEntities.Select(x => x.BasisX));
            tb.AddColumn("BasisY", typedEntities.Select(x => x.BasisY));
            tb.AddColumn("BasisZ", typedEntities.Select(x => x.BasisZ));
            tb.AddColumn("Translation", typedEntities.Select(x => x.Translation));
            tb.AddColumn("HandOrientation", typedEntities.Select(x => x.HandOrientation));
            tb.AddIndexColumn("Rvt.FamilyType", "FamilyType", typedEntities.Select(x => x._FamilyType.Index));
            tb.AddIndexColumn("Rvt.Element", "Host", typedEntities.Select(x => x._Host.Index));
            tb.AddIndexColumn("Rvt.Room", "FromRoom", typedEntities.Select(x => x._FromRoom.Index));
            tb.AddIndexColumn("Rvt.Room", "ToRoom", typedEntities.Select(x => x._ToRoom.Index));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<View> typedEntities)
        {
            var tb = new TableBuilder("Rvt.View");
            tb.AddColumn("Title", typedEntities.Select(x => x.Title));
            tb.AddColumn("Up", typedEntities.Select(x => x.Up));
            tb.AddColumn("Right", typedEntities.Select(x => x.Right));
            tb.AddColumn("Origin", typedEntities.Select(x => x.Origin));
            tb.AddColumn("ViewDirection", typedEntities.Select(x => x.ViewDirection));
            tb.AddColumn("ViewPosition", typedEntities.Select(x => x.ViewPosition));
            tb.AddIndexColumn("Rvt.Camera", "Camera", typedEntities.Select(x => x._Camera.Index));
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Camera> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Camera");
            tb.AddColumn("Id", typedEntities.Select(x => x.Id));
            tb.AddColumn("IsPerspective", typedEntities.Select(x => x.IsPerspective));
            tb.AddColumn("VerticalExtent", typedEntities.Select(x => x.VerticalExtent));
            tb.AddColumn("HorizontalExtent", typedEntities.Select(x => x.HorizontalExtent));
            tb.AddColumn("FarDistance", typedEntities.Select(x => x.FarDistance));
            tb.AddColumn("NearDistance", typedEntities.Select(x => x.NearDistance));
            tb.AddColumn("TargetDistance", typedEntities.Select(x => x.TargetDistance));
            tb.AddColumn("RightOffset", typedEntities.Select(x => x.RightOffset));
            tb.AddColumn("UpOffset", typedEntities.Select(x => x.UpOffset));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Material> typedEntities)
        {
            var tb = new TableBuilder("Rvt.Material");
            tb.AddColumn("Id", typedEntities.Select(x => x.Id));
            tb.AddColumn("Name", typedEntities.Select(x => x.Name));
            tb.AddColumn("MaterialCategory", typedEntities.Select(x => x.MaterialCategory));
            tb.AddColumn("Color", typedEntities.Select(x => x.Color));
            tb.AddColumn("ColorTextureFile", typedEntities.Select(x => x.ColorTextureFile));
            tb.AddColumn("ColorUvScaling", typedEntities.Select(x => x.ColorUvScaling));
            tb.AddColumn("ColorUvOffset", typedEntities.Select(x => x.ColorUvOffset));
            tb.AddColumn("Glossiness", typedEntities.Select(x => x.Glossiness));
            tb.AddColumn("NormalAmount", typedEntities.Select(x => x.NormalAmount));
            tb.AddColumn("NormalTextureFile", typedEntities.Select(x => x.NormalTextureFile));
            tb.AddColumn("NormalUvScaling", typedEntities.Select(x => x.NormalUvScaling));
            tb.AddColumn("NormalUvOffset", typedEntities.Select(x => x.NormalUvOffset));
            tb.AddColumn("Smoothness", typedEntities.Select(x => x.Smoothness));
            tb.AddColumn("Transparency", typedEntities.Select(x => x.Transparency));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Node> typedEntities)
        {
            var tb = new TableBuilder("Vim.Node");
            tb.AddIndexColumn("Rvt.Element", "Element", typedEntities.Select(x => x._Element.Index));
            return tb;
        }
        public static TableBuilder ToTableBuilder(this IEnumerable<Geometry> typedEntities)
        {
            var tb = new TableBuilder("Vim.Geometry");
            tb.AddColumn("Box", typedEntities.Select(x => x.Box));
            tb.AddColumn("VertexCount", typedEntities.Select(x => x.VertexCount));
            tb.AddColumn("FaceCount", typedEntities.Select(x => x.FaceCount));
            return tb;
        }
    } // DocumentBuilderExtensions
} // namespace
