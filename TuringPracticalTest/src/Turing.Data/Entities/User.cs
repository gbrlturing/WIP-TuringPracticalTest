using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Turing.Data.Entities
{
    public class User : IdentityUser<string>
    {
        public string Bio { get; set; }

        public string Image { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
