namespace GovITHub.Auth.Common.Data.Models
{
    public class OrganizationClient
    {
        public int ClientId { get; set; }
        public long OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
