namespace GovITHub.Auth.Common.Data.Contract
{
    public interface IOrganizationUserRepository
    {
        ModelQuery<OrganizationUser> Filter(ModelQueryFilter filter, long organizationId);

        OrganizationUser Find(long id, long organizationId);

        void Update(OrganizationUser organizationUser, long organizationId);
    }

    public class OrganizationUser
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Models.OrganizationUserLevel Level { get; set; }

        public short Status { get; set; }
    }
}