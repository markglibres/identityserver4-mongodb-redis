using System;
using EventStore.Client;

namespace Identity.Worker.Models
{
    public class SubscriptionSettings
    {
        public Guid Id { get; private set; }
        public string Tenant { get; private set; }
        public long LastPosition { get; private set; }

        public SubscriptionSettings()
        {
            
        }
        private SubscriptionSettings(string tenant)
        {
            Tenant = tenant;
            Id = Guid.NewGuid();
        }

        public void SetLastPosition(long lastPosition)
        {
            LastPosition = lastPosition;
        }

        public Position Position => LastPosition == 0 ? Position.Start : new Position((ulong)LastPosition, (ulong)LastPosition);
        
        public static SubscriptionSettings For(string tenant) => new SubscriptionSettings(tenant);
    }
}