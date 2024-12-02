using System.Collections.Generic;
using HealthcareSystem.DAL;
using MySql.Data.MySqlClient;

namespace DBAccess.DAL
{
    /// <summary>
    /// Data access layer for doctor-related database operations.
    /// </summary>
    public class DoctorDAL
    {
        #region Methods

        /// <summary>
        /// Retrieves a list of all doctors from the database.
        /// </summary>
        /// <returns>A list of strings, where each string contains the doctor ID, first name, and last name.</returns>
        public List<string> GetAllDoctors()
        {
            var doctors = new List<string>();

            // Establish a connection to the database.
            using var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();

            // SQL query to fetch doctor details.
            var query = "SELECT doctor_id, fname, lname FROM doctor JOIN person ON doctor.pid = person.pid;";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            // Read results and build the list of doctor information.
            while (reader.Read())
            {
                var doctorInfo = $"{reader["doctor_id"]} : {reader["fname"]}, {reader["lname"]}";
                doctors.Add(doctorInfo);
            }

            return doctors;
        }

        #endregion
    }
}