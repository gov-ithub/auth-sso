﻿namespace GovITHub.Auth.Common.Data
{
    public class OrganizationUserViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Models.OrganizationUserLevel Level { get; set; }

        public short Status { get; set; }
    }
}