using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;

namespace DBAccess.DAL
{
    public class AppointmentDAL
    {
        public bool CreateAppointment(int doctorId, int patientId, DateTime dateTime, string reason)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());

            try
            {
                connection.Open();
                using var transaction = connection.BeginTransaction(); // Begin transaction

                // Step 1: Check if the doctor already has an appointment at the specified time
                var checkQuery = @"
                    SELECT COUNT(*) FROM appointment 
                    WHERE doctor_id = @doctorId AND datetime = @dateTime 
                    FOR UPDATE;"; // Add FOR UPDATE to lock the row during the transaction

                using var checkCommand = new MySqlCommand(checkQuery, connection, transaction);
                checkCommand.Parameters.AddWithValue("@doctorId", doctorId);
                checkCommand.Parameters.AddWithValue("@dateTime", dateTime);

                var isTimeSlotOccupied = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;

                // Debugging output to confirm the check result
                Debug.WriteLine("Is time slot occupied: " + isTimeSlotOccupied);

                if (isTimeSlotOccupied)
                {
                    transaction.Rollback();  // Rollback if time slot is occupied
                    return false;
                }

                // Step 2: Insert the new appointment if no conflict found
                var insertQuery = @"
                    INSERT INTO appointment (doctor_id, patient_id, datetime, appt_reason) 
                    VALUES (@doctorId, @patientId, @dateTime, @reason);";

                using var insertCommand = new MySqlCommand(insertQuery, connection, transaction);
                insertCommand.Parameters.AddWithValue("@doctorId", doctorId);
                insertCommand.Parameters.AddWithValue("@patientId", patientId);
                insertCommand.Parameters.AddWithValue("@dateTime", dateTime);
                insertCommand.Parameters.AddWithValue("@reason", reason);

                insertCommand.ExecuteNonQuery();

                transaction.Commit(); // Commit transaction if all steps succeed
                Debug.WriteLine("Appointment created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating appointment: {ex.Message}");
                return false;
            }
        }
    }
}
