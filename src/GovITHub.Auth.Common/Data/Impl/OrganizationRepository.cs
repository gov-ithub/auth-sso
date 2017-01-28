using System;
using System.Collections.Generic;
using System.Linq;
using GovITHub.Auth.Common.Data.Models;
using GovITHub.Auth.Common.Models;
using IdentityServer4.EntityFramework.DbContexts;

namespace GovITHub.Auth.Common.Data.Impl
{
    /// <summary>
    /// Organization specific repository
    /// </summary>
    public class OrganizationRepository : IOrganizationRepository, IDisposable
    {
        private ApplicationDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Application db context</param>
        public OrganizationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Filter organizations
        /// </summary>
        /// <param name="filter">filter obj</param>
        /// <returns>Model query for organization view model</returns>
        public ModelQuery<OrganizationViewModel> Filter(ModelQueryFilter filter)
        {
            // get all
            if (filter == null) 
            {
                return new ModelQuery<OrganizationViewModel>()
                {
                    List = dbContext.Organizations.Select(t => new OrganizationViewModel()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Website = t.Website,
                        ParentOrganizationId = t.ParentId
                    }),
                    TotalItems = dbContext.Organizations.Count()
                };
            }

            var query = dbContext.Organizations.Select(t => new OrganizationViewModel()
            {
                Id = t.Id,
                Name = t.Name,
                Website = t.Website,
                ParentOrganizationId = t.ParentId
            }).Skip(filter.CurrentPage * filter.ItemsPerPage)
                .Take(filter.ItemsPerPage)
                .Select(p => p);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                if (filter.SortAscending)
                {
                    query = query.OrderBy(filter.SortBy);
                }
                else
                {
                    query = query.OrderByDescending(filter.SortBy);
                }
            }

            var count = query.Count();
            return new ModelQuery<OrganizationViewModel>()
            {
                List = query.ToList(),
                TotalItems = count
            };
        }

        public void Add(OrganizationViewModel item, string adminUserName)
        {
            if (dbContext.Organizations.Any(t => t.Name == item.Name))
            {
                throw new ArgumentException(string.Format("Organization {0} already exists!", item.Name));
            }

            bool attachedToRootOrganization = false;
            if (!item.ParentOrganizationId.HasValue)
            {
                var rootOrg = dbContext.Organizations.FirstOrDefault(t => t.ParentId == null);
                if (rootOrg == null)
                {
                    throw new Exception("Root organization does not exists!");
                }

                item.ParentOrganizationId = rootOrg.Id;
                attachedToRootOrganization = true;
            }
            var adminUser = dbContext.Users.FirstOrDefault(t => t.UserName == adminUserName);
            if (adminUser == null)
            {
                throw new ArgumentException(string.Format("User {0} could not be found!", adminUserName));
            }
            else
            {
                var newOrganization = new Organization()
                {
                    Name = item.Name,
                    ParentId = item.ParentOrganizationId,
                    Website = item.Website
                };
                if (attachedToRootOrganization)
                {
                    newOrganization.Users = new List<OrganizationUser>()
                    {
                        new OrganizationUser()
                        {
                            Level = OrganizationUserLevel.Admin,
                            User = adminUser,
                        }
                    };
                }
                dbContext.Organizations.Add(newOrganization);
                dbContext.SaveChanges();
                item.Id = newOrganization.Id;
            }
        }


        public Organization Find(long id)
        {
            Organization item = dbContext.Organizations.FirstOrDefault(t => t.Id == id);
            return item;
        }

        public Organization Remove(long id)
        {
            Organization item = dbContext.Organizations.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                dbContext.Organizations.Remove(item);
                dbContext.SaveChanges();
            }
            return item;
        }

        public void Update(Organization item)
        {
            Organization itemDb = dbContext.Organizations.FirstOrDefault(t => t.Id == item.Id);
            if (item != null)
            {
                itemDb.Name = item.Name;
                itemDb.Website = item.Website;
                dbContext.SaveChanges();
            }
        }

        public Organization GetByClientId(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return null;
            }

            var client = dbContext.Clients.FirstOrDefault(p => p.ClientId.Equals(clientId));
            if (client != null)
            {
                var org = (from x in dbContext.Organizations
                           join y in dbContext.OrganizationClients on x.Id equals y.OrganizationId
                           where y.ClientId.Equals(client.Id)
                           select x).FirstOrDefault();
                return org;
            }

            return null;
        }

        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public OrganizationUserModel GetOrganizationUser(string userName)
        {
            var orgUser = dbContext.OrganizationUsers.FirstOrDefault(t => t.User.UserName == userName);
            return orgUser != null ?
                new OrganizationUserModel(orgUser.UserId, userName, orgUser.OrganizationId, orgUser.Level, orgUser.Status) : null;
        }
        #endregion
    }

}
