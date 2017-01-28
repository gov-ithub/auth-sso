using GovITHub.Auth.Common.Data.Models;

namespace GovITHub.Auth.Common.Models
{
    public class OrganizationUserModel
    {
        public OrganizationUserModel()
        {
        }

        public OrganizationUserModel(
            string userId,
            string userName,
            long? organizationId,
            OrganizationUserLevel level,
            short status)
        {
            UserId = userId;
            UserName = userName;
            OrganizationId = organizationId;
            Level = level;
            Status = status;
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public long? OrganizationId { get; set; }
        public OrganizationUserLevel Level { get; set; }
        public short Status { get; set; }
    }
}
