using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Util.FIQL
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterMappingAttribute : Attribute
    {
        public string ColumnName { get; }

        public FilterMappingAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    } 
}