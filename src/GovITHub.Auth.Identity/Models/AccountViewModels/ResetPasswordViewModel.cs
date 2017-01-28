using System.ComponentModel.DataAnnotations;

namespace GovITHub.Auth.Identity.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage ="Obligatoriu")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Obligatoriu")]
        [StringLength(100, ErrorMessage = "{0} trebuie să aiba cel puțin {2} și maximum {1} caractere.", MinimumLength = 6)]
        [Display(Name = "Parolă")]
        [DataType(DataType.Password, ErrorMessage = "Invalid")]
        public string Password { get; set; }

        [DataType(DataType.Password, ErrorMessage = "Invalid")]
        [Display(Name = "Confirmare parolă")]
        [Compare("Password", ErrorMessage = "Valoarea nu coincide cu cea a parolei.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
