using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Application.Abstractions;
using Identity.Application.Users.GetUser;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Identity.Domain.User.Events;
using Identity.Infrastructure.Abstractions;
using Xunit;

namespace Identity.Integration.Tests
{
    public class WorkerTests : ServiceSpecifications<IEventsRepository<IDomainEvent>>
    {
        public WorkerTests()
        {
            _streamName = DateTime.Now.Ticks.ToString();
        }

        private readonly string _streamName;

        [Fact]
        public async Task OnSaveEvents_ToEventStore_Should_SubscribeToStream_And_Write_ReadOnly_Model()
        {
            var userId = AggregateGuid.Create<UserId>();
            var @event = new UserCreatedEvent(userId, 
                Faker.Name.FirstName(), 
                Faker.Name.LastName(), 
                $"{Faker.Random.Word()}@example.com", 
                "secret");

            Given(repository => { });

            await When(async repository =>
            {
                await repository.Save(
                    new[] {@event},
                    _streamName,
                    CancellationToken.None);

                return Task.CompletedTask;
            });

            await Then<IDocumentRepository<UserModel>>(async (mediator, repository) =>
            {
                var user = await repository.SingleOrDefault(u => u.Id == userId.Id.ToString(),
                    userId.TenantId);
                user.Should().NotBeNull();
            });
        }
    }
}