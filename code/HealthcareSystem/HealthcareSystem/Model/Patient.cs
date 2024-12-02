using System;
using DBAccess.Model;

namespace HealthcareSystem.Model
{
    /// <summary>
    ///     Represents a patient in the healthcare system, inheriting from <see cref="Person" />.
    ///     Includes patient-specific properties and constructors for different contexts.
    /// </summary>
    public class Patient : Person
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the unique identifier for the patient in the healthcare system.
        /// </summary>

        public int PatientId { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the person associated with this patient.
        /// </summary>

        public int PersonId { get; set; }

        /// <summary>
        ///     Gets or sets the phone number of the patient.
        /// </summary>

        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Gets or sets the current appointment associated with the patient.
        /// </summary>

        public Appointment Appointment { get; set; }

        /// <summary>
        ///     Gets the display name of the patient in the format "PatientId : FirstName, LastName".
        /// </summary>

        public bool IsActive { get; set; }

        public string DisplayName => $"{this.PatientId} : {FirstName}, {LastName}";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Patient" /> class with essential details.
        /// </summary>
        /// <param name="ssn">The Social Security Number of the patient.</param>
        /// <param name="gender">The gender of the patient.</param>
        /// <param name="fname">The first name of the patient.</param>
        /// <param name="lname">The last name of the patient.</param>
        /// <param name="dob">The date of birth of the patient.</param>
        /// <param name="address">The mailing address of the patient.</param>
        /// <param name="phoneNumber">The phone number of the patient.</param>
        public Patient(string ssn, string gender, string fname, string lname, DateTime dob, MailingAddress address,
            string phoneNumber) :
            base(ssn, gender, fname, lname, dob, address)
        {
            this.PhoneNumber = phoneNumber;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Patient" /> class with additional database identifiers.
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient.</param>
        /// <param name="pid">The unique identifier for the person associated with the patient.</param>
        /// <param name="phoneNumber">The phone number of the patient.</param>
        /// <param name="ssn">The Social Security Number of the patient.</param>
        /// <param name="gender">The gender of the patient.</param>
        /// <param name="fname">The first name of the patient.</param>
        /// <param name="lname">The last name of the patient.</param>
        /// <param name="dob">The date of birth of the patient.</param>
        /// <param name="address">The mailing address of the patient.</param>
        public Patient(int patientId, int pid, string phoneNumber, string ssn, string gender, string fname,
            string lname, DateTime dob, MailingAddress address) :
            base(ssn, gender, fname, lname, dob, address)
        {
            this.PatientId = patientId;
            this.PersonId = pid;
            this.PhoneNumber = phoneNumber;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Patient" /> class with additional appointment details.
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient.</param>
        /// <param name="pid">The unique identifier for the person associated with the patient.</param>
        /// <param name="phoneNumber">The phone number of the patient.</param>
        /// <param name="ssn">The Social Security Number of the patient.</param>
        /// <param name="gender">The gender of the patient.</param>
        /// <param name="fname">The first name of the patient.</param>
        /// <param name="lname">The last name of the patient.</param>
        /// <param name="dob">The date of birth of the patient.</param>
        /// <param name="address">The mailing address of the patient.</param>
        /// <param name="app">The appointment details associated with the patient.</param>
        public Patient(int patientId, int pid, string phoneNumber, string ssn, string gender, string fname,
            string lname, DateTime dob, MailingAddress address, Appointment app) :
            base(ssn, gender, fname, lname, dob, address)
        {
            this.PatientId = patientId;
            this.PersonId = pid;
            this.PhoneNumber = phoneNumber;
            this.Appointment = app;
        }

        #endregion
    }
}