using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GLS149_SQLtest;

public class WangOrm2
{
    // Method to get the table name from the class
    public static string GetTableName(Type type)
    {
        var tableAttribute = type.GetCustomAttribute<TableAttribute>();
        if (tableAttribute != null)
        {
            // If schema is specified, include it
            return string.IsNullOrEmpty(tableAttribute.Schema)
                ? tableAttribute.Name
                : $"{tableAttribute.Schema}.{tableAttribute.Name}";
        }
        return "";
    }

    public static string GetColumnName(Type type, string propertyName)
    {
        // Find the property by name
        var property = type.GetProperty(propertyName);

        if (property != null)
        {
            // Get the Column attribute for the property, if it exists
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

            if (columnAttribute != null)
            {
                if (!string.IsNullOrEmpty(columnAttribute.Name))
                {
                    return columnAttribute.Name;
                }
            }
        }

        // Return null if the property or Column attribute doesn't exist
        return "null";
    }
}
