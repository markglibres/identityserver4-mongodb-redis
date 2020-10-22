using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Application.Abstractions;
using Identity.Application.Users.CreateUser;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using MediatR;
using Xunit;

namespace Identity.Integration.Tests
{
    public class MediatRTests: ServiceSpecifications<IMediator>
    {
        [Fact]
        public async Task OnPublishCommand_Should_BeHandled_And_WriteToEventStore()
        {
            Given(handler =>
            {
                
            });
            
            var command = new CreateUserCommand
            {
                Id = Guid.NewGuid(),
                Email = "me@example.com",
                Firstname = "Mark",
                Lastname = "Libres",
                TenantId = "dev",
                PlainPassword = "secret"
            };

            var id = await When(async mediator => await mediator.Send(command, CancellationToken.None));

            await Then<IEventsRepository<IDomainEvent>>(async (handler, repository) =>
            {
                var userId = AggregateGuid.For<UserId>("dev", id);
                var record = await repository.Get(userId.StreamName, CancellationToken.None);
                
                record.Should().NotBeNull();
                record.Should().HaveCount(1);
            });
        }
    }
}