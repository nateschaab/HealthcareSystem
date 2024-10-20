using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareSystem.Model
{
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Account(string username, string password, string firstname, string lastname) 
        {
            this.Username = username;
            this.Password = password;
            this.FirstName = firstname;
            this.LastName = lastname;
        }
    }
}
