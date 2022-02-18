using Turing.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Turing.Api.Requests
{
    public class LoginUserRequest
    {
        [Required]
        public CredentialsModel User { get; set; }
    }
}
