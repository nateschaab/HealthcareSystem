using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DBAccess.DAL
{
    public class DoctorDAL
    {
        public List<string> GetAllDoctors()
        {
            var doctors = new List<string>();

            using var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();

            string query = "SELECT doctor_id, fname, lname FROM doctor JOIN person ON doctor.pid = person.pid;";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string doctorInfo = $"{reader["doctor_id"]}: {reader["fname"]} {reader["lname"]}";
                doctors.Add(doctorInfo);
            }

            return doctors;
        }
    }
}
