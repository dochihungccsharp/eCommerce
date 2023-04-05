using System.Data;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace eCommerce.Shared.Extensions;

public static class CollectionExtensions
{
    private static readonly IDictionary<Type, ICollection<PropertyInfo>> _properties =
        new Dictionary<Type, ICollection<PropertyInfo>>();

    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        Formatting = Formatting.None,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    // convert Datatable to IEnumerable, extension method
    public static IEnumerable<T> ToList<T>(this DataTable dataTable) where T : class, new()
    {
        var typeOfT = typeof(T);

        ICollection<PropertyInfo> properties;

        lock (_properties)
        {
            if (!_properties.TryGetValue(typeOfT, out properties))
            {
                properties = typeOfT.GetProperties().Where(p => p.CanWrite).ToList();
                _properties.Add(typeOfT, properties);
            }
        }

        var list = new List<T>(dataTable.Rows.Count);

        foreach (var row in dataTable.AsEnumerable())
        {
            var obj = new T();
            foreach (DataColumn column in row.Table.Columns)
            {
                PropertyInfo? prop = obj.GetType().GetProperty(column.ColumnName);
                if (prop == null) continue;

                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var value = row[prop.Name] == DBNull.Value ? null : row[prop.Name];

                object safeValue = default!;
                if (value != null)
                {
                    if (column.ColumnName.ToLower().Contains("_"))
                    {
                        safeValue = JsonConvert.DeserializeObject(value.ToString(), propType, Settings);
                    }
                    else
                    {
                        safeValue = Convert.ChangeType(row[prop.Name], propType);
                    }
                }

                prop.SetValue(obj, safeValue, null);
            }

            list.Add(obj);
        }

        return list;
    }

    public static DataTable ToDataTable<T>(this IList<T> list)
    {
        DataTable dataTable = new DataTable();
        
        var properties = typeof(T).GetProperties();
        
        foreach (var property in properties)
        {
            dataTable.Columns.Add(property.Name);
        }
        
        foreach (var item in list)
        {
            DataRow row = dataTable.NewRow();
            
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(DateTime))
                {
                    var value = property.GetValue(item, null);
                    row[property.Name] = value != null ? (object)value : DBNull.Value;
                }
                else
                {
                    row[property.Name] = property.GetValue(item, null);
                }
            }

            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    // check IEnumerable<T> source not null or not empty
    public static bool NotNullOrEmpty<T>(this IEnumerable<T> source)
        => source != null && source.Any();

    // check IList<T> source not null or not empty
    public static bool NotNullOrEmpty<T>(this IList<T> source)
        => source != null && source.Any();
}

