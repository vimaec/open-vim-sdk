using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Vim.BFast;
using Vim.DotNetUtilities;
using Vim.Math3d;

namespace Vim.DataFormat
{
    /// <summary>
    /// Entities can have 0 or more properties. They are stored as a flat unordered list.
    /// A property maps a property name/value pair, each represented by indices into the string table, to a particular 
    /// entity id.     
    /// </summary>
    public struct SerializableProperty
    {
        /// <summary>
        /// The index of the entity that the property is matched to 
        /// </summary>
        public int EntityIndex;

        /// <summary>
        /// The string index of the property name
        /// </summary>
        public int Name;

        /// <summary>
        /// The string index of the property value
        /// </summary>
        public int Value;

        public override bool Equals(object obj)
            => obj is SerializableProperty sp && Equals(sp);

        public bool Equals(SerializableProperty other)
            => EntityIndex == other.EntityIndex && Name == other.Name && Value == other.Value;

        public override int GetHashCode()
            => throw new NotImplementedException();

        public static bool operator ==(SerializableProperty left, SerializableProperty right)
            => left.Equals(right);

        public static bool operator !=(SerializableProperty left, SerializableProperty right)
            => !(left == right);
    }

    /// <summary>
    /// Tracks all of the data for a particular entity type in a conceptual table.     
    /// A column maybe a relation to another entity table (IndexColumn)
    /// a numerical value stored as a double (NumericColumn) or else
    /// it is string data, stored as an index into the global lookup table (StringColumn).
    /// Each entity may have 0 or more properties, which are effective dictionaries of data.
    /// </summary>
    public class SerializableEntityTable
    {
        /// <summary>
        /// Name of 
        /// </summary>
        public string Name;

        /// <summary>
        /// Relation to another entity table. For example surface to element. 
        /// </summary>
        public List<NamedBuffer<int>> IndexColumns = new List<NamedBuffer<int>>();

        /// <summary>
        /// Data encoded as strings in the global string table
        /// </summary>
        public List<NamedBuffer<int>> StringColumns = new List<NamedBuffer<int>>();

        /// <summary>
        /// Numeric data encoded as doubles 
        /// </summary>
        public List<NamedBuffer<double>> NumericColumns = new List<NamedBuffer<double>>();

        /// <summary>
        /// Properties (key / value string pairs) associated with entities. Each entity may have 0, 1 or more 
        /// properties, with any key, so it is not appropriate as relational data. 
        /// </summary>
        public SerializableProperty[] Properties;
    }

    /// <summary>
    /// A node in the scene graph. Size = 76
    /// TODO: this whole thing could probably be in an EntityTable
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SerializableSceneNode
    {
        public const int Size = (3 * 4) + (16 * 4); 

        /// <summary>
        /// The parent node (or -1) if a root node. There should be only one root node. 
        /// </summary>
        public int Parent;

        /// <summary>
        /// The index of the associated geometry. 
        /// </summary>
        public int Geometry;

        /// <summary>
        /// If this node is an instance of another node, this is that index 
        /// TODO: this needs to be blown away. It is too complicated for things to work properly.
        /// </summary>
        public int Instance;

        /// <summary>
        /// 16 floating points numbers representing a 4z4 matrix which is the global space location
        /// </summary>
        public Matrix4x4 Transform;

        public bool Equals(SerializableSceneNode other)
            => Parent == other.Parent && Transform == other.Transform;

        public override bool Equals(object other)
            => other is SerializableSceneNode && Equals((SerializableSceneNode)other);

        public override int GetHashCode()
            => Hash.Combine(Parent.GetHashCode(), Transform.GetHashCode());

        public static SerializableSceneNode DefaultValue
            = new SerializableSceneNode { Instance = -1, Geometry = -1, Parent = -1, Transform = Matrix4x4.Identity };

        public static SerializableSceneNode Default = new SerializableSceneNode
        {
            Geometry = -1,
            Instance = -1,
            Parent = -1,
            Transform = Matrix4x4.Identity,
        };

        public static bool operator ==(SerializableSceneNode left, SerializableSceneNode right)
            => left.Equals(right);

        public static bool operator !=(SerializableSceneNode left, SerializableSceneNode right)
            => !(left == right);
    }

    // TODO: this is temp to minimize churn. Should be replaced with a pseudo-JSON.
    public class SerializableHeader
    {
        public SerializableVersion FileVersion = VimConstants.FileVersion;
        public SerializableVersion ObjectModelVersion = VimConstants.ObjectModelVersion;

        public override string ToString()
            => $"vim:{FileVersion}:objectmodel:{ObjectModelVersion}";

        public static SerializableHeader Parse(string input)
        {
            var comps = input.Split(':');
            if (comps.Length != 4) throw new Exception("Expected header to have four parts: " + input);
            if (comps[0] != "vim") throw new Exception("Expected header to start with `vim`: " + input);
            if (comps[2] != "objectmodel") throw new Exception("Expected header to have object model: " + input);
            return new SerializableHeader
            {
                FileVersion = SerializableVersion.Parse(comps[1]),
                ObjectModelVersion = SerializableVersion.Parse(comps[3])
            };
        }

        public override bool Equals(object obj)
            => obj is SerializableHeader other && ToString() == other.ToString();

        public override int GetHashCode()
            => Hash.Combine(FileVersion.GetHashCode(), ObjectModelVersion.GetHashCode());
    }

    /// <summary>
    /// Controls what parts of the VIM file are loaded
    /// </summary>
    public class LoadOptions
    {
        public bool SkipGeometry = false;
        public bool SkipAssets = false;
    }

    /// <summary>
    /// The low-level representation of a VIM data file.
    /// </summary>
    public class SerializableDocument
    {
        /// <summary>
        /// Controls how the file is read and loaded into memory
        /// </summary>
        public LoadOptions Options = new LoadOptions();

        /// <summary>
        /// A JSON object of information about the file
        /// </summary>
        public SerializableHeader Header = new SerializableHeader();

        /// <summary>
        /// A an array of tables, one for each entity 
        /// </summary>
        public List<SerializableEntityTable> EntityTables = new List<SerializableEntityTable>();

        /// <summary>
        /// Used for looking up property strings and entity string fields by Id
        /// </summary>
        public string[] StringTable = Array.Empty<string>();

        /// <summary>
        /// A list of named buffers each representing a different asset in the file 
        /// </summary>
        public INamedBuffer[] Assets = Array.Empty<INamedBuffer>();

        /// <summary>
        /// The scene graph
        /// </summary>
        public SerializableSceneNode[] Nodes = Array.Empty<SerializableSceneNode>();

        /// <summary>
        /// The uninstanced / untransformed geometry
        /// </summary>
        public G3d.G3D Geometry;

        /// <summary>
        /// The originating file name (if provided)
        /// </summary>
        public string FileName;
    }
}
