using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GovITHub.Auth.Common.Data.Models
{
    public class OrganizationSetting
    {
        public long Id { get; set; }
        public long? OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public bool AllowSelfRegister { get; set; }
        public bool UseDomainRestriction { get; set; }
        [StringLength(50)]
        [MaxLength(50)]
        public string DomainRestriction { get; set; }
        public long? EmailSettingId { get; set; }
        public EmailSetting EmailSetting { get; set; }

        public List<EmailTemplate> EmailTemplates { get; set; }
    }
}
