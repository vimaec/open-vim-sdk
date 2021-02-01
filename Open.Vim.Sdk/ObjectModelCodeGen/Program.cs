using System;
using System.IO;
using System.Linq;
using Vim.DotNetUtilities;
using Vim.ObjectModel;

namespace Vim.ObjectModel.CodeGen
{
    public static class Program
    {
        public static CodeBuilder WriteDocumentEntityData(Type t, CodeBuilder cb = null)
        {
            var relationFields = t.GetRelationFields().ToArray();
            var entityFields = t.GetEntityFields().ToArray();

            // Get the entity table
            cb.AppendLine($"public EntityTable {t.Name}EntityTable => Document.GetTable(\"{ObjectModelReflection.GetEntityTableName(t)}\");");
            cb.AppendLine("");

            // Get each non-relationl columns for each element
            foreach (var f in entityFields)
                cb.AppendLine($"public IArray<{f.FieldType.Name}> {t.Name}{f.Name} => Document.GetColumnData<{f.FieldType.Name}>({t.Name}EntityTable, \"{f.Name}\");");

            // Gget each reational column
            foreach (var f in relationFields)
            {
                var relType = ObjectModelReflection.RelationTypeParameter(f.FieldType);
                var relName = f.Name.Substring(1);
                if (!f.Name.StartsWith("_"))
                    throw new Exception("Relation names must start with a leading underscore");
                cb.AppendLine($"public IArray<int> {t.Name}{relName} => Document.GetColumnData<int>({t.Name}EntityTable, \"{relName}\");");
            }

            cb.AppendLine($"public int Num{t.Name} => {t.Name}EntityTable?.NumRows ?? 0;");

            // Element getter function
            cb.AppendLine($"public {t.Name} Get{t.Name}(int n)");
            cb.AppendLine("{");

            // Get the entity retrieval function
            cb.AppendLine("if (n < 0) return null;");
            cb.AppendLine($"var r = new {t.Name}();");
            cb.AppendLine("r.Document = Document;");
            cb.AppendLine("r.Index = n;");
            foreach (var f in entityFields)
            {
                cb.AppendLine($"r.{f.Name} = {t.Name}{f.Name}?[n] ?? default;");
            }
            foreach (var f in relationFields)
            {
                var relType = ObjectModelReflection.RelationTypeParameter(f.FieldType);
                cb.AppendLine($"r.{f.Name} = new Relation<{relType}>({t.Name}{f.Name.Substring(1)}?.ElementAtOrDefault(n, -1) ?? -1, Get{relType.Name});");
            }
            cb.AppendLine($"r.Properties = {t.Name}PropertyLists.GetOrDefault(n) ?? r.Properties;");
            cb.AppendLine("return r;");
            cb.AppendLine("}");

            return cb.AppendLine();
        }

        public static CodeBuilder WriteEntityClass(Type t, CodeBuilder cb = null)
        {
            var relationFields = t.GetRelationFields().ToArray();

            cb = cb ?? new CodeBuilder();
            cb.AppendLine("// AUTO-GENERATED");
            cb.AppendLine($"public partial class {t.Name}").AppendLine("{");
            foreach (var f in relationFields)
            {
                cb.AppendLine($"public {ObjectModelReflection.RelationTypeParameter(f.FieldType)} {f.Name.Substring(1)} => {f.Name}.Value;");
            }
            cb.AppendLine($"public List<Property> Properties = new List<Property>();");
            cb.AppendLine("} // end of class");
            cb.AppendLine();
            return cb;
        }

        public static CodeBuilder WriteDocument(CodeBuilder cb = null)
        {
            cb = cb ?? new CodeBuilder();

            foreach (var et in ObjectModelReflection.GetEntityTypes())
                WriteEntityClass(et, cb);

            cb.AppendLine("public partial class DocumentModel");
            cb.AppendLine("{");

            cb.AppendLine("Document Document;");

            foreach (var et in ObjectModelReflection.GetEntityTypes())
                WriteDocumentEntityData(et, cb);

            // Write field declarations
            cb.AppendLine("// Entity collections");
            foreach (var t in ObjectModelReflection.GetEntityTypes())
                cb.AppendLine($"public readonly IArray<{t.Name}> {t.Name}List;");
            cb.AppendLine();

            // Write property getters for the entity properties 
            cb.AppendLine("// Entity properties");
            foreach (var t in ObjectModelReflection.GetEntityTypes())
                cb.AppendLine($"public DictionaryOfLists<int, Property> {t.Name}PropertyLists => {t.Name}EntityTable.PropertyLists;");
            cb.AppendLine();

            // Write the constructor
            cb.AppendLine("public DocumentModel(Document d)");
            cb.AppendLine("{");
            cb.AppendLine("Document = d;");

            cb.AppendLine($"// Initialize entity collections");
            foreach (var t in ObjectModelReflection.GetEntityTypes())
                cb.AppendLine($"{t.Name}List = Num{t.Name}.Select(i => Get{t.Name}(i));");
            cb.AppendLine();


            cb.AppendLine("}");

            cb.AppendLine("} // Document class");
            return cb;
        }

        public static void WriteDocumentBuilder(CodeBuilder cb)
        {
            cb.AppendLine("public static class DocumentBuilderExtensions");
            cb.AppendLine("{");

            cb.AppendLine($"public static TableBuilder ToTableBuilder(this IEnumerable<Entity> entities)");
            cb.AppendLine("{");
            cb.AppendLine("var first = entities.FirstOrDefault();");
            cb.AppendLine("if (first == null) return null;");
            foreach (var et in ObjectModelReflection.GetEntityTypes())
            {
                cb.AppendLine($"if (first is {et.Name}) return entities.Cast<{et.Name}>().ToTableBuilder();");
            }
            cb.AppendLine("throw new Exception($\"Could not generate a TableBuilder for {first.GetType()}\");");
            cb.AppendLine("}");

            foreach (var et in ObjectModelReflection.GetEntityTypes())
            {
                var entityType = et.Name;
                cb.AppendLine($"public static TableBuilder ToTableBuilder(this IEnumerable<{entityType}> typedEntities)");
                cb.AppendLine("{");

                var tableName = ObjectModelReflection.GetEntityTableName(et);
                cb.AppendLine($"var tb = new TableBuilder(\"{tableName}\");");

                var entityFields = et.GetEntityFields().ToArray();
                foreach (var f in entityFields)
                {
                    var name = f.Name;
                    cb.AppendLine($"tb.AddColumn(\"{name}\", typedEntities.Select(x => x.{name}));");
                }

                var relationFields = et.GetRelationFields().ToArray();
                foreach (var f in relationFields)
                {
                    var name = f.Name.Substring(1);
                    var relType = ObjectModelReflection.RelationTypeParameter(f.FieldType);
                    var relatedTableName = ObjectModelReflection.GetEntityTableName(relType);
                    if (string.IsNullOrEmpty(relatedTableName))
                        throw new Exception($"Could not find related table for type {relType}");
                    cb.AppendLine($"tb.AddIndexColumn(\"{relatedTableName}\", \"{name}\", typedEntities.Select(x => x._{name}.Index));");
                }

                cb.AppendLine("return tb;");
                cb.AppendLine("}");
            }
            cb.AppendLine("} // DocumentBuilderExtensions");
        }

        public static void Main(string[] args)
        {
            var file = args[0];
            var cb = new CodeBuilder();

            cb.AppendLine("// AUTO-GENERATED FILE, DO NOT MODIFY.");
            cb.AppendLine("// ReSharper disable All");
            cb.AppendLine("using System;");
            cb.AppendLine("using System.Collections.Generic;");
            cb.AppendLine("using System.Linq;");
            cb.AppendLine("using Vim.DataFormat;");
            cb.AppendLine("using Vim.Math3d;");
            cb.AppendLine("using Vim.LinqArray;");
            cb.AppendLine("using Vim.DotNetUtilities;");

            cb.AppendLine();

            cb.AppendLine("namespace Vim.ObjectModel {"); 

            WriteDocument(cb);

            WriteDocumentBuilder(cb);

            cb.AppendLine("} // namespace");
            var content = cb.ToString();
            File.WriteAllText(file, content);

        }
    }
}
