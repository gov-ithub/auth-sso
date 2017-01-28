using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace GovITHub.Auth.Common.Data
{
    public interface IConfigRepository
    {
        Task<IEnumerable<Scope>> GetScopesAsync();
        Task<Client> GetClient(string id);
    }
}
