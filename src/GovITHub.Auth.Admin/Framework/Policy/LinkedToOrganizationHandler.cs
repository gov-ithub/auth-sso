using GovITHub.Auth.Admin.Services;
using GovITHub.Auth.Common.Data;
using IdentityModel;
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
            Microsoft.AspNetCore.Routing.RouteValueDictionary routeValues = ((Microsoft.AspNetCore.Mvc.ActionContext)context.Resource).RouteData.Values;

            long organizationId;
            if (context.TryGetRouteValue("organizationId", out organizationId) && UserLinkedToOrganization(context, organizationId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool UserLinkedToOrganization(AuthorizationHandlerContext context, long organizationId)
        {
            string userName = context.User.Claims.GetClaim(JwtClaimTypes.Name);

            return dbContext.OrganizationUsers.Any(x => x.OrganizationId == organizationId && x.User.UserName == userName);
        }
    }
}