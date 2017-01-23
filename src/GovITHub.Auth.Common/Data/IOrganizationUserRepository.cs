using GovITHub.Auth.Common.Data.Models;

namespace GovITHub.Auth.Common.Data
{
    public interface IOrganizationUserRepository
    {
        ModelQuery<OrganizationUserViewModel> Filter(ModelQueryFilter filter);

        OrganizationUser Find(long id);
    }
}