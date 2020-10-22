using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Application.Abstractions;
using Identity.Domain.Abstractions;
using Identity.Domain.Extensions;
using Identity.Domain.User;
using Identity.Domain.User.Events;
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
        public async Task Should_PassCorrectDetails_When_Calling_Save()
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

            var aggregate = new UserAggregate(AggregateGuid.For<UserId>("dev"));
            var fullname = new Fullname("Mark", "Libres");
            var email = new Email("me@example.com");
            var password = new Password("secret");
            
            
            await WhenAsync(async repository =>
            {
                aggregate.Create(fullname, email, password);
                await repository.Save(aggregate, CancellationToken.None);
                return Task.CompletedTask;
            });

            Then(repository =>
            {
                eventsReceived.Should()
                    .NotBeNull()
                    .And
                    .HaveCount(1);
                streamNameReceived.Should().Be(aggregate.Id.StreamName);
                aggregate.UncommittedEvents.Should()
                    .NotBeNullOrEmpty()
                    .And
                    .HaveCount(1);
                aggregate.CommittedEvents.Should()
                    .BeNullOrEmpty();
            });
        }

        [Fact]
        public async Task Should_ReplayEvents_When_Loading_From_EventStore()
        {
            var userId = AggregateGuid.For<UserId>("dev");
            var fullname = new Fullname("Mark", "Libres");
            var email = new Email("me@example.com");
            var passwordString = "secret";
            var password = new Password(passwordString, false);
            var userCreatedEvent = new UserCreatedEvent(userId, fullname.Firstname, fullname.Lastname, email.Value,
                passwordString);
            
            Given(repository =>
            {
                _eventsRepoMock.Setup(r => r.Get(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((string streamName, CancellationToken cancellationToken) =>
                    {
                        var result = new List<IDomainEvent> { userCreatedEvent }.AsReadOnly();
                        return result;
                    });
            });
            
            var aggregate = await WhenAsync(async repository => await repository.Get(userId, CancellationToken.None));

            Then(repository =>
            {
                aggregate.Should().NotBeNull();
                aggregate.Fullname.Should().Be(fullname);
                aggregate.Email.Should().Be(email);
                aggregate.Password.Should().Be(password);
                aggregate.CommittedEvents.Should()
                    .NotBeNullOrEmpty()
                    .And
                    .HaveCount(1)
                    .And
                    .ContainEquivalentOf(userCreatedEvent);
                aggregate.UncommittedEvents.Should()
                    .BeNullOrEmpty();
            });

        }

    }
}