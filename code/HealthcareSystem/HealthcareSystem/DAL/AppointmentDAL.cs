using System;
using System.Collections.Generic;
using HealthcareSystem.Model;
using MySql.Data.MySqlClient;

namespace HealthcareSystem.DAL
{
    /// <summary>
    ///     Data access layer for managing appointment-related database operations.
    /// </summary>
    public class AppointmentDAL
    {
        #region Methods

        /// <summary>
        ///     Creates a new appointment in the database.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor for the appointment.</param>
        /// <param name="patientId">The ID of the patient for the appointment.</param>
        /// <param name="appointmentDateTime">The date and time of the appointment.</param>
        /// <param name="reason">The reason for the appointment.</param>
        /// <returns><c>true</c> if the appointment is successfully created; otherwise, <c>false</c>.</returns>
        public bool CreateAppointment(int doctorId, int patientId, DateTime appointmentDateTime, string reason)
        {
            if (this.IsDoubleBooking(doctorId, patientId, appointmentDateTime))
            {
                Console.WriteLine("Double booking detected. Appointment creation aborted.");
                return false;
            }

            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                var appointmentId = this.GenerateAppointmentId();
                var visitId = this.GenerateVisitId();

                using var transaction = connection.BeginTransaction();

                var appointmentQuery = @"
                    INSERT INTO appointment (appt_id, doctor_id, patient_id, datetime, appt_reason, visit_id)
                    VALUES (@appt_id, @doctor_id, @patient_id, @datetime, @appt_reason, @visit_id);";

                using var appointmentCommand = new MySqlCommand(appointmentQuery, connection, transaction);
                appointmentCommand.Parameters.AddWithValue("@appt_id", appointmentId);
                appointmentCommand.Parameters.AddWithValue("@doctor_id", doctorId);
                appointmentCommand.Parameters.AddWithValue("@patient_id", patientId);
                appointmentCommand.Parameters.AddWithValue("@datetime", appointmentDateTime);
                appointmentCommand.Parameters.AddWithValue("@appt_reason", reason);
                appointmentCommand.Parameters.AddWithValue("@visit_id", visitId);

                var appointmentRowsAffected = appointmentCommand.ExecuteNonQuery();

                var visitQuery = @"
                    INSERT INTO visit (visit_id, appt_id, datetime)
                    VALUES (@visit_id, @appt_id, @datetime);";

                using var visitCommand = new MySqlCommand(visitQuery, connection, transaction);
                visitCommand.Parameters.AddWithValue("@visit_id", visitId);
                visitCommand.Parameters.AddWithValue("@appt_id", appointmentId);
                visitCommand.Parameters.AddWithValue("@datetime", appointmentDateTime);

                var visitRowsAffected = visitCommand.ExecuteNonQuery();

                if (appointmentRowsAffected > 0 && visitRowsAffected > 0)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAppointment: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Checks if there is a double booking for a doctor or patient at the specified time.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor.</param>
        /// <param name="patientId">The ID of the patient.</param>
        /// <param name="appointmentDateTime">The proposed appointment date and time.</param>
        /// <returns><c>true</c> if a double booking is detected; otherwise, <c>false</c>.</returns>
        private bool IsDoubleBooking(int doctorId, int patientId, DateTime appointmentDateTime)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();

            var startTime = appointmentDateTime.AddMinutes(-20);
            var endTime = appointmentDateTime.AddMinutes(20);

            var query = @"
                SELECT COUNT(*) FROM appointment 
                WHERE (doctor_id = @doctor_id OR patient_id = @patient_id)
                AND datetime BETWEEN @start_time AND @end_time;";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@doctor_id", doctorId);
            command.Parameters.AddWithValue("@patient_id", patientId);
            command.Parameters.AddWithValue("@start_time", startTime);
            command.Parameters.AddWithValue("@end_time", endTime);

            var count = Convert.ToInt32(command.ExecuteScalar());

            return count > 0;
        }

        /// <summary>
        ///     Generates a random appointment ID.
        /// </summary>
        /// <returns>A randomly generated appointment ID.</returns>
        private int GenerateAppointmentId()
        {
            var random = new Random();
            return random.Next(100000, 999999);
        }

        /// <summary>
        ///     Generates a random visit ID.
        /// </summary>
        /// <returns>A randomly generated visit ID.</returns>
        private int GenerateVisitId()
        {
            var random = new Random();
            return random.Next(100000, 999999);
        }

        /// <summary>
        ///     Retrieves a list of appointments for a given patient.
        /// </summary>
        /// <param name="patient">The patient whose appointments are to be retrieved.</param>
        /// <returns>A list of <see cref="Appointment" /> objects associated with the patient.</returns>
        public List<Appointment> GetPatientAppointments(Patient patient)
        {
            var appointmentList = new List<Appointment>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();

            var query = @"
                SELECT 
                    a.appt_id,
                    a.doctor_id,
                    a.patient_id,
                    a.datetime,
                    a.appt_reason,
                    a.visit_id,
                    CONCAT(dper.fname, ' ', dper.lname) AS doctor_name,
                    CONCAT(per.fname, ' ', per.lname) AS patient_name,
                    per.dob AS patient_dob
                FROM 
                    appointment a
                JOIN 
                    patient p ON a.patient_id = p.patient_id
                JOIN 
                    person per ON p.pid = per.pid
                JOIN 
                    doctor d ON a.doctor_id = d.doctor_id
                JOIN 
                    person dper ON d.pid = dper.pid
                WHERE 1=1";

            if (!string.IsNullOrEmpty(patient.FirstName))
            {
                query += " AND per.fname = @firstName";
            }

            if (!string.IsNullOrEmpty(patient.LastName))
            {
                query += " AND per.lname = @lastName";
            }

            var defaultDob = new DateTime(1600, 12, 31);
            if (patient.DateOfBirth.Date != defaultDob)
            {
                query += " AND per.dob = @dob";
            }

            using var command = new MySqlCommand(query, connection);

            if (!string.IsNullOrEmpty(patient.FirstName))
            {
                command.Parameters.Add(new MySqlParameter("@firstName", MySqlDbType.VarChar)
                    { Value = patient.FirstName });
            }

            if (!string.IsNullOrEmpty(patient.LastName))
            {
                command.Parameters.Add(
                    new MySqlParameter("@lastName", MySqlDbType.VarChar) { Value = patient.LastName });
            }

            if (patient.DateOfBirth.Date != defaultDob)
            {
                command.Parameters.Add(
                    new MySqlParameter("@dob", MySqlDbType.Date) { Value = patient.DateOfBirth.Date });
            }

            using var reader = command.ExecuteReader();

            var apptIdOrdinal = reader.GetOrdinal("appt_id");
            var doctorIdOrdinal = reader.GetOrdinal("doctor_id");
            var patientIdOrdinal = reader.GetOrdinal("patient_id");
            var datetimeOrdinal = reader.GetOrdinal("datetime");
            var apptReasonOrdinal = reader.GetOrdinal("appt_reason");
            var visitIdOrdinal = reader.GetOrdinal("visit_id");
            var doctorNameOrdinal = reader.GetOrdinal("doctor_name");
            var patientNameOrdinal = reader.GetOrdinal("patient_name");
            var patientDobOrdinal = reader.GetOrdinal("patient_dob");

            while (reader.Read())
            {
                var appointment = new Appointment
                (
                    reader.GetInt32(apptIdOrdinal),
                    reader.GetInt32(visitIdOrdinal),
                    reader.GetInt32(doctorIdOrdinal),
                    reader.GetInt32(patientIdOrdinal),
                    reader.GetDateTime(datetimeOrdinal),
                    reader.IsDBNull(apptReasonOrdinal) ? null : reader.GetString(apptReasonOrdinal)
                )
                {
                    DoctorName = reader.GetString(doctorNameOrdinal),
                    PatientName = reader.GetString(patientNameOrdinal),
                    PatientDOB = reader.GetDateTime(patientDobOrdinal)
                };

                appointmentList.Add(appointment);
            }

            return appointmentList;
        }

        /// <summary>
        ///     Edits an existing appointment in the database.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to edit.</param>
        /// <param name="doctorId">The new doctor ID for the appointment.</param>
        /// <param name="patientId">The new patient ID for the appointment.</param>
        /// <param name="newAppointmentDateTime">The new date and time for the appointment.</param>
        /// <param name="newReason">The new reason for the appointment.</param>
        /// <param name="isDateTimeChanged">
        ///     <c>true</c> if the appointment date and time are being changed; otherwise, <c>false</c>
        ///     .
        /// </param>
        /// <returns><c>true</c> if the appointment is successfully edited; otherwise, <c>false</c>.</returns>
        public bool EditAppointment(int appointmentId, int doctorId, int patientId, DateTime newAppointmentDateTime,
            string newReason, bool isDateTimeChanged)
        {
            if (newAppointmentDateTime < DateTime.Now)
            {
                Console.WriteLine("Cannot edit past appointments.");
                return false;
            }

            if (isDateTimeChanged && this.IsDoubleBooking(doctorId, patientId, newAppointmentDateTime))
            {
                Console.WriteLine("Double booking detected. Appointment update aborted.");
                return false;
            }

            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                using var transaction = connection.BeginTransaction();

                var updateAppointmentQuery = @"
            UPDATE appointment
            SET doctor_id = @doctor_id,  
                datetime = @datetime, 
                appt_reason = @appt_reason
            WHERE appt_id = @appt_id;";

                using var appointmentCommand = new MySqlCommand(updateAppointmentQuery, connection, transaction);
                appointmentCommand.Parameters.AddWithValue("@doctor_id", doctorId);
                appointmentCommand.Parameters.AddWithValue("@datetime", newAppointmentDateTime);
                appointmentCommand.Parameters.AddWithValue("@appt_reason", newReason);
                appointmentCommand.Parameters.AddWithValue("@appt_id", appointmentId);

                var appointmentRowsAffected = appointmentCommand.ExecuteNonQuery();

                var updateVisitQuery = @"
            UPDATE visit
            SET datetime = @datetime
            WHERE appt_id = @appt_id;";

                using var visitCommand = new MySqlCommand(updateVisitQuery, connection, transaction);
                visitCommand.Parameters.AddWithValue("@datetime", newAppointmentDateTime);
                visitCommand.Parameters.AddWithValue("@appt_id", appointmentId);

                var visitRowsAffected = visitCommand.ExecuteNonQuery();

                if (appointmentRowsAffected > 0)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditAppointment: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}