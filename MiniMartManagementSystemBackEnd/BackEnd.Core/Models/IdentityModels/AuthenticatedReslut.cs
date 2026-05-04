using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Models.IdentityModels
{
    public class AuthenticatedResult
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
