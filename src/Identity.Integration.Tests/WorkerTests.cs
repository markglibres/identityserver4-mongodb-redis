using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Application.Abstractions;
using Identity.Application.Users.GetUser;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Identity.Domain.User.Events;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Abstractions;
using Xunit;

namespace Identity.Integration.Tests
{
    public class WorkerTests : ServiceSpecifications<IAggregateRepository<User, UserId>>
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
            var fullname = Fullname.Create(Faker.Name.FirstName(),Faker.Name.LastName());
            var email = Email.Create($"{Faker.Random.Word()}@example.com");
            var password = Password.Create("secret");
            
            await GivenAsync(async repository =>
            {
                var aggregate = new User(userId);
                aggregate.Create(fullname, email, password);

                await repository.Save(aggregate, CancellationToken.None);
            });

            await Then<IDocumentRepository<UserModel>>(async (mediator, repository) =>
            {
                var user = await repository.SingleOrDefault(u => u.Id == userId.Id.ToString());
                user.Should().NotBeNull();
            });
        }
    }
}