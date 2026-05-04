using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Models.IdentityModels
{
    public class LoginRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
