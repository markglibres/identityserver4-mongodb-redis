using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Application.Abstractions;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Identity.Domain.User.Events;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure;
using Xunit;

namespace Identity.Integration.Tests
{
    public class EventStoreRepositoryTests : ServiceSpecifications<IEventsRepository<IDomainEvent>>
    {
        private TenantId _tenantId;
        private string _streamName;

        public EventStoreRepositoryTests()
        {
            _tenantId = new TenantId("dev");
            _streamName = DateTime.Now.Ticks.ToString();
        }
        
        [Fact]
        public async Task Test()
        {
            Given(repository => repository);

            var user = UserId.From(_tenantId);
            var @event = new UserCreatedEvent(user, "Mark", "Libres", "me@example.com", "secret");

            await When(async repository =>
            {
                await repository.Save(
                    new[] { @event },
                    _streamName,
                    CancellationToken.None);

                return Task.CompletedTask;
            });
            
            await Then(async repository =>
            {
                var result = await repository
                    .Get(_streamName, CancellationToken.None);

                result.Should().NotBeNull();
                result.Should().HaveCount(1);
            });
        }
    }
}