using System;
using System.Collections.Generic;
using System.Linq;
using Vim.LinqArray;
using Vim.BFast;
using Vim.DotNetUtilities;
using Vim.Math3d;
using System.Collections;

namespace Vim.DataFormat
{
    public class TableBuilder
    {
        public readonly string Name;
        public readonly Dictionary<string, double[]> NumericColumns = new Dictionary<string, double[]>();
        public readonly Dictionary<string, int[]> IndexColumns = new Dictionary<string, int[]>();
        public readonly Dictionary<string, string[]> StringColumns = new Dictionary<string, string[]>();
        public readonly List<PropertyBuilder> Properties = new List<PropertyBuilder>();
        public int NumRows = 0;

        public TableBuilder(string name)
            => Name = name;

        public TableBuilder UpdateOrValidateRows(int n)
        {
            if (NumRows == 0) NumRows = n;
            else if (NumRows != n) throw new Exception($"Value count {n} does not match the expected number of rows {NumRows}");
            return this;
        }

        public TableBuilder AddIndexColumn(string tableName, string fieldName, int[] ids)
        {
            UpdateOrValidateRows(ids.Length);
            IndexColumns.Add($"{tableName}:{fieldName}", ids);
            return this;
        }

        public TableBuilder AddColumn(string name, string[] values)
        {
            UpdateOrValidateRows(values.Length);
            StringColumns.Add(name, values);
            return this;
        }

        public TableBuilder AddColumn(string name, double[] values)
        {
            UpdateOrValidateRows(values.Length);
            NumericColumns.Add(name, values);
            return this;
        }

        public TableBuilder AddIndexColumn(string tableName, string fieldName, IEnumerable<int> ids)
            => AddIndexColumn(tableName, fieldName, ids.ToArray());

        public TableBuilder AddColumn(string name, IEnumerable<double> values)
            => AddColumn(name, values.ToArray());

        public TableBuilder AddColumn(string name, IEnumerable<string> values)
            => AddColumn(name, values.ToArray());

        public TableBuilder AddColumn(string name, IEnumerable<float> values)
            => AddColumn(name, values.Select(x => (double)x));

        public TableBuilder AddColumn(string name, IEnumerable<int> values)
            => AddColumn(name, values.Select(x => (double)x));

        public TableBuilder AddColumn(string name, IEnumerable<short> values)
            => AddColumn(name, values.Select(x => (double)x));

        public TableBuilder AddColumn(string name, IEnumerable<byte> values)
            => AddColumn(name, values.Select(x => (double)x));

        public TableBuilder AddColumn(string name, IEnumerable<bool> values)
            => AddColumn(name, values.Select(x => x ? 1.0 : 0));

        public TableBuilder AddProperty(int entityId, string name, string value)
        {
            Properties.Add(new PropertyBuilder(entityId, name, value));
            return this;
        }

        public TableBuilder AddColumn(string name, IEnumerable<DVector2> xs)
            => AddColumn($"{name}.X", xs.Select(v => v.X))
               .AddColumn($"{name}.Y", xs.Select(v => v.Y));

        public TableBuilder AddColumn(string name, IEnumerable<Vector2> xs)
            => AddColumn($"{name}.X", xs.Select(v => v.X))
                        .AddColumn($"{name}.Y", xs.Select(v => v.Y));

        public TableBuilder AddColumn(string name, IEnumerable<DVector4> xs)
            => AddColumn($"{name}.X", xs.Select(v => v.X))
                .AddColumn($"{name}.Y", xs.Select(v => v.Y))
                .AddColumn($"{name}.Z", xs.Select(v => v.Z))
                .AddColumn($"{name}.W", xs.Select(v => v.W));

        public TableBuilder AddColumn(string name, IEnumerable<Vector4> xs)
            => AddColumn($"{name}.X", xs.Select(v => v.X))
                .AddColumn($"{name}.Y", xs.Select(v => v.Y))
                .AddColumn($"{name}.Z", xs.Select(v => v.Z))
                .AddColumn($"{name}.W", xs.Select(v => v.W));

        public TableBuilder AddColumn(string name, IEnumerable<DVector3> xs)
            => AddColumn($"{name}.X", xs.Select(v => v.X))
                .AddColumn($"{name}.Y", xs.Select(v => v.Y))
                .AddColumn($"{name}.Z", xs.Select(v => v.Z));

        public TableBuilder AddColumn(string name, IEnumerable<Vector3> xs)
            => AddColumn($"{name}.X", xs.Select(v => v.X))
                .AddColumn($"{name}.Y", xs.Select(v => v.Y))
                .AddColumn($"{name}.Z", xs.Select(v => v.Z));

        public TableBuilder AddColumn(string name, IEnumerable<AABox> xs)
            => AddColumn($"{name}.Min.X", xs.Select(v => v.Min.X))
                .AddColumn($"{name}.Min.Y", xs.Select(v => v.Min.Y))
                .AddColumn($"{name}.Min.Z", xs.Select(v => v.Min.Z))
                .AddColumn($"{name}.Max.X", xs.Select(v => v.Max.X))
                .AddColumn($"{name}.Max.Y", xs.Select(v => v.Max.Y))
                .AddColumn($"{name}.Max.Z", xs.Select(v => v.Max.Z));

        public TableBuilder AddColumn(string name, IEnumerable<DAABox> xs)
            => AddColumn($"{name}.Min.X", xs.Select(v => v.Min.X))
                .AddColumn($"{name}.Min.Y", xs.Select(v => v.Min.Y))
                .AddColumn($"{name}.Min.Z", xs.Select(v => v.Min.Z))
                .AddColumn($"{name}.Max.X", xs.Select(v => v.Max.X))
                .AddColumn($"{name}.Max.Y", xs.Select(v => v.Max.Y))
                .AddColumn($"{name}.Max.Z", xs.Select(v => v.Max.Z));

        public IEnumerable<string> GetAllStrings()
            => StringColumns.Values.SelectMany(sc => sc)
            .Concat(Properties.Select(p => p.Name))
            .Concat(Properties.Select(p => p.Value))
            .Where(x => x != null);

        public void Clear()
        {
            NumRows = 0;
            NumericColumns.Clear();
            StringColumns.Clear();
            Properties.Clear();
            IndexColumns.Clear();
        }
    }
}
