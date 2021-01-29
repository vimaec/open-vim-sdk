using System;
using System.Collections.Generic;
using Vim.DataFormat;
using Vim.DotNetUtilities;

namespace Vim.ObjectModel
{
    public class ObjectModelBuilder
    {
        public Dictionary<Type, IndexedSet<Entity>> EntitiesFromTypes
            = new Dictionary<Type, IndexedSet<Entity>>();

        public int Add<T>(T entity) where T: Entity
            => EntitiesFromTypes.GetOrCompute(typeof(T), t => new IndexedSet<Entity>()).Add(entity);

        public DocumentBuilder AddTablesToDocumentBuilder(DocumentBuilder db)
        {
            foreach (var kv in EntitiesFromTypes)
            {
                var tableName = kv.Key.GetEntityTableName();
                var tb = kv.Value.OrderedKeys.ToTableBuilder();
                db.Tables.Add(tableName, tb);
            }
            return db;
        }
    }
}
