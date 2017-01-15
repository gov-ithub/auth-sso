using GovITHub.Auth.Common.Data.Models;
using GovITHub.Auth.Common.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GovITHub.Auth.Common.Data.Impl
{
    public class OrganizationRepository : IOrganizationRepository, IDisposable
    {
        ApplicationDbContext _dbContext;
        ConfigurationDbContext _configDbContext;

        public OrganizationRepository(ApplicationDbContext dbContext, ConfigurationDbContext configDbContext)
        {
            _dbContext = dbContext;
            _configDbContext = configDbContext;
        }

        public ModelQuery<OrganizationViewModel> Filter(ModelQueryFilter filter)
        {
            if (filter == null) // get all
                return new ModelQuery<OrganizationViewModel>()
                {
                    List = _dbContext.Organizations.Select(t => new OrganizationViewModel()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Website = t.Website,
                        ParentOrganizationId = t.ParentId
                    }),
                    TotalItems = _dbContext.Organizations.Count()
                };

            var query = _dbContext.Organizations.Select(t => new OrganizationViewModel()
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
                    query = query.OrderBy(filter.SortBy);
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
            if (_dbContext.Organizations.Any(t => t.Name == item.Name))
                throw new ArgumentException(string.Format("Organization {0} already exists!", item.Name));
            bool attachedToRootOrganization = false;
            if (!item.ParentOrganizationId.HasValue)
            {
                var rootOrg = _dbContext.Organizations.FirstOrDefault(t => t.ParentId == null);
                if (rootOrg == null)
                    throw new Exception("Root organization does not exists!");
                item.ParentOrganizationId = rootOrg.Id;
                attachedToRootOrganization = true;
            }
            var adminUser = _dbContext.Users.FirstOrDefault(t => t.UserName == adminUserName);
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
                _dbContext.Organizations.Add(newOrganization);
                _dbContext.SaveChanges();
                item.Id = newOrganization.Id;
            }
        }


        public Organization Find(long id)
        {
            Organization item = _dbContext.Organizations.FirstOrDefault(t => t.Id == id);
            return item;
        }

        public Organization Remove(long id)
        {
            Organization item = _dbContext.Organizations.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                _dbContext.Organizations.Remove(item);
                _dbContext.SaveChanges();
            }
            return item;
        }

        public void Update(Organization item)
        {
            Organization itemDb = _dbContext.Organizations.FirstOrDefault(t => t.Id == item.Id);
            if (item != null)
            {
                itemDb.Name = item.Name;
                itemDb.Website = item.Website;
                _dbContext.SaveChanges();
            }
        }

        public Organization GetByClientId(string clientId)
        {
            if(string.IsNullOrEmpty(clientId))
            {
                return null;
            }

            var client = _configDbContext.Clients.FirstOrDefault(p => p.ClientId.Equals(clientId));
            if(client != null)
            {
                var org = (from x in _dbContext.Organizations
                           join y in _dbContext.OrganizationClients on x.Id equals y.OrganizationId
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
                    _dbContext.Dispose();
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
            var orgUser = _dbContext.OrganizationUsers.FirstOrDefault(t => t.User.UserName == userName);
            return orgUser != null ?
                new OrganizationUserModel(orgUser.UserId, userName, orgUser.OrganizationId, orgUser.Level, orgUser.Status) : null;
        }
        #endregion
    }

}
