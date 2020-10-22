using AutoFixture;
using FluentAssertions;
using Identity.Domain;
using Identity.Domain.Abstractions;
using Identity.Domain.Extensions;
using Identity.Domain.User;
using Identity.Domain.User.Events;
using Identity.Domain.ValueObjects;
using Xunit;

namespace Identity.Unit.Tests
{
    public class UserTests : AggregateSpecification<UserAggregate>
    {
        public UserTests()
        {
            Register(() => TenantId.From("dev"));
            Register(() => AggregateGuid.For<UserId>("dev"));
            Register(() => new UserAggregate(Fixture.Create<UserId>()));
        }
        
        [Fact]
        public void Should_CreateEvent_And_Apply_To_Aggregate()
        {
            Given(aggregate => { });

            var fullname = new Fullname("Mark Gil", "Libres");
            var email = new Email("me@example.com");
            var password = new Password("etc");
            
            When(aggregate =>
            {
                aggregate.Create(
                    fullname,
                    email,
                    password);
            });

            Then(aggregate =>
            {
                aggregate.Fullname.Should().Be(fullname);
                aggregate.Email.Should().Be(email);
                aggregate.Password.Should().Be(password);
            });

            Then<UserCreatedEvent>((aggregate, @event) =>
            {
                @event.Firstname.Should().Be("Mark Gil");
                @event.Lastname.Should().Be("Libres");
                @event.Email.Should().Be("me@example.com");
                @event.Password.Should().Be("etc".ToSha256());
            });
        }
    }
}