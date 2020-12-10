using System;

namespace Identity.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventNameAttribute : Attribute
    {
        public EventNameAttribute(string nameSpace, string eventName, string eventVersion="1.0")
        {
            EventNameSpace = nameSpace;
            EventName = eventName;
            EventVersion = eventVersion;
        }

        public string EventName { get; }

        public string EventVersion { get; }

        public string EventNameSpace { get; }

        public string EventFullname => $"{EventNameSpace}.{EventName}.{EventVersion}";
    }
}