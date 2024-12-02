using MySql.Data.MySqlClient;

namespace HealthcareSystem.DAL
{
    /// <summary>
    ///     Provides database connection utilities for the application.
    /// </summary>
    public static class Connection
    {
        #region Methods

        /// <summary>
        ///     Builds and returns the connection string for the MySQL database.
        /// </summary>
        /// <returns>A string representing the connection details for the database.</returns>
        public static string ConnectionString()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "cs-dblab01.uwg.westga.edu", // MySQL server address
                Database = "cs3230f24e", // Database name
                UserID = "cs3230f24e", // MySQL username
                Password = "ZNHDMCh/nV6p}tneR.6H", // MySQL password
                Port = 3306, // MySQL port (default: 3306)
                SslMode = MySqlSslMode.None,
                AllowUserVariables = true
            };

            return builder.ToString();
        }

        #endregion
    }
}