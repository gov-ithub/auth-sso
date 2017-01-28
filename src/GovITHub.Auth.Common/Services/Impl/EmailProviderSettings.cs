﻿using Newtonsoft.Json;

namespace GovITHub.Auth.Common.Services.Impl
{
    /// <summary>
    /// Email provider settings
    /// </summary>
    public class EmailProviderSettings
    {
        public string Address { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public bool UseSSL { get; set; }

        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public string ProviderName { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
