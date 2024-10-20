using MySql.Data.MySqlClient;

namespace DBAccess.DAL
{
    public static class Connection
    {
        public static string ConnectionString()
        {
            var builder = new MySqlConnectionStringBuilder();

            // Set the connection string properties
            builder.Server = "cs-dblab01.uwg.westga.edu";    // MySQL server address
            builder.Database = "tdena1";        // Database name
            builder.UserID = "tdena1";          // MySQL username
            builder.Password = "Awesom44$";        // MySQL password
            builder.Port = 3306;                    // MySQL port (default: 3306)
            builder.SslMode = MySqlSslMode.None;

            // Get the constructed connection string
            return builder.ToString();

        }
    }
}
