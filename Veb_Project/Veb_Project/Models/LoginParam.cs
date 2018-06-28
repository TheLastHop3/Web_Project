using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veb_Project.Models
{
    public class LoginParam
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Logged { get; set; } = false;
    }
}