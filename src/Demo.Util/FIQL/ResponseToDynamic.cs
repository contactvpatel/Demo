using Demo.Util.Models;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;

namespace Demo.Util.FIQL
{
    public class ResponseToDynamic : IResponseToDynamic
    {
        private readonly IDbConnection _dbConnection;

        public ResponseToDynamic(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public string GetQueryParts(string query, string part)
        {
            var queryParts = SplitConditions(query, '&', '{', '}');
            var result = (queryParts.Where(x => x.StartsWith($"{part}=")).FirstOrDefault() ?? "").Replace($"{part}=", "");
            result = result?.Trim() ?? string.Empty;
            return result;
        }

        public string GetSort(string query)
        {
            return GetQueryParts(query, "sort");
        }

        public string GetFilters(string query)
        {
            return GetQueryParts(query, "filters");
        }

        public string GetFields(string query)
        {
            return GetQueryParts(query, "fields");
        }

        public string GetPageNo(string query)
        {
            return GetQueryParts(query, "pageno");
        }

        public string GetPageSize(string query)
        {
            return GetQueryParts(query, "pagesize");
        }

        public IEnumerable<QueryIncludeModel> GetInclude(string include)
        {
            List<QueryIncludeModel> queryIncludes = new();
            include = include?.Trim() ?? string.Empty;
            var listOfInclude = SplitConditions(include, ';');
            foreach (var item in listOfInclude)
            {
                var queryInclude = new QueryIncludeModel();

                var b = item.TrimEnd('}');

                var a = b.Split('{', StringSplitOptions.RemoveEmptyEntries);
                if (a.Length == 2)
                {
                    queryInclude.ObjectName = a[0];
                    queryInclude.ObjectQuery = a[1];
                    queryInclude.ObjectFields = GetFields(queryInclude.ObjectQuery);
                    queryInclude.ObjectFilters = GetFilters(queryInclude.ObjectQuery);
                }
                else
                    queryInclude.ObjectName = item;

                queryIncludes.Add(queryInclude);
            }
            return queryIncludes;
        }

        public List<SubQueryParam> ParseIncludeParameter(string include)
        {
            var subQueryParams = new List<SubQueryParam>();

            var subQueries = SplitConditions(include, ';', '{', '}');

            foreach (var subQuery in subQueries)
            {
                var startOfParams = subQuery.IndexOf('{');
                if (startOfParams == -1)
                {
                    var objectName = subQuery;
                    subQueryParams.Add(new SubQueryParam
                    {
                        ObjectName = objectName,
                        Fields = "",
                        Filters = ""
                    });
                }
                else
                {
                    var objectName = subQuery.Substring(0, startOfParams).Trim();
                    var paramsString = subQuery.Substring(startOfParams + 1);
                    paramsString = paramsString.IndexOf('}') > 0 ? paramsString.Substring(0, paramsString.Length - 1) : paramsString;

                    var paramPairs = SplitConditions(paramsString, '&', '{', '}');
                    string fields = null;
                    string filters = null;
                    string includes = null;

                    foreach (var pair in paramPairs)
                    {
                        var keyValue = pair.Split('=', 2);
                        if (keyValue.Length == 2)
                        {
                            var key = keyValue[0].Trim();
                            var value = keyValue[1].Trim();

                            if (key.Equals("fields", StringComparison.OrdinalIgnoreCase))
                            {
                                fields = value;
                            }
                            else if (key.Equals("filters", StringComparison.OrdinalIgnoreCase))
                            {
                                filters = value;
                            }
                            else if (key.Equals("include", StringComparison.OrdinalIgnoreCase))
                            {
                                includes = value;
                            }
                        }
                    }
                    subQueryParams.Add(new SubQueryParam
                    {
                        ObjectName = objectName,
                        Fields = fields,
                        Filters = filters,
                        Include = includes
                    });
                }
            }

            return subQueryParams;
        }

        private IEnumerable<string> SplitConditions(string query, char separator, char ignoreStartChar = '(', char ignoreEndChar = ')')
        {
            int depth = 0;
            List<int> splitIndexes = new List<int>();

            for (int i = 0; i < query.Length; i++)
            {
                if (query[i] == ignoreStartChar) depth++;
                if (query[i] == ignoreEndChar) depth--;
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

        public async Task<DynamicResponseModel> ContextResponse<T>(IQueryable result, string fields, string filters, string sort, int pageNo = 0, int pageSize = 0)
        {
            DynamicResponseModel responseToDynamicModel = new DynamicResponseModel();
            var filtersAndProperties = ConvertFiqlToLinq.FiqlToLinq<T>(filters ?? "");
            filters = filtersAndProperties.Filters;
            var _filterFields = filtersAndProperties.Properties.Where(x => !string.IsNullOrEmpty(x) && !fields.Split(',').Any(y => y.ToLower() == x.ToLower())).ToList();

            if (_filterFields.Count > 0 && !string.IsNullOrEmpty(fields))
                fields = string.Concat(fields, ",", string.Join(",", _filterFields.ToArray()));
            if (!string.IsNullOrEmpty(fields))
            {
                result = result.Select($"new ({fields})");
            }
            if (!string.IsNullOrEmpty(filters))
            {
                result = result.Where(filters);
            }
            if (!string.IsNullOrEmpty(sort))
            {
                result = result.OrderBy(sort);
            }
            if (pageNo > 0 && pageSize > 0)
            {
                responseToDynamicModel.TotalRecords = result.Count();
                result = result.Skip((pageNo - 1) * pageSize).Take(pageSize);
            }

            responseToDynamicModel.Data = await result.ToDynamicListAsync();

            if (!(pageNo > 0 && pageSize > 0))
            {
                responseToDynamicModel.TotalRecords = responseToDynamicModel.Data.Count;
            }

            return responseToDynamicModel;
        }

        string SortFieldsMapingName<T>(string sort)
        {
            string retValue = string.Empty;

            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            Dictionary<string, string> sortFields = sort
                .Split(',')
                .Select(x => x.Trim())
                .ToDictionary(
                    x => x,
                    x => x.Replace(" desc", "").Replace(" asc", "").Trim()
                );

            foreach (var property in sortFields)
            {
                var prop = properties
                    .Where(x => x.Name.Equals(property.Value, StringComparison.CurrentCultureIgnoreCase))
                    .FirstOrDefault();

                if (prop != null)
                {
                    var attribute = prop.GetCustomAttributes(typeof(FilterMappingAttribute), false)
                                        .FirstOrDefault() as FilterMappingAttribute;

                    if (attribute != null)
                    {
                        if (property.Key.EndsWith("desc", StringComparison.CurrentCultureIgnoreCase))
                        {
                            retValue += $"{attribute.ColumnName} desc, ";
                        }
                        else if (property.Key.EndsWith("asc", StringComparison.CurrentCultureIgnoreCase))
                        {
                            retValue += $"{attribute.ColumnName} asc, ";
                        }
                        else
                        {
                            retValue += $"{attribute.ColumnName}, ";
                        }
                    }
                    else
                    {
                        if (property.Key.EndsWith("desc", StringComparison.CurrentCultureIgnoreCase))
                        {
                            retValue += $"{property.Value} desc, ";
                        }
                        else if (property.Key.EndsWith("asc", StringComparison.CurrentCultureIgnoreCase))
                        {
                            retValue += $"{property.Value} asc, ";
                        }
                        else
                        {
                            retValue += $"{property.Value}, ";
                        }
                    }
                }
            }

            retValue = retValue.TrimEnd(',', ' ');

            return retValue;
        }

        public async Task<DynamicDapperResponseModel<T>> DapperResponse<T>(string query, string filters, string sort, int pageNo = 0, int pageSize = 0)
        {

            DynamicDapperResponseModel<T> responseToDynamicModel = new DynamicDapperResponseModel<T>();
            var filtersAndProperties = ConvertFiqlToDapper.FiqlToDapper<T>(filters ?? "");
            filters = filtersAndProperties.Filters;
            sort = SortFieldsMapingName<T>(sort);
            // Modify the query with filters, sorting, and pagination
            if (!string.IsNullOrEmpty(filters))
            {
                query += $" WHERE {filters}";
            }
            if (pageNo > 0 && pageSize > 0)
            {
                var countQuery = $"SELECT COUNT(1) FROM ({query}) AS CountQuery";
                responseToDynamicModel.TotalRecords = await _dbConnection.ExecuteScalarAsync<int>(countQuery);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                query += $" ORDER BY {sort}";
            }
            if (pageNo > 0 && pageSize > 0)
            {
                query += $" OFFSET {(pageNo - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            }

            responseToDynamicModel.Data = (await _dbConnection.QueryAsync<T>(query)).ToList();

            if (!(pageNo > 0 && pageSize > 0))
            {
                responseToDynamicModel.TotalRecords = responseToDynamicModel.Data.Count;
            }

            return responseToDynamicModel;
        }
        public string GetPropertyNamesString<T>()
        {
            var names = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
                            .Select(p => p.Name)
                            .ToArray();

            return string.Join(",", names);
        }

        public bool TryGetMissingPropertyNames<T>(string fields, out string missingFields)
        {
            var propertyNames = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                         .Select(p => p.Name)
                                         .ToArray();

            var fieldArray = fields.Split(',');

            var missingFieldList = fieldArray.Where(field => !propertyNames.Contains(field)).ToList();

            if (missingFieldList.Any())
            {
                missingFields = string.Join(",", missingFieldList);
                return false;
            }

            missingFields = null;
            return true;
        }

        public dynamic ConvertTo<T>(T retVal, string select)
        {
            var options = new JsonSerializerOptions();

            var selectColumn = select.Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet<string>();
            options.Converters.Add(new DynamicResponseConverter<T>(selectColumn));

            dynamic json = JsonDocument.Parse(JsonSerializer.Serialize(retVal, options));

            return json;
        }

        public dynamic ConvertTo<T>(List<T> retVal, string select)
        {
            var options = new JsonSerializerOptions();

            var selectColumn = select.Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            if (!selectColumn.Any(x => string.IsNullOrEmpty(x)))
            {
                options.Converters.Add(new DynamicResponseConverter<T>(selectColumn));
            }

            JsonDocument json = JsonDocument.Parse(JsonSerializer.Serialize(retVal, options));

            return json;
        }

        internal class DynamicResponseConverter<T> : JsonConverter<T>
        {
            private readonly HashSet<string> _propertiesToIgnore;

            public DynamicResponseConverter(HashSet<string> propertiesToIgnore)
            {
                _propertiesToIgnore = propertiesToIgnore;
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach (var property in typeof(T).GetProperties())
                {
                    if (_propertiesToIgnore.Any(x => x.ToLower() == property.Name.ToLower()) || _propertiesToIgnore.Count() == 0)
                    {
                        var propValue = property.GetValue(value);
                        writer.WritePropertyName(property.Name);
                        JsonSerializer.Serialize(writer, propValue, options);
                    }
                }

                writer.WriteEndObject();
            }
        }
    }

    public interface IResponseToDynamic
    {
        string GetPropertyNamesString<T>();
        bool TryGetMissingPropertyNames<T>(string fields, out string missingFields);
        IEnumerable<QueryIncludeModel> GetInclude(string include);
        List<SubQueryParam> ParseIncludeParameter(string include);
        dynamic ConvertTo<T>(List<T> retVal, string select);
        dynamic ConvertTo<T>(T retVal, string select);
        Task<DynamicResponseModel> ContextResponse<T>(IQueryable result, string fields, string filters, string sort, int pageNo = 0, int pageSize = 0);
        Task<DynamicDapperResponseModel<T>> DapperResponse<T>(string query, string filters, string sort, int pageNo = 0, int pageSize = 0);
    }
}