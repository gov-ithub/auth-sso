namespace GovITHub.Auth.Admin.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Common.Data.Models.OrganizationUserLevel Level { get; set; }

        public UserStatus Status { get; set; }
    }

    public enum UserStatus
    {
        Inactiv = 0,
        Activ = 1,
    }
}