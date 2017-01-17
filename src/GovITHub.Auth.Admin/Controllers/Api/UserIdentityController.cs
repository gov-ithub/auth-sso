using GovITHub.Auth.Admin.Services;
using GovITHub.Auth.Common.Data;
using GovITHub.Auth.Common.Data.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovITHub.Auth.Admin.Controllers.Api
{
    //[Authorize]
    [Route("api/[controller]")]
    public class UserIdentityController : Controller
    {
        private readonly IOrganizationRepository _organizationRepository;
        public UserIdentityController(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string userName = User.Claims.GetClaim(JwtClaimTypes.Name);
            var level = User.Claims.GetClaim<OrganizationUserLevel>(JwtClaimTypes.Role);
            long organizationId = User.Claims.GetClaim<long>("OrganizationId");
            string organizationName = _organizationRepository.Find(organizationId)?.Name;
            return Json(new { username = userName, level = level, organizationId = organizationId, organizationName = organizationName});
        }
    }
}
