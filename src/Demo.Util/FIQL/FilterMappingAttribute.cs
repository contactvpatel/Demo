namespace Demo.Util.FIQL
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterMappingAttribute : Attribute
    {
        public string ColumnName { get; }
        public Type T { get; }
        public bool ComputedColumn { get; } = false;
        public FilterMappingAttribute(string columnName)
        {
            ColumnName = columnName;
        }
        public FilterMappingAttribute(Type type)
        {
            T = type;
        }
        public FilterMappingAttribute(bool computedColumn)
        {
            ComputedColumn = computedColumn;
        }
    }
}