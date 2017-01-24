namespace GovITHub.Auth.Common.Data.Contract
{
    public interface IOrganizationUserRepository
    {
        ModelQuery<OrganizationUser> Filter(ModelQueryFilter filter);

        OrganizationUser Find(long id);

        void Update(OrganizationUser organizationUser);
    }

    public class OrganizationUser
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Models.OrganizationUserLevel Level { get; set; }

        public short Status { get; set; }
    }
}