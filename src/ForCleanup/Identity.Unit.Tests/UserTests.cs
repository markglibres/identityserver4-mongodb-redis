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
    public class UserTests : AggregateSpecification<User>
    {
        public UserTests()
        {
            Register(() => TenantId.Create("dev"));
            Register(() => UserId.Create("dev"));
            Register(() => new User(Fixture.Create<UserId>()));
        }
        
        [Fact]
        public void OnCreateUser_Should_Emit_UserCreatedEvent()
        {
            Given(aggregate => { });

            var fullname = Fullname.Create(Faker.Name.FirstName(), Faker.Name.LastName());
            var email = Email.Create($"{Faker.Random.Word()}@example.com");
            var password = Password.Create(Faker.Random.Word());

            var start = DateTimeUtc.Now();
            When(aggregate =>
            {
                aggregate.Create(
                    fullname,
                    email,
                    password);
            });
            var end = DateTimeUtc.Now();
            
            Then(aggregate =>
            {
                aggregate.Fullname.Should().Be(fullname);
                aggregate.Email.Should().Be(email);
                aggregate.Password.Should().Be(password);
            });

            Then<UserCreatedEvent>((aggregate, @event) =>
            {
                @event.Firstname.Should().Be(fullname.Firstname);
                @event.Lastname.Should().Be(fullname.Lastname);
                @event.Email.Should().Be(email.ToString());
                @event.Password.Should().Be(password.ToString());
                @event.CreatedOn.Should().NotBe(default);
                @event.CreatedOn
                    .Should()
                    .BeCloseTo(start.Value)
                    .And
                    .BeCloseTo(end.Value);
            });
        }

        [Fact]
        public void OnUpdatePassword_Should_Emit_UserPasswordCreatedEvent()
        {
            Given(user =>
            {
                var fullname = Fullname.Create(Faker.Name.FirstName(), Faker.Name.LastName());
                var email = Email.Create($"{Faker.Random.Word()}@example.com");
                var password = Password.Create(Faker.Random.Word());
                
                user.Create(
                    fullname,
                    email,
                    password);
            });

            var updatedPassword = Password.Create(Faker.Random.Word());
            When(user =>
            {
                user.UpdatePassword(updatedPassword);
            });

            Then(user =>
            {
                user.Password.Should().Be(updatedPassword);
            });

            Then<UserPasswordUpdatedEvent>((user, @event) =>
            {
                @event.EntityId.Should().Be(user.Id.Id.ToString());
                @event.HashedPassword.Should().Be(updatedPassword.ToString());
            });
        }
    }
}