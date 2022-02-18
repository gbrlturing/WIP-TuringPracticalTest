using Turing.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Turing.Api.Requests
{
    public class RegisterUserRequest
    {
        [Required]
        public RegisterUserModel User { get; set; }
    }
}
