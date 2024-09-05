﻿namespace Demo.Util.FIQL
{
    public static class ConvertFiqlToDapper
    {
        public static FiltersAndProperties FiqlToDapper<T>(string fiql)
        {
            FiltersAndProperties filtersAndProperties = new FiltersAndProperties();

            if (string.IsNullOrEmpty(fiql)) { return filtersAndProperties; }

            fiql = fiql.Replace("\\", "\\\\");
            fiql = fiql.Replace(">=", "=ge=");
            fiql = fiql.Replace("<=", "=le=");
            fiql = fiql.Replace("!=", "=ne=");
            fiql = fiql.Replace("<", "=lt=", comparisonType: StringComparison.CurrentCulture);
            fiql = fiql.Replace(">", "=gt=");


            var andConditions = SplitConditions(fiql, ',');
            var linqConditions = new List<string>();

            foreach (var andCondition in andConditions.Where(condition => !string.IsNullOrEmpty(condition)))
            {
                var orConditions = SplitConditions(andCondition, ';');
                var linqOrConditions = new List<string>();

                foreach (var orCondition in orConditions.Where(condition => !string.IsNullOrEmpty(condition)))
                {
                    if (SplitConditions(orCondition, ',').Count() > 1)
                    {
                        var fiqlloopString = FiqlToDapperLoop<T>(orCondition.TrimStart('(').TrimEnd(')'));
                        linqOrConditions.Add($"{fiqlloopString.Filters}");
                        filtersAndProperties.Properties.AddRange(fiqlloopString.Properties);
                    }
                    else
                    {
                        var fiqlloopString = FiqlToDapperLoop<T>(orCondition.TrimStart('(').TrimEnd(')'));
                        linqOrConditions.Add($"{fiqlloopString.Filters}");
                        filtersAndProperties.Properties.AddRange(fiqlloopString.Properties);
                    }
                }

                linqConditions.Add($"{startRoundBracate(linqOrConditions)}{string.Join(" AND ", linqOrConditions)}{endRoundBracate(linqOrConditions)}");
            }
            filtersAndProperties.Filters = $"{startRoundBracate(linqConditions)}{string.Join(" OR ", linqConditions)}{endRoundBracate(linqConditions)}";
            return filtersAndProperties;
        }
        static string startRoundBracate(List<string> conditions)
        {
            if (conditions.Count > 1)
            {
                return "(";
            }
            else
            {
                return "";
            }
        }
        static string endRoundBracate(List<string> conditions)
        {
            if (conditions.Count > 1)
            {
                return ")";
            }
            else
            {
                return "";
            }
        }
        static FiltersAndProperties FiqlToDapperLoop<T>(string fiql)
        {
            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            FiltersAndProperties filtersAndProperties = new FiltersAndProperties();
            // Split by semicolon for AND, and comma for OR

            var andConditions = SplitConditions(fiql, ',');
            var linqConditions = new List<string>();

            foreach (var andCondition in andConditions.Where(condition => !string.IsNullOrEmpty(condition)))
            {
                var orConditions = SplitConditions(andCondition, ';');
                var linqOrConditions = new List<string>();

                foreach (var orCondition in orConditions.Where(condition => !string.IsNullOrEmpty(condition)))
                {
                    if (SplitConditions(orCondition, ',').Count() > 1)
                    {
                        var fiqlloopString = FiqlToDapperLoop<T>(orCondition.TrimStart('(').TrimEnd(')'));
                        linqOrConditions.Add($"{fiqlloopString.Filters}");
                        filtersAndProperties.Properties.AddRange(fiqlloopString.Properties);
                    }
                    var parts = SplitConditions(orCondition, '=').ToArray();
                    if (parts.Length < 3)
                    {
                        throw new ArgumentException("Invalid FIQL query");
                    }

                    string property = parts[0];
                    string op = parts[1];
                    string value = parts[2].Trim('(', ')');

                    var propertie = properties.Where(x => x.Name.Equals(property, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (propertie != null)
                    {
                        var attribute = propertie.GetCustomAttributes(typeof(FilterMappingAttribute), false).FirstOrDefault() as FilterMappingAttribute;

                        if (attribute != null)
                        {
                            property = attribute.ColumnName;
                        }
                    }

                    filtersAndProperties.Properties.Add(property);

                    string linqOp = op switch
                    {
                        "gt" => ">",
                        "lt" => "<",
                        "ge" => ">=",
                        "le" => "<=",
                        "eq" => "=",
                        "ne" => "<>",

                        "eqd" => "=",
                        "led" => "<=",
                        "ged" => ">=",
                        "ltd" => "<",
                        "gtd" => ">",
                        "ned" => "<>",

                        "==" => "=",
                        "" => "=",
                        "in" => "IN",
                        "out" => "NOT IN",
                        "ilike" => "LIKE",
                        "olike" => "NOT LIKE",
                        _ => throw new ArgumentException($"Unsupported operator: {op}")
                    };

                    var propertyType = propertie?.PropertyType;
                    if (propertyType == null)
                        throw new ArgumentException($"Unsupported type: {op}");

                    if (linqOp == "IN" || linqOp == "NOT IN")
                    {
                        var values = string.Join(",", value.Split(',')
                           .Select(v => v.Trim())
                           .Select(v => propertyType == typeof(string) || propertyType == typeof(DateTime) ? $"'{v}'" : v)
                           .ToArray());

                        linqOrConditions.Add($"{property} {linqOp} ({values})");
                    }
                    else if (linqOp == "LIKE" || linqOp == "NOT LIKE")
                    {
                        if (value.StartsWith('*') || value.EndsWith('*'))
                        {
                            value = $"'{value.Replace("*", "%")}'";
                            linqOrConditions.Add($"{property} like ({value})");
                        }
                        else
                        {
                            value = $"'%{value}%'";
                            linqOrConditions.Add($"{property} {linqOp} ({value})");
                        }
                    }
                    else
                    {
                        if (propertyType == typeof(string) || propertyType == typeof(DateTime))
                        {
                            if (value?.ToLower() == "null")
                            {
                                linqOrConditions.Add($"{property} is null");
                            }
                            else
                            {
                                value = $"'{value}'";
                                if ("eqd,led,ged,ltd,gtd,ned".Split(',').Any(x => x.Equals(op, StringComparison.CurrentCultureIgnoreCase)))
                                    linqOrConditions.Add($"cast({property} as date) {linqOp} {value}");
                                else
                                    linqOrConditions.Add($"{property} {linqOp} {value}");
                            }
                        }
                        else
                        {
                            linqOrConditions.Add($"{property} {linqOp} {value}");
                        }

                    }
                }

                linqConditions.Add($"{startRoundBracate(linqOrConditions)}{string.Join(" AND ", linqOrConditions)}{endRoundBracate(linqOrConditions)}");
            }
            filtersAndProperties.Filters = $"{startRoundBracate(linqConditions)}{string.Join(" OR ", linqConditions)}{endRoundBracate(linqConditions)}";
            return filtersAndProperties;
        }

        private static IEnumerable<string> SplitConditions(string query, char separator)
        {
            int depth = 0;
            List<int> splitIndexes = new List<int>();

            for (int i = 0; i < query.Length; i++)
            {
                if (query[i] == '(') depth++;
                if (query[i] == ')') depth--;
                if (depth == 0 && query[i] == separator)
                {
                    splitIndexes.Add(i);
                }
            }

            splitIndexes.Add(query.Length);

            int start = 0;
            foreach (int index in splitIndexes)
            {
                yield return query.Substring(start, index - start);
                start = index + 1;
            }
        }
    }
}