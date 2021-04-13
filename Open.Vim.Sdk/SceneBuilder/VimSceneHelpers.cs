using System.Collections.Generic;
using System.Linq;
using Vim.DataFormat;
using Vim.DotNetUtilities;
using Vim.Geometry;
using Vim.LinqArray;
using Vim.Math3d;
using Vim.ObjectModel;

namespace Vim
{
    public static class VimSceneHelpers
    {
        public static string[] DisciplineAndCategories = new[] {
"Mechanical:Air Terminals",
"Architecture:Areas",
"Generic:Assemblies",
"Electrical:Cable Tray Fittings",
"Electrical:Cable Tray Runs",
"Electrical:Cable Trays",
"Architecture:Casework",
"Architecture:Ceiling",
"Architecture:Columns",
"Electrical:Communication Devices",
"Electrical:Conduit Fittings",
"Electrical:Conduit Runs",
"Electrical:Conduits",
"Architecture:Curtain Panels",
"Architecture:Curtain Systems",
"Architecture:Curtain Wall Mullions",
"Electrical:Data Devices",
"Generic:Detail Items",
"Architecture:Doors",
"Mechanical:Duct Accessories",
"Mechanical:Duct Fittings",
"Mechanical:Duct Insulations",
"Mechanical:Duct Linings",
"Mechanical:Duct Placeholders",
"Mechanical:Duct Systems",
"Mechanical:Ducts",
"Electrical:Electrical Circuits",
"Electrical:Electrical Equipment",
"Electrical:Electrical Fixtures",
"Electrical:Fire Alarm Devices",
"Mechanical:Flex Ducts",
"Plumbing:Flex Pipes",
"Architecture:Floors",
"Generic:Generic Models",
"Architecture:Grids",
"Mechanical:HVAC Zones",
"Architecture:Levels",
"Architecture:Lines",
"Electrical:Lighting Devices",
"Electrical:Lighting Fixtures",
"Architecture:Mass",
"Architecture:Materials",
"Mechanical:Mechanical Equipment",
"Mechanical:MEP Fabrication Ductwork",
"Mechanical:MEP Fabrication Hangers",
"Mechanical:MEP Fabrication Containment",
"Mechanical:MEP Fabrication Pipework",
"Electrical:Nurse Call Devices",
"Mechanical:Parts",
"Architecture:Parking",
"Plumbing:Pipe Accessories",
"Plumbing:Pipe Fittings",
"Plumbing:Pipe Insulations",
"Plumbing:Pipe Placeholders",
"Plumbing:Pipes",
"Architecture:Planting",
"Plumbing:Piping Systems",
"Plumbing:Plumbing Fixtures",
"Generic:Project Info",
"Architecture:Railings",
"Architecture:Ramps",
"Generic:Raster Images",
"Architecture:Rooms",
"Architecture:Roofs	",
"Electrical:Security Devices",
"Architecture:Shaft Openings",
"Generic:Sheets",
"Architecture:Spaces",
"Generic:Specialty Equipment",
"Plumbing:Sprinklers",
"Architecture:Stairs",
"Structural:Structural Area Reinforcement",
"Structural:Structural Beam Systems",
"Structural:Structural Columns",
"Structural:Structural Connections",
"Structural:Structural Fabric Areas",
"Structural:Structural Fabric Reinforcement",
"Structural:Structural Foundations",
"Structural:Structural Framing",
"Structural:Structural Path Reinforcement",
"Structural:Structural Rebar",
"Structural:Structural Rebar Couplers",
"Structural:Structural Stiffeners",
"Structural:Structural Trusses",
"Electrical:Switch Systems",
"Electrical:Telephone Devices",
"Architecture:Topography",
"Architecture:Views",
"Architecture:Walls",
"Architecture:Windows",
//IFC Groups
"Generic:OST_GenericModel",
"Architecture:OST_Site",
"Generic:OST_SpecialityEquipment",
"Structural:OST_StructuralFraming",
"Architecture:OST_Walls",
"Architecture:OST_Doors",
"Architecture:OST_Floors",
"Architecture:OST_Ceilings",
"Architecture:OST_Roofs",
"Architecture:OST_Windows",
"Structural:OST_StructuralFoundation",
"Architecture:OST_Stairs",
"Architecture:OST_Columns",
"Structural:OST_StructuralColumns",
"Structural:OST_Rebar",
"Mechanical:OST_MechanicalEquipment",
"Architecture:OST_StairsLandings",
"Architecture:OST_StairsRailing",
"Architecture:OST_Ramps",
"Architecture:OST_CurtaSystem",
"Generic:OST_GenericAnnotation",
"Plumbing:OST_PlumbingFixtures",
"Architecture:OST_Grids",
"Structural:OST_StructConnections",
"Mechanical:OST_Parts",
"Architecture:OST_Furniture",
"Architecture:OST_CurtainWallPanels",
"Architecture:OST_CurtainWallMullions",
"Mechanical:OST_DuctTerminal",
"Mechanical:OST_DuctCurves",
"Electrical:OST_LightingFixtures",
"Plumbing:OST_PipeCurves",
"Plumbing:OST_PipeFitting",
"Plumbing:OST_PipeAccessory",
"Mechanical:OST_DuctFitting",
"Structural:OST_FabricAreas"
        };

        public static bool Is3DView(View view)
            => view?.Element?.Type == "View3D" || view?.Element?.Type == "ViewSection";

        public static Dictionary<string, string> CategoryToDiscipline
            = DisciplineAndCategories.ToDictionary(c => c.Substring(c.IndexOf(':') + 1), c => c.Substring(0, c.IndexOf(':')));

        public static string[] Disciplines
            = CategoryToDiscipline.Values.Distinct().OrderBy(x => x).ToArray();

        public static string[] Categories
            = CategoryToDiscipline.Keys.OrderBy(x => x).ToArray();

        public static string GetDisiplineFromCategory(string category, string defaultDiscipline = "Generic")
            => CategoryToDiscipline.GetOrDefault(category ?? "", defaultDiscipline);

        public static IEnumerable<string> GetCategoriesFromDiscipline(string discipline)
            => CategoryToDiscipline.Where(kv => kv.Value == discipline).Select(kv => kv.Key);

        public static Vector4 ColorRGBA(this Material m)
            => m == null 
                ? new Vector4(0.5f, 0.5f, 0.5f, 1)  
                : new Vector4((float)m.Color.X / 255f,
                    (float)m.Color.Y / 255f,
                    (float)m.Color.Z / 255f,
                    1.0f - (float)m.Transparency);

        public static Vector4 GetDiffuseColor(this Material m)
            => m?.Color.ToDiffuseColor(m.Transparency) ?? DefaultColor;

        public static Vector4 ToDiffuseColor(this DVector3 v, double transparency)
            => new Vector4((float)v.X, (float)v.Y, (float)v.Z, 1.0f - (float)transparency);

        public static Dictionary<int, Vector4> MaterialColorLookup(this VimScene scene)
        {
            var r = scene.Model.MaterialList.ToEnumerable().ToDictionary(m => m.Id, GetDiffuseColor);
            r.Add(-1, DefaultColor);
            return r;
        }

        public static Vector4 DefaultColor = new Vector4(0.5f, 0.5f, 0.5f, 1);

        public static IArray<Vector4> FaceColors(this VimScene scene, IMesh m)
        {
            var colorLookup = scene.MaterialColorLookup();
            return m.FaceMaterialIds.Select(mId => colorLookup.GetOrDefault(mId, DefaultColor));
        }

        public static IArray<Vector4> CalculateVertexColors(this VimScene scene, IMesh m)
            => m.FaceDataToVertexData(scene.FaceColors(m));

        public static VimScene ToVimScene(this SerializableDocument self)
            => new VimScene(self);

        public static bool IsMirrored(this VimSceneNode node)
            => node.Transform.IsReflection;
    }
}
