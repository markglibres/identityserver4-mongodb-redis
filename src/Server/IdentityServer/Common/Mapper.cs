using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer.Common
{
    public interface IMapper
    {
        T Map<T>(object source, Action<T> action = null) where T : class, new();
    }

    public class Mapper : IMapper
    {
        public T Map<T>(object source, Action<T> action = null) where T : class, new()
        {
            var destination = Activator.CreateInstance<T>();
            var sourceProps = source?.GetType().GetProperties();
            var destinationProps = destination?.GetType().GetProperties();

            if (!sourceProps?.Any() ?? true) return destination;
            if (!destinationProps?.Any() ?? true) return destination;

            foreach (var sourceProp in sourceProps)
            {
                if (!sourceProp.CanRead) continue;

                var sourceVal = sourceProp.GetValue(source);
                var destinationProp = destinationProps.FirstOrDefault(p => p.Name == sourceProp.Name && p.CanWrite);
                if (destinationProp == null) continue;

                destinationProp.SetValue(destination, sourceVal, null);
            }

            action?.Invoke(destination);
            return destination;
        }

        public static void MapNotNullProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            var sourceProps = source?.GetType().GetProperties();
            var destinationProps = destination?.GetType().GetProperties();

            if (!sourceProps?.Any() ?? true) return;
            if (!destinationProps?.Any() ?? true) return;

            foreach (var sourceProp in sourceProps)
            {
                if (!sourceProp.CanRead) continue;

                var sourceVal = sourceProp.GetValue(source);
                if (sourceVal == null || sourceVal == GetTypeDefault(sourceVal)) return;

                var destinationProp = destinationProps.FirstOrDefault(p => p.Name == sourceProp.Name && p.CanWrite);
                if (destinationProp == null) continue;

                if (!IsPrimitive(sourceProp.PropertyType))
                {
                    var destinationVal = destinationProp.GetValue(destination);
                    if (destinationVal == null)
                        destinationProp.SetValue(destination, Activator.CreateInstance(destinationProp.PropertyType),
                            null);

                    MapNotNullProperties(sourceVal, destinationProp.GetValue(destination));
                }
                else
                {
                    destinationProp.SetValue(destination, sourceVal, null);
                }
            }
        }

        private static object GetTypeDefault(object obj)
        {
            return obj.GetType().GetMethod("GetDefaultGeneric")?.MakeGenericMethod(obj.GetType()).Invoke(obj, null);
        }

        private static T GetDefaultGeneric<T>()
        {
            return default;
        }

        private static bool IsPrimitive(Type type)
        {
            var otherPrimitiveTypes = new List<string> {"string", "decimal", "datetime"};

            return type.IsPrimitive || type.IsValueType ||
                   otherPrimitiveTypes.Contains(type.Name, StringComparer.OrdinalIgnoreCase);
        }
    }
}
