using MySql.Data.MySqlClient;
using System;

namespace DBAccess.DAL
{
    public class AppointmentDAL
    {
        public bool CreateAppointment(int doctorId, int patientId, DateTime appointmentDateTime, string reason)
        {
            // Check for double booking before attempting to create an appointment
            if (IsDoubleBooking(doctorId, patientId, appointmentDateTime))
            {
                Console.WriteLine("Double booking detected. Appointment creation aborted.");
                return false;
            }

            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                int appointmentId = GenerateAppointmentId();
                int visitId = GenerateVisitId();

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

            // Define the time window (20 minutes before and after the requested time)
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

            // Return true if any existing appointment falls within the 20-minute window
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
    }
}
