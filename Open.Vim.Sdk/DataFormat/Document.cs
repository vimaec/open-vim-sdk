using Vim.LinqArray;
using Vim.BFast;
using Vim.DotNetUtilities;

namespace Vim.DataFormat
{
    // TODO: this should be merged into Serializable document. 
    public class Document
    {
        public Document(SerializableDocument document)
        {
            _Document = document;
            Header = _Document.Header;
            Nodes = _Document.Nodes.ToIArray();
            Geometry = _Document.Geometry;
            StringTable = _Document.StringTable.ToIArray();
            EntityTables = _Document.EntityTables.ToLookup(
                et => DocumentExtensions.GetTableKeyFromTableName(et.Name),
                et => et.ToEntityTable(this));
            Assets = _Document.Assets.ToLookup(et => et.Name, et => et);
        }

        private SerializableDocument _Document { get; }
        public SerializableHeader Header { get; }
        public ILookup<string, EntityTable> EntityTables { get; }
        public ILookup<string, INamedBuffer> Assets { get; }
        public IArray<string> StringTable { get; }
        public IArray<SerializableSceneNode> Nodes { get; }
        public string GetString(int index) => StringTable.ElementAtOrDefault(index);
        public G3d.G3D Geometry { get; }
    }

    public class Property
    {
        public Document Document;
        public SerializableProperty _Property;

        public Property(Document document, SerializableProperty prop)
        {
            Document = document;
            _Property = prop;
        }

        public int Id => _Property.EntityIndex;
        public string Name => Document.GetString(_Property.Name);
        public string Value => Document.GetString(_Property.Value);
    }
}
