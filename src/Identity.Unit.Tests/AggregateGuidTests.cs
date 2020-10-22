using System;
using FluentAssertions;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Xunit;

namespace Identity.Unit.Tests
{
    public class AggregateGuidTests : UnitSpecifications<AggregateGuid>
    {
        [Fact]
        public void OnCreate_AggregateGuid_Using_For_Should_Generate_Values()
        {
            var guid = Guid.NewGuid();
            var tenant = "dev";
            
            var id = AggregateGuid.For<UserId>("dev", guid);

            id.Should().NotBeNull();
            id.TenantId.ToString().Should().Be(tenant);
            id.Id.Should().Be(guid);
        }
        
    }
}