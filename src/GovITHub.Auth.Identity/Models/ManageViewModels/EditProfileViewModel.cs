using GovITHub.Auth.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace GovITHub.Auth.Identity.Models.ManageViewModels
{
    public class EditProfileViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Display(Name = "Birth date")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "County")]
        public string County { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Street address")]
        public string StreetAddress { get; set; }
    }
}
