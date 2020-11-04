using System.Threading.Tasks;
using EventStore.Client;
using Identity.Infrastructure.Abstractions;
using Identity.Infrastructure.Models;

namespace Identity.Infrastructure
{
    public class StreamManager : IStreamManager
    {
        private readonly IDocumentRepository<SubscriptionSettings> _documentRepository;
        private const string _tenant = "all";

        public StreamManager(IDocumentRepository<SubscriptionSettings> documentRepository)
        {
            _documentRepository = documentRepository;
            
        }
        public async Task<Position> GetPosition()
        {
            var settings = await _documentRepository
                .SingleOrDefault(s => s.Tenant == _tenant);

            if (settings != null) return settings.Position;
            
            settings = SubscriptionSettings.For(_tenant);
            await _documentRepository.Insert(settings);

            return settings.Position;
        }

        public async Task SetPosition(Position position)
        {
            var settings = await _documentRepository.SingleOrDefault(s => s.Tenant == _tenant);
            
            settings.SetLastPosition((long)position.CommitPosition);
            await _documentRepository.Update(settings, s => s.Id.Equals(settings.Id));
        }
    }
}