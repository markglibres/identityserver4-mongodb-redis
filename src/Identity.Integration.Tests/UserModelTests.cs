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
    public class UserModelTests : ServiceSpecifications<IAggregateRepository<User, UserId>>
    {
        public UserModelTests()
        {
            _streamName = DateTime.Now.Ticks.ToString();
        }

        private readonly string _streamName;

        [Fact]
        public async Task OnCreateUser_Should_Update_ReadModel()
        {
            var userId = UserId.Create();
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
        
        [Fact]
        public async Task OnUpdatePassword_Should_Update_ReadModel()
        {
            var userId = UserId.Create();
            var fullname = Fullname.Create(Faker.Name.FirstName(),Faker.Name.LastName());
            var email = Email.Create($"{Faker.Random.Word()}@example.com");
            var password = Password.Create("secret");
            
            
            await GivenAsync(async repository =>
            {
                var aggregate = new User(userId);
                aggregate.Create(fullname, email, password);
                await repository.Save(aggregate, CancellationToken.None);
            });

            var updatedPassword = Password.Create(Faker.Random.Word());
            await When(async repository =>
            {
                var savedUser = await repository.Get(userId, CancellationToken.None);
                savedUser.UpdatePassword(updatedPassword);
                await repository.Save(savedUser, CancellationToken.None);
            });

            await Then<IDocumentRepository<UserModel>>(async (mediator, repository) =>
            {
                var user = await repository.SingleOrDefault(u => u.Id == userId.Id.ToString());
                user.Should().NotBeNull();
                user.Password.Should().Be(updatedPassword.ToString());
            });
        }
    }
}