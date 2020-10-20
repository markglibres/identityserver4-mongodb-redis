using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Integration.Tests
{
    public abstract class ServiceSpecifications<T> : ConfigurationSpecifications
        where T: class
    {
        private T _service;

        protected virtual T Given(Func<T, T> func)
        {
            _service = Services.GetService<T>();
            return func(_service);
        }

        protected virtual Task<TResult> When<TResult>(Func<T, Task<TResult>> func)
        {
            return func(_service);
        }

        protected virtual Task Then(Action<T> assertions)
        {
            assertions(_service);
            return Task.CompletedTask;
        }
    }
}