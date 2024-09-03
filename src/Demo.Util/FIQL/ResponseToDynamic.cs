using Demo.Util.Models;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demo.Util.FIQL
{
    public class ResponseToDynamic : IResponseToDynamic
    {
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

        public async Task<ResponseToDynamicModel> ContextResponse(IQueryable result, string fields, string filters, string sort, int pageNo = 0, int pageSize = 0)
        {
            ResponseToDynamicModel responseToDynamicModel = new ResponseToDynamicModel();
            var filtersAndProperties = ConvertFiqlToLinq.FiqlToLinq(filters ?? "");
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

        public List<string> GetPropertyNames<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
                            .Select(p => p.Name)
                            .ToList();
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
        List<string> GetPropertyNames<T>();
        IEnumerable<QueryIncludeModel> GetInclude(string include);
        List<SubQueryParam> ParseIncludeParameter(string include);
        dynamic ConvertTo<T>(List<T> retVal, string select);
        dynamic ConvertTo<T>(T retVal, string select);
        Task<ResponseToDynamicModel> ContextResponse(IQueryable result, string fields, string filters, string sort, int pageNo = 0, int pageSize = 0);
    }
}