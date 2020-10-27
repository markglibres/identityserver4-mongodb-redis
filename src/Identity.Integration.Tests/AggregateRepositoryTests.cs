using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Application.Abstractions;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Identity.Domain.ValueObjects;
using Xunit;

namespace Identity.Integration.Tests
{
    public class AggregateRepositoryTests : ServiceSpecifications<IAggregateRepository<UserAggregate, UserId>>
    {
        [Fact]
        public async Task Should_Be_Able_To_Serialize_And_Deserialize_Data()
        {
            var userId = AggregateGuid.Create<UserId>("dev");
            var fullname = Fullname.Create("Mark","Libres");
            var email = Email.Create("me@example.com");
            var password = Password.Create("secret");
            
            await GivenAsync(async repository =>
            {
                var aggregate = new UserAggregate(userId);
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
    }
}