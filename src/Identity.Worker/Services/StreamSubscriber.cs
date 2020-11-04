using System.Threading.Tasks;
using EventStore.Client;
using Identity.Infrastructure.Abstractions;
using Identity.Worker.Models;
using Identity.Worker.Services.Abstractions;

namespace Identity.Worker.Services
{
    public class StreamSubscriber : IStreamSubscriber
    {
        private readonly IDocumentRepository<SubscriptionSettings> _documentRepository;
        private const string _database = "global";
        private const string _tenant = "all";

        public StreamSubscriber(IDocumentRepository<SubscriptionSettings> documentRepository)
        {
            _documentRepository = documentRepository;
            
        }
        public async Task<Position> GetPosition()
        {
            var settings = await _documentRepository
                .SingleOrDefault(_database,
                    s => s.Tenant == _tenant);

            if (settings != null) return settings.Position;
            
            settings = SubscriptionSettings.For(_tenant);
            await _documentRepository.Insert(_database, settings);

            return settings.Position;
        }

        public async Task SetPosition(Position position)
        {
            var settings = await _documentRepository.SingleOrDefault(_database,
                s => s.Tenant == _tenant);
            
            settings.SetLastPosition((long)position.CommitPosition);
            await _documentRepository.Update(_database, settings, s => s.Id.Equals(settings.Id));
        }
    }
}