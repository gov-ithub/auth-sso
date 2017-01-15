using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovITHub.Auth.Common
{
    public static class CommonServicesExtensions
    {
        /// <summary>
        /// Add auth common services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void AddAuthCommonServices(this IServiceCollection services, IConfigurationRoot config)
        {
            // add caching
            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = config[Config.REDIS_INSTANCE];
                options.Configuration = config[Config.REDIS_CONFIGURATION];
            });
        }
    }
}
