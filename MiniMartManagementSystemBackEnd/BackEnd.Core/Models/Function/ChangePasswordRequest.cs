using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.Core.Models.Function
{
    public class ChangePasswordRequest
    {
        public Guid AppUserId { get; set; }

        public string? OldPassword { get; set; }
        public string NewPassword { get; set; } = null!;
    }
}
