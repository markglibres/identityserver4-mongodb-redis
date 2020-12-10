using System.Threading.Tasks;
using EventStore.Client;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Abstractions;
using Identity.Infrastructure.Models;

namespace Identity.Infrastructure
{
    public class StreamManager : IStreamManager
    {
        private readonly IDocumentRepository<SubscriptionSettings> _documentRepository;
        private readonly TenantId TenantId = TenantId.Default;

        public StreamManager(IDocumentRepository<SubscriptionSettings> documentRepository)
        {
            _documentRepository = documentRepository;
            
        }
        public async Task<Position> GetPosition()
        {
            var settings = await _documentRepository
                .SingleOrDefault(s => s.Tenant == TenantId.ToString(), TenantId);

            if (settings != null) return settings.Position;
            
            settings = SubscriptionSettings.For(TenantId.ToString());
            await _documentRepository.Insert(settings);

            return settings.Position;
        }

        public async Task SetPosition(Position position)
        {
            var settings = await _documentRepository.SingleOrDefault(s => s.Tenant == TenantId.ToString(), TenantId);
            
            settings.SetLastPosition((long)position.CommitPosition);
            await _documentRepository.Update(settings, s => s.Id.Equals(settings.Id), TenantId);
        }
    }
}