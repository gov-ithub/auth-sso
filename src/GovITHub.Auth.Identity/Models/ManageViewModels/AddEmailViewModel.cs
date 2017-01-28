using System.ComponentModel.DataAnnotations;

namespace GovITHub.Auth.Identity.Models.ManageViewModels
{
    public class AddEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
