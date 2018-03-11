using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pas.ViewModels
{
    public class UserSession
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
    }
}
