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
        public int PatientId { get; set; }
        public int PersonId { get; set; }
        public string PhoneNumber { get; set; }

        public Patient (string ssn, string gender, string fname, string lname, DateTime dob, string streetAddress, string zip, string city, string state, string country, string phoneNumber) : 
            base(ssn, gender, fname, lname, dob, streetAddress, zip, city, state, country)
        {
            this.PhoneNumber = phoneNumber;
        }

        public Patient(int patientId, int pid, string phoneNumber, string ssn, string gender, string fname, string lname, DateTime dob, string streetAddress, string zip, string city, string state, string country) : this(ssn, gender, fname, lname, dob, streetAddress, zip, city, state, country)
        {
            this.PatientId = patientId;
            this.PersonId = pid;
            this.PhoneNumber = phoneNumber;
        }

        public Patient(string ssn, string gender, string fname, string lname, DateTime dob, string streetAddress, string zip, string city, string state, string country) : base(ssn, gender, fname, lname, dob, streetAddress, zip, city, state, country)
        {
        }
    }
}
