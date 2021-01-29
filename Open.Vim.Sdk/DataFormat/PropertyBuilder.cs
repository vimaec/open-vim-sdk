namespace Vim.DataFormat
{
    public class PropertyBuilder
    {
        public PropertyBuilder(int entityId, string name, string value)
        {
            EntityId = entityId;
            Name = name;
            Value = value;
        }
        public int EntityId;
        public string Name;
        public string Value;
    }
}
