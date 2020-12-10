using System.Reflection;

namespace Identity.Domain.Extensions
{
    public static class PropertyExtensions
    {
        public static void SetProperty(this object obj, string propertyName, object value)
        {
            obj
                .GetType()
                .BaseType
                ?.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(obj, value, null);
        }
    }
}