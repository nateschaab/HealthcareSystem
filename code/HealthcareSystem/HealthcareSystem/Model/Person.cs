﻿using DBAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareSystem.Model
{
    public class Person
    {
        public string SSN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public MailingAddress MailAddress { get; set; }

        public Person (string ssn, string gender, string fname, string lname, DateTime dob, string streetAddress, string zip, string city, string state, string country)
        {
            this.MailAddress = new MailingAddress(streetAddress, zip, city, country, state);
            this.SSN = ssn;
            this.Gender = gender;
            this.FirstName = fname;
            this.LastName = lname;
            this.DateOfBirth = dob;
        }
    }
}