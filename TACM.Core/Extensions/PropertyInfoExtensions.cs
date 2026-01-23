using System.Reflection;

namespace TACM.Core.Extensions
{
    public static class PropertyInfoExtensions
    {
        private static readonly IDictionary<int, Func<object?, bool>> _typesAndValidators = new Dictionary<int, Func<object?, bool>>()
        {            
            { typeof(string).GetHashCode(), (object? value) => !string.IsNullOrEmpty(value?.ToString()) },
            { typeof(short).GetHashCode(), (object? value) => short.TryParse(value?.ToString(), out short result) },
            { typeof(int).GetHashCode(), (object? value) => int.TryParse(value?.ToString(), out int result) },
            { typeof(long).GetHashCode(), (object? value) => long.TryParse(value?.ToString(), out long result) },
            { typeof(ushort).GetHashCode(), (object? value) => ushort.TryParse(value?.ToString(), out ushort result) },
            { typeof(uint).GetHashCode(), (object? value) => uint.TryParse(value?.ToString(), out uint result) },
            { typeof(ulong).GetHashCode(), (object? value) => ulong.TryParse(value?.ToString(), out ulong result) },
            { typeof(DateTime).GetHashCode(), (object? value) => DateTime.TryParse(value?.ToString(), out DateTime result) },
            { typeof(object).GetHashCode(), (object? value) => value is not null },

            { typeof(short?).GetHashCode(), (object? value) => value is not null    && short.TryParse(value?.ToString(), out short result) },
            { typeof(int?).GetHashCode(), (object? value) => value is not null      && int.TryParse(value?.ToString(), out int result) },
            { typeof(long?).GetHashCode(), (object? value) => value is not null     && long.TryParse(value ?.ToString(), out long result) },
            { typeof(ushort?).GetHashCode(), (object? value) => value is not null   && ushort.TryParse(value ?.ToString(), out ushort result) },
            { typeof(uint?).GetHashCode(), (object? value) => value is not null     && uint.TryParse(value ?.ToString(), out uint result) },
            { typeof(ulong?).GetHashCode(), (object? value) => value is not null    && ulong.TryParse(value ?.ToString(), out ulong result) },
            { typeof(DateTime?).GetHashCode(), (object? value) => value is not null && DateTime.TryParse(value?.ToString(), out DateTime result) }          
        };

        public static bool HasValue(this PropertyInfo property, object instance)
        {
            var success = _typesAndValidators.TryGetValue(property.PropertyType.GetHashCode(), out var validatorFunc);

            if (!success)
                return false;

            return validatorFunc?.Invoke(property.GetValue(instance)) ?? false;
        }
    }
}
