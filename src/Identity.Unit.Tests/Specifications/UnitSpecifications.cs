using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Xunit;

namespace Identity.Unit.Tests
{
    public abstract class UnitSpecifications<T> where T : class
    {
        private readonly Dictionary<string, object> _services;
        private T _sut;
        protected T SystemUnderTest => _sut;

        public UnitSpecifications()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization());
            _services = new Dictionary<string, object>();
        }

        protected IFixture Fixture { get; }

        protected Mock<TMock> Register<TMock>(Action<Mock<TMock>> action) where TMock : class
        {
            var mock = Fixture.FreezeMoq<TMock>();
            AddOrUpdateService(mock);

            action(mock);
            return mock;
        }

        protected TService Register<TService>(Func<TService> func)
        {
            var service = Fixture.Freeze<TService>(composer => composer.FromFactory(func));
            AddOrUpdateService(service);

            return service;
        }

        private void AddOrUpdateService<TService>(TService service)
        {
            var key = service.GetType().Name;

            if (_services.ContainsKey(key)) _services[key] = service;
            else _services.Add(key, service);
        }

        protected T Given(Action<T> action)
        {
            _sut = Fixture.Create<T>();
            action(_sut);
            return _sut;
        }

        protected T When(Action<T> action)
        {
            action(_sut);
            return _sut;
        }

        protected T ExceptionWhen<TException>(Action<T> action)
            where TException : Exception
        {
            Assert.Throws<TException>(() => action(_sut));
            return _sut;
        }

        protected T Then(Action<T> action)
        {
            action(_sut);
            return _sut;
        }
    }
}