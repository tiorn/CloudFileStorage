using System.ComponentModel.DataAnnotations;

namespace CloudFileStorage.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter correct username")]
        [MaxLength(20, ErrorMessage = "Username must be less than 20 characters long")]
        [MinLength(3, ErrorMessage = "Username must be more than 3 characters long")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter password")]
        [MaxLength(20, ErrorMessage = "Password must be less than 20 characters long")]
        [MinLength(3, ErrorMessage = "Password must be more than 3 characters long")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password mismatch")]
        public string PasswordConfirm { get; set; }
    }
}
