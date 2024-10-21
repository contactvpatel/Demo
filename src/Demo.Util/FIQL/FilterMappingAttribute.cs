namespace Demo.Util.FIQL
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterMappingAttribute : Attribute
    {
        public string ColumnName { get; }
        public Type T { get; }
        public FilterMappingAttribute(string columnName)
        {
            ColumnName = columnName;
        }
        public FilterMappingAttribute(Type type)
        {
            T = type;
        }
    }
}