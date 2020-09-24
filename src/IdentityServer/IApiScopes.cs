using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer
{
    public interface IApiScopes
    {
        IEnumerable<ApiScope> GetApiScopes();
    }
}