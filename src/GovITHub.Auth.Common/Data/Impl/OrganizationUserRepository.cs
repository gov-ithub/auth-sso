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

        public ModelQuery<Contract.OrganizationUser> Filter(long organizationId, ModelQueryFilter filter)
        {
            IQueryable<Models.OrganizationUser> dbOrganizationUsersQuery = dbContext.OrganizationUsers.Where(x => x.OrganizationId == organizationId);

            Contract.OrganizationUser[] organizationUsers = dbOrganizationUsersQuery.Include(x => x.User).Select(x =>
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
                TotalItems = dbOrganizationUsersQuery.Count()
            };
        }

        public Contract.OrganizationUser Find(long organizationId, long id)
        {
            Models.OrganizationUser dbOrganzationUser = dbContext.OrganizationUsers.Where(x => x.OrganizationId == organizationId).Include(x => x.User).FirstOrDefault(t => t.Id == id);

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

            Models.OrganizationUser dbOrganizationUser = dbContext.OrganizationUsers.FirstOrDefault(x => x.OrganizationId == organizationUser.OrganizationId && x.Id == organizationUser.Id);
            dbOrganizationUser.Level = organizationUser.Level;
            dbOrganizationUser.Status = organizationUser.Status;
            dbOrganizationUser.UserId = applicationUser.Id;

            dbContext.SaveChanges();
        }

        public void Add(Contract.OrganizationUser organizationUser)
        {
            Common.Models.ApplicationUser applicationUser = dbContext.Users.FirstOrDefault(x => x.Email == organizationUser.Name);

            Models.OrganizationUser dbOrganizationUser = new Models.OrganizationUser();
            dbOrganizationUser.Level = organizationUser.Level;
            dbOrganizationUser.Status = organizationUser.Status;
            dbOrganizationUser.UserId = applicationUser.Id;
            dbOrganizationUser.OrganizationId = organizationUser.OrganizationId;

            dbContext.OrganizationUsers.Add(dbOrganizationUser);

            dbContext.SaveChanges();
        }

        public void Delete(long organizationId, long id)
        {
            Models.OrganizationUser dbOrganizationUser = dbContext.OrganizationUsers.FirstOrDefault(x => x.OrganizationId == organizationId && x.Id == id);
            if (dbOrganizationUser != null)
            {
                dbContext.OrganizationUsers.Remove(dbOrganizationUser);
                dbContext.SaveChanges();
            }
        }
    }
}