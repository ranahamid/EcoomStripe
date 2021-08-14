﻿using System.ComponentModel.DataAnnotations;

namespace SaasEcom.Core.Models
{
    public class StripeAccount
    {
        public int Id { get; set; }

        [Display(Name = "Live Mode")]
        public bool LiveMode { get; set; }

        [Display(Name = "Live public API Key")]
        public string StripeLivePublicApiKey { get; set; }
        [Display(Name = "Live secret API Key")]
        public string StripeLiveSecretApiKey { get; set; }

        [Display(Name = "Test public API Key")]
        public string StripeTestPublicApiKey { get; set; }
        [Display(Name = "Test secret API Key")]
        public string StripeTestSecretApiKey { get; set; }

        public virtual SaasEcomUser SaasEcomUser { get; set; }
    }
}
