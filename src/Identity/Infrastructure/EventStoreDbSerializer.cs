using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EventStore.Client;
using Identity.Domain.Abstractions;
using Identity.Domain.Attributes;
using Identity.Domain.Extensions;
using Identity.Domain.User.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;
using JsonProperty = Newtonsoft.Json.Serialization.JsonProperty;

namespace Identity.Infrastructure
{
    public class EventStoreDbSerializer : IEventStoreDbSerializer
    {
        private readonly List<Type> _domainEventTypes;
        private readonly JsonSerializerSettings _serializerSettings;

        public EventStoreDbSerializer()
        {
            _domainEventTypes = GetEventTypes();
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
                NullValueHandling = NullValueHandling.Ignore
            };
            

        }
        public EventData Serialize(IDomainEvent @event)
        {
            var eventId = @event.Id;
            var type = @event.GetType().GetEventType();
            var data = ToBytes(@event);
            var metadata = ToBytes(GetHeaders(@event));
            
            return new EventData(Uuid.Parse(eventId), type, data, metadata);

        }

        public async Task<IDomainEvent> Deserialize(EventRecord @event)
        {
            var metaData = JsonConvert.DeserializeObject<EventMetadata>(FromBytes(@event.Metadata));
            var eventType = _domainEventTypes
                .FirstOrDefault(type =>
                    type.GetEventName() == metaData.EventName && type.GetEventVersion() == metaData.EventVersion);
            
            if(eventType == null) return null;

            using var reader = new StreamReader(new MemoryStream(@event.Data.ToArray()));
            var data = await reader.ReadToEndAsync();
            var deserializeObject = JsonConvert.DeserializeObject(data, eventType, _serializerSettings);
            return (IDomainEvent)deserializeObject;
        }

        private static List<Type> GetEventTypes()
        {
            var domainEvents = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttribute(typeof(EventNameAttribute)) != null 
                               && (type.IsSubclassOf(typeof(DomainEvent)) || type.IsSubclassOf(typeof(IDomainEvent))))
                .ToList();

            return domainEvents;
        }

        private ReadOnlyMemory<byte> ToBytes(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, _serializerSettings);
            return Encoding.UTF8.GetBytes(json);
        }

        private string FromBytes(ReadOnlyMemory<byte> bytes) => Encoding.UTF8.GetString(bytes.ToArray());

        private EventMetadata GetHeaders(IDomainEvent @event)
        {
            var headers = new EventMetadata
            {
                Id = @event.Id,
                EventName = @event.GetType().GetEventName(),
                EventVersion = @event.GetType().GetEventVersion()
            };

            return headers;
        }
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

    class EventMetadata
    {
        public string Id { get; set; }
        public string EventName { get; set; }
        public string EventVersion { get; set; }
    }
    
}