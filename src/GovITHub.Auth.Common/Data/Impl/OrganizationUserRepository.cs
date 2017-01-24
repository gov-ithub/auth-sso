using GovITHub.Auth.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
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
            OrganizationUserViewModel[] organizationUsers = dbContext.OrganizationUsers.Include(x => x.User).Select(x =>
                new
                {
                    Id = x.Id,
                    Name = x.User.Email,
                    Level = x.Level,
                    Status = x.Status
                }).Apply(filter).ToArray().Select(x => new OrganizationUserViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Level = x.Level,
                    Status = x.Status
                }).ToArray();

            return new ModelQuery<OrganizationUserViewModel>()
            {
                List = organizationUsers,
                TotalItems = dbContext.OrganizationUsers.Count()
            };
        }

        public OrganizationUser Find(long id)
        {
            return dbContext.OrganizationUsers.Include(x => x.User).FirstOrDefault(t => t.Id == id);
        }
    }
}