using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovITHub.Auth.Common.Data
{
    public class OrganizationViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }

        public long? ParentOrganizationId { get; set; }
    }
}
