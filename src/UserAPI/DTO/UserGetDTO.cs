using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.DTO
{
    public class UserGetDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}