using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GovITHub.Auth.Common.Data.Impl
{
    public class OrganizationUserRepository : Contract.IOrganizationUserRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OrganizationUserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ModelQuery<Contract.OrganizationUser> Filter(ModelQueryFilter filter)
        {
            Contract.OrganizationUser[] organizationUsers = dbContext.OrganizationUsers.Include(x => x.User).Select(x =>
                new
                {
                    Id = x.Id,
                    Name = x.User.Email,
                    Level = x.Level,
                    Status = x.Status
                }).Apply(filter).ToArray().Select(x => new Contract.OrganizationUser()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Level = x.Level,
                    Status = x.Status
                }).ToArray();

            return new ModelQuery<Contract.OrganizationUser>()
            {
                List = organizationUsers,
                TotalItems = dbContext.OrganizationUsers.Count()
            };
        }

        public Contract.OrganizationUser Find(long id)
        {
            Models.OrganizationUser dbOrganzationUser = dbContext.OrganizationUsers.Include(x => x.User).FirstOrDefault(t => t.Id == id);

            return new Contract.OrganizationUser()
            {
                Id = dbOrganzationUser.Id,
                Name = dbOrganzationUser.User.Email,
                Level = dbOrganzationUser.Level,
                Status = dbOrganzationUser.Status
            };
        }

        public void Update(Contract.OrganizationUser organizationUser)
        {
            Common.Models.ApplicationUser applicationUser = dbContext.Users.FirstOrDefault(x => x.Email == organizationUser.Name);

            Models.OrganizationUser dbOrganizationUser = dbContext.OrganizationUsers.Find(organizationUser.Id);
            dbOrganizationUser.Level = organizationUser.Level;
            dbOrganizationUser.Status = organizationUser.Status;
            dbOrganizationUser.UserId = applicationUser.Id;

            dbContext.SaveChanges();
        }
    }
}