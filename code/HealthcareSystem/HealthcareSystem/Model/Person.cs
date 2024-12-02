using System;
using DBAccess.Model;

namespace HealthcareSystem.Model
{
    /// <summary>
    ///     Represents a person in the healthcare system, including personal details and mailing address.
    /// </summary>
    public class Person
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the Social Security Number of the person.
        /// </summary>

        public string SSN { get; set; }

        /// <summary>
        ///     Gets or sets the first name of the person.
        /// </summary>

        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the person.
        /// </summary>

        public string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the gender of the person.
        /// </summary>

        public string Gender { get; set; }

        /// <summary>
        ///     Gets or sets the date of birth of the person.
        /// </summary>

        public DateTime DateOfBirth { get; set; }

        /// <summary>
        ///     Gets or sets the mailing address of the person.
        /// </summary>

        public MailingAddress MailAddress { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Person" /> class.
        /// </summary>
        /// <param name="ssn">The Social Security Number of the person.</param>
        /// <param name="gender">The gender of the person.</param>
        /// <param name="fname">The first name of the person.</param>
        /// <param name="lname">The last name of the person.</param>
        /// <param name="dob">The date of birth of the person.</param>
        /// <param name="address">The mailing address of the person.</param>
        public Person(string ssn, string gender, string fname, string lname, DateTime dob, MailingAddress address)
        {
            this.MailAddress = address;
            this.SSN = ssn;
            this.Gender = gender;
            this.FirstName = fname;
            this.LastName = lname;
            this.DateOfBirth = dob;
        }

        #endregion
    }
}