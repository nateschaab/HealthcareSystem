using MySql.Data.MySqlClient;

namespace DBAccess.DAL
{
    public static class Connection
    {
        public static string ConnectionString()
        {
            var builder = new MySqlConnectionStringBuilder();

            builder.Server = "cs-dblab01.uwg.westga.edu";    // MySQL server address
            builder.Database = "cs3230f24e";        // Database name
            builder.UserID = "cs3230f24e";          // MySQL username
            builder.Password = "ZNHDMCh/nV6p}tneR.6H";        // MySQL password
            builder.Port = 3306;                    // MySQL port (default: 3306)
            builder.SslMode = MySqlSslMode.None;
            builder.AllowUserVariables = true;

            return builder.ToString();
        }
    }
}
