using GovITHub.Auth.Common.Data;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace GovITHub.Auth.Admin.Framework.Policy
{
    public class LinkedToOrganizationRequirement : IAuthorizationRequirement
    { }

    public class LinkedToOrganizationHandler : AuthorizationHandler<LinkedToOrganizationRequirement>
    {
        private readonly ApplicationDbContext dbContext;

        public LinkedToOrganizationHandler(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LinkedToOrganizationRequirement requirement)
        {
            object organizationId;
            if (((Microsoft.AspNetCore.Mvc.ActionContext)context.Resource).RouteData.Values.TryGetValue("organizationId", out organizationId) && dbContext.OrganizationUsers.Any(x => x.OrganizationId == (long)organizationId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}