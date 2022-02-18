using System.ComponentModel.DataAnnotations;

namespace Turing.Core.Models
{
    public class CredentialsModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
