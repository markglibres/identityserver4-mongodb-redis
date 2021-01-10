using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace IdentityServer.Management.Common
{
    public class Mapper : IMapper
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public Mapper()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new PrivateResolver(),
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                },
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }

        public T Map<T>(object source, Action<T> action = null) where T : class, new()
        {
            if (source == null) return default;

            var serializedSource = JsonConvert.SerializeObject(source, _serializerSettings);
            var destination = JsonConvert.DeserializeObject(serializedSource, typeof(T), _serializerSettings);

            action?.Invoke(destination as T);

            return destination as T;
        }

        public static void MapNotNullProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            var sourceProps = source?.GetType().GetProperties();
            var destinationProps = destination?.GetType().GetProperties();

            if (sourceProps == null || destinationProps == null) return;

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
                        destinationProp.SetValue(destination, Activator.CreateInstance(destinationProp.PropertyType));

                    MapNotNullProperties(sourceVal, destinationProp.GetValue(destination));
                }
                else
                {
                    destinationProp.SetValue(destination, sourceVal);
                }

                // destination.GetType().InvokeMember(
                //     destinationProp.Name,
                //     BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                //     Type.DefaultBinder, destination, new[] {sourceVal});
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

    public interface IMapper
    {
        T Map<T>(object source, Action<T> action = null) where T : class, new();
    }

    internal class PrivateResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (prop.Writable) return prop;

            var property = member as PropertyInfo;
            var hasPrivateSetter = property?.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;
            return prop;
        }
    }
}
