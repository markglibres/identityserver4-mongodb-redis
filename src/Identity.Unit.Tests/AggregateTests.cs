using System;
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
    public class AggregateTests : AggregateSpecification<UserAggregate>
    {
        public AggregateTests()
        {
            Register(() => TenantId.Create("dev"));
            Register(() => AggregateGuid.Create<UserId>("dev"));
            Register(() => new UserAggregate(Fixture.Create<UserId>()));
        }
        
        [Fact]
        public void Should_CreateEvent_And_Apply_To_Aggregate()
        {
            Given(aggregate => { });

            var fullname = Fullname.Create("Mark Gil", "Libres");
            var email = Email.Create("me@example.com");
            var password = Password.Create("etc");

            var startDateTime = DateTimeOffset.Now;
            When(aggregate =>
            {
                aggregate.Create(
                    fullname,
                    email,
                    password);
            });
            var endDateTime = DateTimeOffset.Now;

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
                @event.CreatedOn.Should()
                    .NotBe(default)
                    .And
                    .BeCloseTo(startDateTime)
                    .And
                    .BeCloseTo(endDateTime);
            });
        }
    }
}