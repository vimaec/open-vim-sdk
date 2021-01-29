using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.DataFormat
{
    public class VimSchema
    {
        public enum ColumnType
        {
            Numeric,
            String,
            Index,
        }

        public string Version { get; set; }
        public List<Table> Tables { get; set; } = new List<Table>();

        public Table FindTable(string tableName)
            => Tables.Where(t => t.Name == tableName).FirstOrDefault();

        public class Table
        {
            public string Name { get; set; }
            public List<Column> Columns { get; set; } = new List<Column>();

            public Column FindColumn(string columnName)
                => Columns.Where(c => c.Name == columnName).FirstOrDefault();

            public void AddColumn(string name, ColumnType type)
                => Columns.Add(new Column { Name = name, Type = type.ToString() });

            public void AddColumns(string name, Type t)
            {
                if (t == typeof(int) || t == typeof(bool) || t == typeof(float) || t == typeof(double) || t == typeof(byte) || t == typeof(long) || t == typeof(short))
                {
                    AddColumn(name, ColumnType.Numeric);
                }
                else if (t == typeof(string))
                {
                    AddColumn(name, ColumnType.String);
                }
                else if (t == typeof(Vector2) || t == typeof(Vector3) || t == typeof(Vector4) || t == typeof(AABox) || t == typeof(AABox2D)
                    || t == typeof(DVector2) || t == typeof(DVector3) || t == typeof(DVector4) || t == typeof(DAABox) || t == typeof(DAABox2D))
                {
                    foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
                        AddColumns($"{name}.{f.Name}", f.FieldType);
                }
                else
                {
                    throw new Exception($"Can't construct schema from object model: unrecognized type {t}");
                }
            }

        }

        public class Column
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public Table AddTable(string name)
        {
            if (FindTable(name) != null)
                throw new Exception($"Table {name} already exists");
            var t = new Table { Name = name };
            Tables.Add(t);
            return t;
        }

        public static VimSchema Create(string filePath)
            => Create(Serializer.Deserialize(filePath).ToDocument());

        public static VimSchema Create(Document doc)
        {
            var r = new VimSchema();
            r.Version = doc.Header.ObjectModelVersion.ToString();
            foreach (var et in doc.EntityTables.Values.ToEnumerable())
            {
                var t = new Table() { Name = et.Name.SimplifiedName() };

                r.Tables.Add(t);

                foreach (var c in et.IndexColumns.Values.ToEnumerable())
                    t.AddColumn(c.Name, ColumnType.Index);

                foreach (var c in et.NumericColumns.Values.ToEnumerable())
                    t.AddColumn(c.Name, ColumnType.Numeric);

                foreach (var c in et.StringColumns.Values.ToEnumerable())
                    t.AddColumn(c.Name, ColumnType.String);
            }
            return r;
        }

        public VimSchema AddedOrChanged(VimSchema previous, bool changedOnly)
        {
            var r = new VimSchema();
            r.Version = $"{previous.Version} => {Version}";
            foreach (var t in Tables)
            {
                var previousTable = previous.FindTable(t.Name);
                if (previousTable == null && !changedOnly)
                {
                    r.Tables.Add(t);
                }
                else if (previousTable != null)
                {
                    var newTable = new Table() { Name = t.Name };
                    foreach (var c in t.Columns)
                    {
                        var previousColumn = previousTable.FindColumn(c.Name);
                        if (previousColumn == null && !changedOnly)
                        {
                            newTable.Columns.Add(c);
                        }
                        if (previousColumn != null && changedOnly && c.Type != previousColumn.Type)
                        { 
                            newTable.Columns.Add(c);
                        }
                    }
                    // If there are any new columns
                    if (newTable.Columns.Count > 0)
                        r.Tables.Add(newTable);
                }
            }
            return r;
        }

        public VimSchema Added(VimSchema previous)
            => AddedOrChanged(previous, false);

        public VimSchema Removed(VimSchema previous)
            => previous.Added(this);        

        public VimSchema Modified(VimSchema previous)
            => AddedOrChanged(previous, true);

        public bool IsSame(VimSchema vs)
            => Added(vs).Tables.Count == 0
            && Removed(vs).Tables.Count == 0
            && Modified(vs).Tables.Count == 0;

        public static bool SameSchema(VimSchema vim1, VimSchema vim2)
            => vim1.IsSame(vim2);
    }
}
