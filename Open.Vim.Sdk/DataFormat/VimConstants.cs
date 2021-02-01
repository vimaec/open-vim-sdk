using System.Collections.Generic;
using System.Linq;
using Vim.DotNetUtilities;

namespace Vim.DataFormat
{
    public static class TableNames
    {
        public const string Geometry = "Vim.Geometry";
        public const string Node = "Vim.Node";
        public const string Documents = "Vim.Documents";
        public const string Assets = "Vim.Assets";

        public const string Material = "Rvt.Material";
        public const string Family = "Rvt.Family";
        public const string FamilyInstance = "Rvt.FamilyInstance";
        public const string FamilyType = "Rvt.FamilyType";
        public const string Element = "Rvt.Element";
        public const string Face = "Rvt.Face";
        public const string Category = "Rvt.Category";
        public const string Level = "Rvt.Level";
        public const string Phase = "Rvt.Phase";
        public const string Room = "Rvt.Room";
        public const string View = "Rvt.View";
        public const string Camera = "Rvt.Camera";
        public const string Workset = "Rvt.Workset";
        public const string DesignOption = "Rvt.DesignOption";
        public const string AssemblyInstance = "Rvt.AssemblyInstance";
        public const string Group = "Rvt.Group";
        public const string Model = "Rvt.Model";
    }

    public static class VimConstants
    {
        public const string PropertiesBufferName = "properties";
        public const string TableName = "table:";
        public const string IndexColumnNamePrefix = "index:";
        public const string StringColumnNamePrefix = "string:";
        public const string NumberColumnNamePrefix = "numeric:";
        
        public static HashSet<string> ComputedTableNames = new HashSet<string>
        {
            TableNames.Geometry
        };

        public static HashSet<string> NonBimNames = new HashSet<string>
        {
            TableNames.Geometry,
            TableNames.Node,
            TableNames.Documents,
            TableNames.Assets,
            TableNames.Material,
        };

        // Version Notes:

        // initial version (no longer supported)
        public const string V0_9_1_0 = "vim:0.9:objectmodel:1.0";

        // g3d format changed to contain one more token in its URN
        public const string V0_9_2_0 = "vim:0.9:objectmodel:2.0";

        // exported geometry no longer contains baked transforms
        public const string V0_9_3_0_0 = "vim:2.1.0:objectmodel:3.0.0";

        // Revit exporter tessellates faces containing multiple non-intersecting curve loops.
        public const string V0_9_3_0_1 = "vim:0.9:objectmodel:3.0.1";

        // Revit exporter isolates instances to their originating document when linked files are encountered.
        public const string V0_9_3_0_2 = "vim:0.9:objectmodel:3.0.2";

        // Revit exporter tessellates planar faces as quad strips in a more generic manner.
        public const string V0_9_3_0_3 = "vim:0.9:objectmodel:3.0.3";

        // Revit exporter tessellation fix for occasionally flipped normals.
        public const string V0_9_3_0_4 = "vim:0.9:objectmodel:3.0.4";

        // Added fields to the ObjectModel:
        // - Entity (Index, Document)
        // - Face (PeriodU, PeriodV, Radius1, Radius2, MinU, MinV, MaxU, MaxV, Area, TessellationMethod)
        // - FamilyInstance (FacingFlipped, FacingOrientation, HandFlipped, Mirrored, HasModifiedGeometry, Scale, BasisX, BasisY, BasisZ, Translation, HandOrientation)
        public const string V0_9_3_1_0 = "vim:0.9:objectmodel:3.1.0";

        // Revit exporter improvements:
        // - Nodes are exported as a flat list to streamline the deserialization process (no more subtree instancing).
        // - Nodes originating from a reflection operation have their geometry flipped (and their reflection term removed) to streamline the rendering process.
        // - Node instancing is now based on the "stable representation" of the element's geometry.
        // - Improved exception handling for non conformal transforms in Revit.
        public const string V0_9_3_1_1 = "vim:0.9:objectmodel:3.1.1";

        // VIM file serialization no longer makes use of the Span class to overcome the 2GB limit for arrays.
        public const string V0_9_3_2_0 = "vim:0.9:objectmodel:3.2.0";

        // - removed Element.Box
        // - removed Face.TesselationResult
        // - added DesignOption, AssemblyInstance, Workset, Group tables
        // - added several fields to Element 
        public const string V0_9_3_3_0 = "vim:0.9:objectmodel:3.3.0";

        // - added Model table (for Revit linked files aka Revit Document)
        // - removing Link table
        // - fixed bug in missing AssemblyInstance data in Element
        // - view now has correct export 
        public const string V0_9_3_4_0 = "vim:0.9:objectmodel:3.4.0";

        // - added Room field to element 
        // - added several fields to the Model table
        // - element location when represented by a bounding line or curve (instead of point) is computed as a point by averaging the two end points
        public const string V0_9_3_5_0 = "vim:0.9:objectmodel:3.5.0";

        /// <summary>
        /// The list of supported headers. Note: the last item corresponds to the current version (V_CURRENT)
        /// </summary>
        public static readonly IReadOnlyList<string> SupportedHeaders
            = new[]
            {
                V0_9_2_0,
                V0_9_3_0_0,
                V0_9_3_0_1,
                V0_9_3_0_2,
                V0_9_3_0_3,
                V0_9_3_0_4,
                V0_9_3_1_0,
                V0_9_3_1_1,
                V0_9_3_2_0,
                V0_9_3_3_0,
                V0_9_3_4_0,
                V0_9_3_5_0,
            };

        // Current version 
        // ReSharper disable once InconsistentNaming
        public static string V_CURRENT => SupportedHeaders.Last();

        public static readonly SerializableVersion FileVersion;
        public static readonly SerializableVersion ObjectModelVersion;

        /// <summary>
        /// A static initializer which populates the FileVersion and the ObjectModelVersion with the parsed contents of V_CURRENT.
        /// </summary>
        static VimConstants()
        {
            var parsedCurrent = SerializableHeader.Parse(V_CURRENT);
            FileVersion = parsedCurrent.FileVersion;
            ObjectModelVersion = parsedCurrent.ObjectModelVersion;
        }

        /// <summary>
        /// The expected attributes in the Legacy (pre-V1) VIM files
        /// </summary>
        public static class G3dLegacySchema
        {
            public const string Position = "g3d:vertex:position:0:float32:3";
            public const string Index = "g3d:corner:index:0:int32:1";
            public const string Uv = "g3d:vertex:uv:0:float32:2";
            public const string MaterialId = "g3d:face:materialid:0:int32:1";
            public const string GroupId = "g3d:face:groupid:0:int32:1";
            public const string IndexOffset = "g3d:group:indexoffset:0:int32:1";
            public const string VertexOffset = "g3d:group:vertexoffset:0:int32:1";

            public static string[] Attributes = new[]
            {
                Position, Index, Uv, MaterialId, GroupId, IndexOffset, VertexOffset
            };
        };

        /// <summary>
        /// The expected attributes in the V1 VIM file
        /// </summary>
        public static class G3dSchema
        {
            public const string Position = "g3d:vertex:position:0:float32:3";
            public const string Index = "g3d:corner:index:0:int32:1";
            public const string Uv = "g3d:vertex:uv:0:float32:2";
            public const string MaterialId = "g3d:face:materialid:0:int32:1";
            public const string ObjectId = "g3d:face:objectid:0:int32:1";
            public const string IndexOffset = "g3d:subgeo:indexoffset:0:int32:1";
            public const string VertexOffset = "g3d:subgeo:vertexoffset:0:int32:1";
            public const string Transform = "g3d:instance:transform:0:float32:16";
            public const string SubGeometry = "g3d:instance:subgeometry:0:int32:1";

            public static string[] Attributes = new[]
            {
                Position, Index, Uv, MaterialId, ObjectId, IndexOffset, VertexOffset, Transform, SubGeometry
            };
        };

        public static class DisciplineNames
        {
            public const string Mechanical = nameof(Mechanical);
            public const string Architecture = nameof(Architecture);
            public const string Generic = nameof(Generic);
            public const string Electrical = nameof(Electrical);
            public const string Plumbing = nameof(Plumbing);
            public const string Structural = nameof(Structural);
        }
    }
}
