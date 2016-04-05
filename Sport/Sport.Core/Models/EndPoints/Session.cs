using System;
using System.Collections.Generic;

namespace Sport.Core.Models.EndPoints
{
    public class Session
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public string Image { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public List<string> Roles { get; set; }
        public string Password { get; set; }

        public bool IsExpire()
        {
            return Expires <= DateTime.Now;
        }
    }

}
