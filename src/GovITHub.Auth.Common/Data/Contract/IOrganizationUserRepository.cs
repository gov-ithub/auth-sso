namespace GovITHub.Auth.Common.Data.Contract
{
    public interface IOrganizationUserRepository
    {
        ModelQuery<OrganizationUser> Filter(long organizationId, ModelQueryFilter filter);

        OrganizationUser Find(long organizationId, long id);

        ValidationError Update(OrganizationUser organizationUser);

        ValidationError Add(OrganizationUser organizationUser);

        void Delete(long organizationId, long id);
    }

    public class OrganizationUser
    {
        public long Id { get; set; }

        public long OrganizationId { get; set; }

        public string Name { get; set; }

        public Models.OrganizationUserLevel Level { get; set; }

        public short Status { get; set; }
    }
}