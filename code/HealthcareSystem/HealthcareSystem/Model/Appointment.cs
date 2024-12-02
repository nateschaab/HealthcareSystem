using System;

namespace HealthcareSystem.Model
{
    /// <summary>
    ///     Represents a scheduled appointment in the healthcare system, including details about the doctor, patient, and
    ///     visit.
    /// </summary>
    public class Appointment
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the unique identifier for the appointment.
        /// </summary>

        public int AppointmentId { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the visit associated with this appointment.
        /// </summary>

        public int VisitId { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the doctor associated with this appointment.
        /// </summary>

        public int DoctorId { get; set; }

        /// <summary>
        ///     Gets or sets the full name of the doctor associated with this appointment.
        /// </summary>

        public string DoctorName { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the patient associated with this appointment.
        /// </summary>

        public int PatientId { get; set; }

        /// <summary>
        ///     Gets or sets the full name of the patient associated with this appointment.
        /// </summary>

        public string PatientName { get; set; }

        /// <summary>
        ///     Gets or sets the date of birth of the patient associated with this appointment.
        /// </summary>

        public DateTime PatientDOB { get; set; }

        /// <summary>
        ///     Gets or sets the scheduled date and time of the appointment.
        /// </summary>

        public DateTime Date { get; set; }

        /// <summary>
        ///     Gets or sets the reason for the appointment.
        /// </summary>

        public string Reason { get; set; }

        /// <summary>
        ///     Gets the display name for the appointment, including the date, doctor name, patient name, and patient date of
        ///     birth.
        ///     Format: "Date, DoctorName, PatientName, PatientDOB".
        /// </summary>

        public string DisplayName =>
            $"{this.Date}, {this.DoctorName}, {this.PatientName}, {this.PatientDOB.ToShortDateString()}";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Appointment" /> class with the specified details.
        /// </summary>
        /// <param name="appId">The unique identifier for the appointment.</param>
        /// <param name="visitId">The unique identifier for the visit associated with this appointment.</param>
        /// <param name="doctorId">The unique identifier for the doctor associated with this appointment.</param>
        /// <param name="patientId">The unique identifier for the patient associated with this appointment.</param>
        /// <param name="date">The scheduled date and time of the appointment.</param>
        /// <param name="reason">The reason for the appointment.</param>
        public Appointment(int appId, int visitId, int doctorId, int patientId, DateTime date, string reason)
        {
            this.AppointmentId = appId;
            this.VisitId = visitId;
            this.DoctorId = doctorId;
            this.PatientId = patientId;
            this.Date = date;
            this.Reason = reason;
        }

        #endregion
    }
}