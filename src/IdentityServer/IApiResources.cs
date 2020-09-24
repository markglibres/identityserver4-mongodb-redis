using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer
{
    public interface IApiResources
    {
        IEnumerable<ApiResource> GetApiResources();
    }
}