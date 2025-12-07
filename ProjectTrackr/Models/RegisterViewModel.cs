using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectTrackr.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string username { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordRepeat")]
        public string passwordRepeat { get; set; }
    }
}
