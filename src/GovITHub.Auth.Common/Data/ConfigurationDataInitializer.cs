using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace GovITHub.Auth.Common.Data
{
    public class ConfigurationDataInitializer
    {
        private readonly ConfigurationDbContext cfgDbContext;
        private readonly PersistedGrantDbContext prstDbContext;
        private readonly ConfigCommon config;
        private readonly ApplicationDbContext dbContext;

        public ConfigurationDataInitializer(ConfigurationDbContext configContext, PersistedGrantDbContext prstContext, ConfigCommon config, ApplicationDbContext dbContext)
        {
            this.cfgDbContext = configContext;
            this.prstDbContext = prstContext;
            this.dbContext = dbContext;
            this.config = config;
        }

        public void InitializeData()
        {
            cfgDbContext.Database.Migrate();
            prstDbContext.Database.Migrate();
            InitializeClientsAndScopes();
        }

        private void InitializeClientsAndScopes()
        {
            if (cfgDbContext.Clients.FirstOrDefault() == null)
            {
                var mainOrg = dbContext.Organizations.FirstOrDefault(p => !p.ParentId.HasValue);
                if(mainOrg == null)
                {
                    throw new System.Exception("Main organization not set");
                }

                foreach (var client in config.GetClients())
                {
                    var clientEntity = client.ToEntity();
                    cfgDbContext.Clients.Add(clientEntity);
                    mainOrg.OrganizationClients = new System.Collections.Generic.List<Models.OrganizationClient>();
                    mainOrg.OrganizationClients.Add(new Models.OrganizationClient() { ClientId = clientEntity.Id });
                }
                cfgDbContext.SaveChanges();
                dbContext.SaveChanges();
            }

            if (cfgDbContext.ApiResources.FirstOrDefault() == null)
            {
                foreach (var apiResource in config.GetApiResources())
                {
                    cfgDbContext.ApiResources.Add(apiResource.ToEntity());
                }
                cfgDbContext.SaveChanges();
            }

            if (cfgDbContext.IdentityResources.FirstOrDefault() == null)
            {
                foreach (var identityResource in config.GetIdentityResources())
                {
                    cfgDbContext.IdentityResources.Add(identityResource.ToEntity());
                }
                cfgDbContext.SaveChanges();
            }
        }
    }
}
