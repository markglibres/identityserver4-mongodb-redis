using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Identity.Domain;
using Identity.Domain.Abstractions;
using Moq;
using Xunit;

namespace Identity.Unit.Tests
{
    public abstract class AggregateSpecification<T> : UnitSpecifications<T> 
        where T: class, IAggregate
    {
        protected IDomainEvent Then<TDomainEvent>(Action<T, TDomainEvent> action) where TDomainEvent: DomainEvent
        {
            var @event = SystemUnderTest.UncommittedEvents?.FirstOrDefault(e => e.GetType().Name == typeof(TDomainEvent).Name);
            action(SystemUnderTest, @event as TDomainEvent);
            return @event;
        }
    }
}