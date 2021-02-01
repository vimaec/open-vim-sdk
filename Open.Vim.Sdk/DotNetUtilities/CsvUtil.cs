using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Vim.DotNetUtilities
{
    // https://stackoverflow.com/questions/4959722/c-sharp-datatable-to-csv
    public static class CsvUtil
    {
        public static string EscapeQuotes(this string self)
            => self?.Replace("\"", "\"\"") ?? "";

        public static string Surround(this string self, string before, string after)
            => $"{before}{self}{after}";

        public static string Quoted(this string self, string quotes = "\"")
            => self.Surround(quotes, quotes);

        public static string QuotedCSVFieldIfNecessary(this string self)
            => (self == null) ? "" : self.Contains('"') || self.Contains(',') ? self.Quoted() : self;

        public static string ToCsvField(this string self)
            => self.EscapeQuotes().QuotedCSVFieldIfNecessary();

        public static string ToCsvRow(this IEnumerable<string> self)
            => string.Join(",", self.Select(ToCsvField));

        public static IEnumerable<string> ToCsvRows(this DataTable self)
        {
            yield return self.Columns.OfType<object>().Select(c => c.ToString()).ToCsvRow();
            foreach (var dr in self.Rows.OfType<DataRow>())
                yield return dr.ItemArray.Select(i => ToCsvField(i.ToString())).ToCsvRow();
        }

        public static void ToCsvFile(this DataTable self, string path)
            => File.WriteAllLines(path, self.ToCsvRows());

       public static void PropertiesToCsvFile<T>(this IEnumerable<T> self, string path)
            => self.PropertiesToDataTable().ToCsvFile(path);

        public static void FieldsToCsvFile<T>(this IEnumerable<T> self, string path)
             => self.FieldsToDataTable().ToCsvFile(path);

        public static void FieldsAndPropertiesToCsvFile<T>(this IEnumerable<T> self, string path)
             => self.FieldsAndPropertiesToDataTable().ToCsvFile(path);

        /// <summary>
        /// Creates a data table from an array of classes, using the properties of the classes as column values
        /// https://stackoverflow.com/questions/18746064/using-reflection-to-create-a-datatable-from-a-class
        /// </summary>
        public static DataTable PropertiesToDataTable<T>(this IEnumerable<T> self)
        {
            var properties = typeof(T).GetProperties();

            // create a new data table if needed. Otherwise we are adding to the passed dataTable
            var dataTable = new DataTable();

            foreach (var info in properties)
                dataTable.Columns.Add(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType);

            foreach (var entity in self)
                dataTable.Rows.Add(properties.Select(p => p.GetValue(entity)).ToArray());

            return dataTable;
        }

        /// <summary>
        /// Creates a data table from an array of classes, using the fields of the clases as column values
        /// https://stackoverflow.com/questions/18746064/using-reflection-to-create-a-datatable-from-a-class
        /// </summary>
        public static DataTable FieldsAndPropertiesToDataTable<T>(this IEnumerable<T> self)
        {
            var fields = typeof(T).GetAllFields();
            var properties = typeof(T).GetProperties();

            // create a new data table if needed. Otherwise we are adding to the passed dataTable
            var dataTable = new DataTable();

            foreach (var info in fields)
                dataTable.Columns.Add(info.Name, Nullable.GetUnderlyingType(info.FieldType) ?? info.FieldType);

            foreach (var info in properties)
                dataTable.Columns.Add(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType);

            foreach (var entity in self)
                dataTable.Rows.Add(
                    fields
                        .Select(p => p.GetValue(entity))
                        .Concat(properties.Select(p => p.GetValue(entity)))
                        .ToArray());


            return dataTable;
        }

        /// <summary>
        /// Creates a data table from an array of classes, using the fields of the clases as column values
        /// https://stackoverflow.com/questions/18746064/using-reflection-to-create-a-datatable-from-a-class
        /// </summary>
        public static DataTable FieldsToDataTable<T>(this IEnumerable<T> self)
        {
            var fields = typeof(T).GetAllFields();

            // create a new data table if needed. Otherwise we are adding to the passed dataTable
            var dataTable = new DataTable();

            foreach (var info in fields)
                dataTable.Columns.Add(info.Name, Nullable.GetUnderlyingType(info.FieldType) ?? info.FieldType);

            foreach (var entity in self)
                dataTable.Rows.Add(fields.Select(p => p.GetValue(entity)).ToArray());

            return dataTable;
        }

        public static string PrettyPrint(this DataTable self, string sep = "\t")
        {
            var sb = new StringBuilder();
            foreach (DataColumn c in self.Columns)
                sb.Append(c.ColumnName).Append(sep);
            sb.AppendLine();
            foreach (DataRow r in self.Rows)
                sb.AppendLine(string.Join(sep, r.ItemArray));
            return sb.ToString();
        }

        /// <summary>
        /// Given a data table, computes stats for every column in the data table that can be converted into numbers.
        /// </summary>
        public static Dictionary<string, Statistics> ColumnStatistics(this DataTable self)
            => self.Columns.OfType<DataColumn>().Where(IsNumericColumn).ToDictionary(
                dc => dc.ColumnName,
                dc => dc.ColumnValues<double>().Statistics());

        /// <summary>
        /// Adds a new column to the datatable with the specific name, position, and values.
        /// </summary>
        public static DataTable AddColumn<T>(this DataTable self, string name, IEnumerable<T> values, int position = -1)
        {
            var dc = self.Columns.Add(name, typeof(T));
            if (position >= 0)
                dc.SetOrdinal(position);
            position = dc.Ordinal;
            var i = 0;
            foreach (var v in values)
            {
                if (self.Rows.Count <= i)
                    self.Rows.Add(self.NewRow());
                self.Rows[i++][position] = v;
            }
            return self;
        }

        /// <summary>
        /// Adds a new autoincrement column to an exislist of primary columns that is appropriate for auto-incrementing. 
        /// </summary>
        public static DataColumn AddAutoIncrementPrimaryKeyColumn(this DataTable self)
        {
            var existingKeys = self.PrimaryKey ?? Array.Empty<DataColumn>();
            var r = new DataColumn();
            r.DataType = typeof(long);
            r.AutoIncrement = true;
            r.AutoIncrementSeed = 0;
            r.AutoIncrementStep = 1;            
            self.Columns.Add(r);
            self.PrimaryKey = existingKeys.Append(r).ToArray();
            return r;
        }

        /// <summary>
        /// Returns true if a data column can be cast to doubles.
        /// </summary>
        public static bool IsNumericColumn(this DataColumn dc)
            => dc.DataType.CanCastToDouble();

        /// <summary>
        /// Generates a DataTable from all of the dictionaries provided 
        /// </summary>
        public static DataTable DictionariesToDataTable(this IEnumerable<IDictionary<string, string>> parameters)
        {
            var parameterNames = new IndexedSet<string>();

            var dataTable = new DataTable();

            foreach (var d in parameters)
                foreach (var kv in d)
                    parameterNames.GetOrAdd(kv.Key);

            foreach (var key in parameterNames.Keys)
                dataTable.Columns.Add(key);

            foreach (var d in parameters)
                dataTable.Rows.Add(parameterNames.Keys.Select(d.GetOrDefault).ToArray());

            return dataTable;
        }

        /// <summary>
        /// Generates a CSV from all of the dictionaries provided
        /// </summary>
        public static void DictionariesToCsvFile(this IEnumerable<IDictionary<string, string>> dictionaries, string filePath)
            => dictionaries.DictionariesToDataTable().ToCsvFile(filePath);

    }
}
