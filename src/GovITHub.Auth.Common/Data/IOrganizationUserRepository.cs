namespace GovITHub.Auth.Common.Data
{
    public interface IOrganizationUserRepository
    {
        ModelQuery<OrganizationUserViewModel> Filter(ModelQueryFilter filter);
    }
}