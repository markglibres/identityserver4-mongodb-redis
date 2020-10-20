using System;

namespace Identity.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventNameAttribute : Attribute
    {
        private readonly string _eventName;
        private readonly string _eventVersion;
        
        public EventNameAttribute(string eventName, string eventVersion="v1")
        {
            _eventName = eventName;
            _eventVersion = eventVersion;
        }

        public string EventName => _eventName;
        public string EventVersion => _eventVersion;
        public string EventType => $"{_eventName}.{_eventVersion}";
    }
}