using System.Reflection;
using Identity.Domain.Attributes;

namespace Identity.Domain.Extensions
{
    public static class EventNameAttributeExtensions
    {
        public static string GetEventName(this MemberInfo type) => type.GetCustomAttribute<EventNameAttribute>()?.EventName;
        public static string GetEventVersion(this MemberInfo type) => type.GetCustomAttribute<EventNameAttribute>()?.EventVersion;
        public static string GetEventFullname(this MemberInfo type) => type.GetCustomAttribute<EventNameAttribute>()?.EventFullname;
        public static string GetEventNamespace(this MemberInfo type) => type.GetCustomAttribute<EventNameAttribute>()?.EventNameSpace;
    }
}