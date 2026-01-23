using System.Collections.Immutable;
using System.Reflection;

namespace TACM.UI.Utils;

public static class PropertiesMemoryCache
{
    private static IDictionary<int, ImmutableArray<PropertyInfo>> _propertiesDict = new Dictionary<int, ImmutableArray<PropertyInfo>>();

    private static ImmutableArray<PropertyInfo> AddPropertiesAndReturnProperties(ref readonly Type type)
    {
        var properties = type.GetProperties().ToImmutableArray();
        _propertiesDict[type.GetHashCode()] = properties;

        return properties;
    }

    public static ImmutableArray<PropertyInfo> GetProperties(in Type type)
    {
        var success = _propertiesDict.TryGetValue(type.GetHashCode(), out var properties);

        if (!success)
            properties = AddPropertiesAndReturnProperties(in type);

        return properties;
    }
}
