using Microsoft.Extensions.DependencyInjection;

namespace Identity
{
    public static class Startup
    {
        public static IMvcBuilder AddIdentity(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder.AddApplicationPart(typeof(Startup).Assembly);
        }
    }
}