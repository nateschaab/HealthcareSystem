using HealthcareSystem.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DBAccess.DAL
{
    public class AppointmentDAL
    {
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

                int appointmentId = this.GenerateAppointmentId();
                int visitId = this.GenerateVisitId();

                using var transaction = connection.BeginTransaction();

                string appointmentQuery = @"
                    INSERT INTO appointment (appt_id, doctor_id, patient_id, datetime, appt_reason, visit_id)
                    VALUES (@appt_id, @doctor_id, @patient_id, @datetime, @appt_reason, @visit_id);";

                using var appointmentCommand = new MySqlCommand(appointmentQuery, connection, transaction);
                appointmentCommand.Parameters.AddWithValue("@appt_id", appointmentId);
                appointmentCommand.Parameters.AddWithValue("@doctor_id", doctorId);
                appointmentCommand.Parameters.AddWithValue("@patient_id", patientId);
                appointmentCommand.Parameters.AddWithValue("@datetime", appointmentDateTime);
                appointmentCommand.Parameters.AddWithValue("@appt_reason", reason);
                appointmentCommand.Parameters.AddWithValue("@visit_id", visitId);

                int appointmentRowsAffected = appointmentCommand.ExecuteNonQuery();

                string visitQuery = @"
                    INSERT INTO visit (visit_id, appt_id, datetime)
                    VALUES (@visit_id, @appt_id, @datetime);";

                using var visitCommand = new MySqlCommand(visitQuery, connection, transaction);
                visitCommand.Parameters.AddWithValue("@visit_id", visitId);
                visitCommand.Parameters.AddWithValue("@appt_id", appointmentId);
                visitCommand.Parameters.AddWithValue("@datetime", appointmentDateTime);

                int visitRowsAffected = visitCommand.ExecuteNonQuery();

                if (appointmentRowsAffected > 0 && visitRowsAffected > 0)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAppointment: {ex.Message}");
                return false;
            }
        }

        private bool IsDoubleBooking(int doctorId, int patientId, DateTime appointmentDateTime)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();

            DateTime startTime = appointmentDateTime.AddMinutes(-20);
            DateTime endTime = appointmentDateTime.AddMinutes(20);

            string query = @"
                SELECT COUNT(*) FROM appointment 
                WHERE (doctor_id = @doctor_id OR patient_id = @patient_id)
                AND datetime BETWEEN @start_time AND @end_time;";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@doctor_id", doctorId);
            command.Parameters.AddWithValue("@patient_id", patientId);
            command.Parameters.AddWithValue("@start_time", startTime);
            command.Parameters.AddWithValue("@end_time", endTime);

            int count = Convert.ToInt32(command.ExecuteScalar());

            return count > 0;
        }

        private int GenerateAppointmentId()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }

        private int GenerateVisitId()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }

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

            DateTime defaultDob = new DateTime(1600, 12, 31);
            if (patient.DateOfBirth.Date != defaultDob)
            {
                query += " AND per.dob = @dob";
            }

            Debug.WriteLine("Executing query: " + query);
            Debug.WriteLine($"Parameter firstName: {patient.FirstName}");
            Debug.WriteLine($"Parameter lastName: {patient.LastName}");
            Debug.WriteLine($"Parameter dob: {patient.DateOfBirth.Date}");

            using var command = new MySqlCommand(query, connection);

            if (!string.IsNullOrEmpty(patient.FirstName))
            {
                command.Parameters.Add(new MySqlParameter("@firstName", MySqlDbType.VarChar) { Value = patient.FirstName });
            }

            if (!string.IsNullOrEmpty(patient.LastName))
            {
                command.Parameters.Add(new MySqlParameter("@lastName", MySqlDbType.VarChar) { Value = patient.LastName });
            }

            if (patient.DateOfBirth.Date != defaultDob)
            {
                command.Parameters.Add(new MySqlParameter("@dob", MySqlDbType.Date) { Value = patient.DateOfBirth.Date });
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
                );

                appointment.DoctorName = reader.GetString(doctorNameOrdinal);
                appointment.PatientName = reader.GetString(patientNameOrdinal);
                appointment.PatientDOB = reader.GetDateTime(patientDobOrdinal);

                appointmentList.Add(appointment);
            }

            return appointmentList;
        }


        public bool EditAppointment(int appointmentId, int doctorId, int patientId, DateTime newAppointmentDateTime, string newReason, bool isDateTimeChanged)
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

                string updateAppointmentQuery = @"
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

                int appointmentRowsAffected = appointmentCommand.ExecuteNonQuery();

                string updateVisitQuery = @"
                    UPDATE visit
                    SET datetime = @datetime
                    WHERE appt_id = @appt_id;";

                using var visitCommand = new MySqlCommand(updateVisitQuery, connection, transaction);
                visitCommand.Parameters.AddWithValue("@datetime", newAppointmentDateTime);
                visitCommand.Parameters.AddWithValue("@appt_id", appointmentId);

                int visitRowsAffected = visitCommand.ExecuteNonQuery();

                if (appointmentRowsAffected > 0)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditAppointment: {ex.Message}");
                return false;
            }
        }

    }
}