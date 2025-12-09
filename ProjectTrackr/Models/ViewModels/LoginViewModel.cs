using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectTrackr.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "UsernameOrEmail")]
        public string usernameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
    }
}