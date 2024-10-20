using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareSystem.Model
{
    public class Patient : Person
    {
        public string PatientId { get; set; }
        public string PersonId { get; set; }
        public string PhoneNumber { get; set; }

        public Patient (string ssn, string gender, string fname, string lname, DateTime dob, string streetAddress, string zip, string city, string state, string country, string phoneNumber) : 
            base(ssn, gender, fname, lname, dob, streetAddress, zip, city, state, country)
        {
            this.PhoneNumber = phoneNumber;
        }
    }
}
