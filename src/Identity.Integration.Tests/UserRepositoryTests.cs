using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Application.Abstractions;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Identity.Domain.User.Events;
using Identity.Domain.ValueObjects;
using Xunit;

namespace Identity.Integration.Tests
{
    public class UserRepositoryTests : ServiceSpecifications<IAggregateRepository<User, UserId>>
    {
        [Fact]
        public async Task OnCreate_Should_Write_And_Deserialize_Data()
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
            
            var result = await When(async repository => await repository.Get(userId, CancellationToken.None));

            Then(repository =>
            {
                result.Should().NotBeNull();
                result.Id.Should().Be(userId);
                result.Fullname.Should().Be(fullname);
                result.Email.Should().Be(email);
                result.Password.Should().Be(password);
            });
        }
        
        [Fact]
        public async Task OnUpdatePassword_Should_Write_And_Deserialize_Data()
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

            var updatedPassword = Password.Create(Faker.Random.Word());
            
            var result = await When(async repository =>
            {
                var savedUser = await repository.Get(userId, CancellationToken.None);
                savedUser.UpdatePassword(updatedPassword);
                await repository.Save(savedUser, CancellationToken.None);

                return await repository.Get(userId, CancellationToken.None);
            });

            Then(repository =>
            {
                result.Should().NotBeNull();
                result.Id.Should().Be(userId);
                result.CommittedEvents
                    .Should()
                    .NotBeNullOrEmpty()
                    .And
                    .HaveCount(2);
                result.CommittedEvents
                    .FirstOrDefault(e => e.GetType() == typeof(UserCreatedEvent))
                    .Should().NotBeNull();
                result.CommittedEvents
                    .FirstOrDefault(e => e.GetType() == typeof(UserPasswordUpdatedEvent))
                    .Should().NotBeNull();

            });
        }
    }
}