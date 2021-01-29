using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vim.BFast;
using Vim.DotNetUtilities;
using Vim.LinqArray;

namespace Vim.DataFormat
{
    public class EntityTable 
    {
        public EntityTable(Document document, SerializableEntityTable entityTable)
        {
            Document = document;
            _EntityTable = entityTable;
            Name = _EntityTable.Name;
            PropertyLists = new DictionaryOfLists<int, Property>();
            foreach (var p in _EntityTable.Properties)
                PropertyLists.Add(p.EntityIndex, new Property(Document, p));
            NumericColumns = LinqArray.LinqArray.ToLookup(_EntityTable.NumericColumns, c => c.Name, c => c);
            // We access our indices using simplified names "Element" rather than what gets serialized "Rvt.Element:Element"
            IndexColumns = LinqArray.LinqArray.ToLookup(_EntityTable.IndexColumns, c => c.Name.SimplifiedName(), c => c);
            StringColumns = LinqArray.LinqArray.ToLookup(_EntityTable.StringColumns, c => c.Name, c => c);
            NumRows = Columns.FirstOrDefault()?.NumElements() ?? 0;

            if (!Columns.All(x => x.NumElements() == NumRows))
            {
                Debug.Fail("All columns in an entity table must be the same length");
            }
        }

        private SerializableEntityTable _EntityTable { get; }
        public Document Document { get; }
        public string Name { get; }
        public int NumRows { get; }
        public LinqArray.ILookup<string, NamedBuffer<double>> NumericColumns { get; }
        public LinqArray.ILookup<string, NamedBuffer<int>> StringColumns { get; }
        public LinqArray.ILookup<string, NamedBuffer<int>> IndexColumns { get; }
        public DictionaryOfLists<int, Property> PropertyLists { get; }
        public IEnumerable<Property> Properties => PropertyLists.SelectMany(kv => kv.Value);
        public IArray<INamedBuffer> Columns
            => NumericColumns.Values.Select(x => (INamedBuffer)x)
                .Concatenate(IndexColumns.Values.Select(x => (INamedBuffer)x))
                .Concatenate(StringColumns.Values.Select(x => (INamedBuffer)x));
    }
}
