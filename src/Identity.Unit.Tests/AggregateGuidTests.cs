using System;
using FluentAssertions;
using Identity.Domain.Abstractions;
using Identity.Domain.User;
using Identity.Domain.ValueObjects;
using Xunit;

namespace Identity.Unit.Tests
{
    public class AggregateGuidTests : UnitSpecifications<AggregateGuid>
    {
        [Theory]
        [InlineData("dev", "dev")]
        [InlineData(null, "Identity")]
        public void OnCreate_AggregateGuid_Using_For_Should_Generate_Values(string tenant, string expectedTenant)
        {
            var guid = Guid.NewGuid();
            
            var id = AggregateGuid.Create<UserId>(guid, tenant);

            id.Should().NotBeNull();
            id.TenantId.ToString().Should().Be(expectedTenant);
            id.Id.Should().Be(guid);
        }
        
    }
}