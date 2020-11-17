using System;
using System.Collections.Generic;
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
    }

    public interface IMapper
    {
        T Map<T>(object source, Action<T> action = null) where T: class, new();
    }
    
    class PrivateResolver : DefaultContractResolver {
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