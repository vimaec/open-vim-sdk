using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vim.BFast;
using Vim.DataFormat;
using Vim.DotNetUtilities;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.ObjectModel
{
    public static class ObjectModelExtensions
    {
        public static FamilyType GetFamilyType(this FamilyInstance fi)
            => fi?.FamilyType;

        public static string GetFamilyTypeName(this FamilyInstance fi)
            => fi?.FamilyType?.Element?.Name ?? "";

        public static Family GetFamily(this FamilyType ft)
            => ft?.Family;

        public static Family GetFamily(this FamilyInstance fi)
            => fi?.GetFamilyType()?.GetFamily();

        public static string GetFamilyName(this FamilyInstance fi)
            => fi?.GetFamily()?.Element?.Name ?? "";

        public static INamedBuffer GetColumn(this EntityTable et, string name)
        {
            if (et.NumericColumns.Contains(name))
                return et.NumericColumns[name];
            if (et.StringColumns.Contains(name))
                return et.StringColumns[name];
            if (et.IndexColumns.Contains(name))
                return et.IndexColumns[name];
            return null;
        }
            
        /// <summary>
        /// Returns the column data as an IArray.
        /// If the column is null, returns null.
        /// </summary>
        public static IArray<T> GetColumnData<T>(this Document doc, INamedBuffer ec)
        {
            if (ec == null)
                return null; 

            var type = typeof(T);
            if (ec is NamedBuffer<double> doubles)
            {
                var vals = doubles.AsArray<double>().ToIArray();
                if (type == typeof(double))
                    return vals as IArray<T>;
                if (type == typeof(float))
                    return vals.Select(v => (float)v) as IArray<T>;
                if (type == typeof(int))
                    return vals.Select(v => (int)v) as IArray<T>;
                if (type == typeof(byte))
                    return vals.Select(v => (byte)v) as IArray<T>;
                if (type == typeof(bool))
                    return vals.Select(v => v != 0) as IArray<T>;

                throw new Exception($"Cannot cast doubles to {type}");
            }

            if (ec is NamedBuffer<int> strings && type == typeof(string))
            {
                var vals = strings.AsArray<int>().ToIArray();
                if (type == typeof(string))
                    return vals.Select(doc.GetString) as IArray<T>;

                throw new Exception($"Cannot cast strings to {type}");
            }

            if (ec is NamedBuffer<int> ints)
            {
                var vals = ints.AsArray<int>().ToIArray();
                if (type == typeof(int))
                    return vals as IArray<T>;

                throw new Exception("Can only get integer indexes from an integer entity column");
            }

            throw new Exception($"Unrecognized column type {ec.GetType()}");
        }

        /// <summary>
        /// Returns an array of column data in the table based on the given names.
        /// If any of the columns are not found, returns null.
        /// </summary>
        public static IArray<T>[] GetMultiColumnData<T>(this Document doc, EntityTable table, params string[] names)
        {
            var columns = new IArray<T>[names.Length];

            for (var i = 0; i < columns.Length; ++i)
            {
                var column = doc.GetColumnData<T>(table, names[i]);
                if (column == null) { return null; }

                columns[i] = column;
            }

            return columns;
        }

        /// <summary>
        /// Returns the column data based on the given name.
        /// If the column is not found, returns null.
        /// </summary>
        public static IArray<T> GetColumnData<T>(this Document doc, EntityTable table, string name)
        {
            if (table == null)
                return null;

            var type = typeof(T);
            if (type == typeof(DVector2))
            {
                var mcd = doc.GetMultiColumnData<double>(table, $"{name}.X", $"{name}.Y");
                return mcd?[0].Zip(mcd[1], (x, y) => new DVector2(x, y)) as IArray<T>;
            }

            if (type == typeof(Vector2))
            {
                var mcd = doc.GetMultiColumnData<float>(table, $"{name}.X", $"{name}.Y");
                return mcd?[0].Zip(mcd[1], (x, y) => new Vector2(x, y)) as IArray<T>;
            }

            if (type == typeof(DVector3))
            {
                var mcd = doc.GetMultiColumnData<double>(table, $"{name}.X", $"{name}.Y", $"{name}.Z");
                return mcd?[0].Zip(mcd[1], mcd[2], (x, y, z) => new DVector3(x, y, z)) as IArray<T>;
            }

            if (type == typeof(Vector3))
            {
                var mcd = doc.GetMultiColumnData<float>(table, $"{name}.X", $"{name}.Y", $"{name}.Z");
                return mcd?[0].Zip(mcd[1], mcd[2], (x, y, z) => new Vector3(x, y, z)) as IArray<T>;
            }

            if (type == typeof(DVector4))
            {
                var mcd = doc.GetMultiColumnData<double>(table, $"{name}.X", $"{name}.Y", $"{name}.Z", $"{name}.Z");
                return mcd?[0].Zip(mcd[1], mcd[2], mcd[3], (x, y, z, w) => new DVector4(x, y, z, w)) as IArray<T>;
            }

            if (type == typeof(Vector4))
            {
                var mcd = doc.GetMultiColumnData<float>(table, $"{name}.X", $"{name}.Y", $"{name}.Z", $"{name}.Z");
                return mcd?[0].Zip(mcd[1], mcd[2], mcd[3], (x, y, z, w) => new Vector4(x, y, z, w)) as IArray<T>;
            }

            if (type == typeof(DAABox))
            {
                var mcd = doc.GetMultiColumnData<DVector3>(table, $"{name}.Min", $"{name}.Max");
                return mcd?[0].Zip(mcd[1], (x, y) => new DAABox(x, y)) as IArray<T>;
            }

            if (type == typeof(AABox))
            {
                var mcd = doc.GetMultiColumnData<Vector3>(table, $"{name}.Min", $"{name}.Max");
                return mcd?[0].Zip(mcd[1], (x, y) => new AABox(x, y)) as IArray<T>;
            }

            return doc.GetColumnData<T>(table?.GetColumn(name));
        }
    }
}
