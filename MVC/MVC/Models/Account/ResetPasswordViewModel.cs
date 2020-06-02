using Generic.Attributes;
using HBS.LocalizedValidationAttributes.Kentico.MVC;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace Generic.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Current Password")]
        [LocalizedCurrentUserPasswordValid(ErrorMessage = "{$ validation.invalidpassword $}")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [LocalizedPasswordPolicy]
        [DisplayName("New Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [LocalizedPasswordPolicy]
        [DisplayName("Confirm New Password")]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}