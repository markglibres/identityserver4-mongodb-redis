using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Identity.Application.Abstractions;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure;
using Moq;
using Xunit;

namespace Identity.Unit.Tests
{
    public class AggregateRepositoryTests : UnitSpecifications<AggregateRepository<UserAggregate, UserId>>
    {
        private Mock<IEventsRepository<IDomainEvent>> _eventsRepoMock;

        public AggregateRepositoryTests()
        {
            _eventsRepoMock = new Mock<IEventsRepository<IDomainEvent>>();
            Register(() => _eventsRepoMock.Object);
        }

        [Fact]
        public void Should_PassCorrectDetails_When_Calling_Save()
        {
            IReadOnlyCollection<IDomainEvent> eventsReceived = null;
            var streamNameReceived = string.Empty;
            Given(repository =>
            {
                _eventsRepoMock.Setup(r => r.Save(
                        It.IsAny<IReadOnlyCollection<IDomainEvent>>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                    .Callback<IReadOnlyCollection<IDomainEvent>, string, CancellationToken>((
                        events, 
                        streamName, 
                        cancellationToken) =>
                    {
                        eventsReceived = events;
                        streamNameReceived = streamName;
                    });
            });

            var aggregate = new UserAggregate(UserId.From(TenantId.From("dev")));
            var fullname = new Fullname("Mark", "Libres");
            var email = new Email("me@example.com");
            var password = new Password("secret");
            
            When(repository =>
            {
                aggregate.Create(fullname, email, password);
                repository.Save(aggregate, CancellationToken.None);
            });

            Then(repository =>
            {
                eventsReceived.Should()
                    .NotBeNull()
                    .And
                    .HaveCount(1);
                streamNameReceived.Should().Be(aggregate.Id.StreamName);
            });
        }
    }
}