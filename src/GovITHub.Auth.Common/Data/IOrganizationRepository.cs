using GovITHub.Auth.Common.Data.Models;
using GovITHub.Auth.Common.Models;

namespace GovITHub.Auth.Common.Data
{
    public interface IOrganizationRepository
    {
        Organization Find(long id);
        void Update(Organization item);
        Organization Remove(long id);
        void Add(OrganizationViewModel item, string adminUserName);
        ModelQuery<OrganizationViewModel> Filter(ModelQueryFilter filter);
        OrganizationUserModel GetOrganizationUser(string name);
        Organization GetByClientId(string clientId);
    }
}
