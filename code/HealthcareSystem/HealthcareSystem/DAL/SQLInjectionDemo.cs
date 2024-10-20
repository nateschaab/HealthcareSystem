using System;
using MySql.Data.MySqlClient;

namespace DBAccess.DAL
{
    public class SqlInjectionDemo
    {
        /// <summary>
        /// check if the username and password is valid -- prone to SQL Injection attacks
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns>true if it's valid login, false, otherwise</returns>
        public bool IsLoginValid_SqlInjectionDemo(string username, string password)
        {
            var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();
            var badquery = "select count(*) from login where username ='" + username +
                           "' and password ='" + password + "';";

            var command = new MySqlCommand(badquery, connection);

            var count = (long)command.ExecuteScalar();
            return count != 0;

            //while (reader.Read())
            //{
            //    var userNameOrdinal = reader.GetOrdinal("username");
            //    var userName = reader.GetString(userNameOrdinal);
            //    return userName != null;
            //}

            // return false;
        }


        /// <summary>
        /// check if the username and password is valid -- mitigates SQL Injection attacks
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns>true if it's valid login, false, otherwise</returns>
        public bool IsLoginValid(string username, string password)
        {
            var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();
           
            var goodQuery = "select count(*) from login where username = @username and password =@password;";

            MySqlCommand command = new MySqlCommand(goodQuery, connection);
            //obsolete
            // cmd.Parameters.Add("@username", username);

            //not recommended
            //cmd.Parameters.AddWithValue("@username", username);
            //cmd.Parameters.AddWithValue("@password", password);

            command.Parameters.Add("@username", MySqlDbType.VarChar);
            command.Parameters["@username"].Value = username;

            command.Parameters.Add("@password", MySqlDbType.VarChar);
            command.Parameters["@password"].Value = password;

            var count = Convert.ToInt32(command.ExecuteScalar());

            return count == 1;
        }

    }
}

