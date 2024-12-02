using System;
using System.Data;
using DBAccess.DAL;
using MySql.Data.MySqlClient;

namespace HealthcareSystem.DAL
{
    /// <summary>
    ///     Provides data access functionality for user login operations.
    /// </summary>
    public class LoginDAL
    {
        #region Methods

        /// <summary>
        ///     Validates the login credentials for a user and retrieves their first and last name if the credentials are valid.
        /// </summary>
        /// <param name="username">The username of the account to validate.</param>
        /// <param name="password">The password of the account to validate.</param>
        /// <returns>
        ///     A tuple containing:
        ///     <list type="bullet">
        ///         <item><c>isValid</c>: A boolean indicating whether the login credentials are valid.</item>
        ///         <item><c>firstName</c>: The first name of the user, if the credentials are valid; otherwise, <c>null</c>.</item>
        ///         <item><c>lastName</c>: The last name of the user, if the credentials are valid; otherwise, <c>null</c>.</item>
        ///     </list>
        /// </returns>
        public (bool isValid, string firstName, string lastName, string role) ValidateLoginAndGetName(string username,
            string password)
        {
            try
            {
                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                var query = "SELECT fname, lname, role FROM account WHERE username = @username AND password = @password;";

                using var command = new MySqlCommand(query, connection);

                command.Parameters.Add("@username", (DbType)MySqlDbType.VarChar);
                command.Parameters["@username"].Value = username;

                command.Parameters.Add("@password", (DbType)MySqlDbType.VarChar);
                command.Parameters["@password"].Value = password;

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var firstName = reader["fname"].ToString();
                    var lastName = reader["lname"].ToString();
                    var role = reader["role"].ToString();

                    return (true, firstName, lastName, role);
                }

                return (false, null, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ValidateLoginAndGetName: {ex.Message}");
                return (false, null, null, null);
            }
        }

        #endregion
    }
}