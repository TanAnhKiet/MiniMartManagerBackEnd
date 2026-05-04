using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Models.System
{
    public class RoleClaimsDTO
    {
        public required string Type { get; set; }
        public required string Value { get; set; }

        public string? DisplayName { get; set; }
        public bool Selected { get; set; }

    }
}
    