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

        public ConfigurationDataInitializer(ConfigurationDbContext configContext, PersistedGrantDbContext prstContext, ConfigCommon config)
        {
            cfgDbContext = configContext;
            prstDbContext = prstContext;
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
                foreach (var client in config.GetClients())
                {
                    cfgDbContext.Clients.Add(client.ToEntity());
                }
                cfgDbContext.SaveChanges();
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
