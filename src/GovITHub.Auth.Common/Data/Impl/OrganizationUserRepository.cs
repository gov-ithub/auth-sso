using System.Linq;

namespace GovITHub.Auth.Common.Data.Impl
{
    public class OrganizationUserRepository : IOrganizationUserRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OrganizationUserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ModelQuery<OrganizationUserViewModel> Filter(ModelQueryFilter filter)
        {
            OrganizationUserViewModel[] organizationUsers = dbContext.OrganizationUsers.Apply(filter).ToArray().Select(x => new OrganizationUserViewModel()
            {
                Id = x.Id,
                Level = x.Level,
                OrganizationId = x.OrganizationId,
                Status = x.Status
            }).ToArray();

            return new ModelQuery<OrganizationUserViewModel>()
            {
                List = organizationUsers,
                TotalItems = dbContext.OrganizationUsers.Count()
            };
        }
    }
}