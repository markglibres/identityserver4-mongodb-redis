using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer
{
    public interface ISeedClients
    {
        IEnumerable<Client> GetClients();
    }
}