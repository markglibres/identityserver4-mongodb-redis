using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Integration.Tests
{
    public abstract class ServiceSpecifications<T> : ConfigurationSpecifications
        where T: class
    {
        private T _service;

        protected virtual void Given(Action<T> func)
        {
            _service = Services.GetService<T>();
            func(_service);
        }
        
        protected virtual async Task GivenAsync(Func<T, Task> func)
        {
            _service = Services.GetService<T>();
            await func(_service);
        }

        protected virtual async Task<TResult> When<TResult>(Func<T, Task<TResult>> func)
        {
            return await func(_service);
        }

        protected virtual async Task Then(Func<T, Task> assertions)
        {
            await assertions(_service);
        }
        
        protected virtual void Then(Action<T> assertions)
        {
            assertions(_service);
        }
        
        protected virtual async Task Then<TService>(Func<T, TService, Task>  assertions)
        {
            var service = Services.GetService<TService>();
            await assertions(_service, service);
        }
    }
}