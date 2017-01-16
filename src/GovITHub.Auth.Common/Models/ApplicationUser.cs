using System.Collections.Generic;
using GovITHub.Auth.Common.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GovITHub.Auth.Common.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public List<OrganizationUser> OrganizationUsers { get; set; }
    }
}
