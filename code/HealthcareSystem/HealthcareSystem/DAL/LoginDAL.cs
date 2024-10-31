using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace DBAccess.DAL
{
    public class LoginDAL
    {
        public (bool isValid, string firstName, string lastName) ValidateLoginAndGetName(string username, string password)
        {
            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                var query = "SELECT fname, lname FROM account WHERE username = @username AND password = @password;";

                using var command = new MySqlCommand(query, connection);

                command.Parameters.Add("@username", (DbType)MySqlDbType.VarChar);
                command.Parameters["@username"].Value = username;

                command.Parameters.Add("@password", (DbType)MySqlDbType.VarChar);
                command.Parameters["@password"].Value = password;

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string firstName = reader["fname"].ToString();
                    string lastName = reader["lname"].ToString();

                    return (true, firstName, lastName);
                }

                return (false, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ValidateLoginAndGetName: {ex.Message}");
                return (false, null, null);
            }
        }
    }
}