using DBAccess.Model;
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
        public Appointment Appointment { get; set; }
        public string DisplayName => $"{this.PatientId} : {FirstName} {LastName}";

        public Patient (string ssn, string gender, string fname, string lname, DateTime dob, MailingAddress address, string phoneNumber) : 
            base(ssn, gender, fname, lname, dob, address)
        {
            this.PhoneNumber = phoneNumber;
        }

        public Patient(int patientId, int pid, string phoneNumber, string ssn, string gender, string fname, string lname, DateTime dob, MailingAddress address) :
             base(ssn, gender, fname, lname, dob, address)
        {
            this.PatientId = patientId;
            this.PersonId = pid;
            this.PhoneNumber = phoneNumber;
        }

        public Patient(int patientId, int pid, string phoneNumber, string ssn, string gender, string fname, string lname, DateTime dob, MailingAddress address, Appointment app) :
             base(ssn, gender, fname, lname, dob, address)
        {
            this.PatientId = patientId;
            this.PersonId = pid;
            this.PhoneNumber = phoneNumber;
            this.Appointment = app;
        }
    }
}
