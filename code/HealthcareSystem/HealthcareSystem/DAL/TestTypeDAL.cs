using System.Collections.Generic;
using DBAccess.DAL;
using MySql.Data.MySqlClient;

namespace HealthcareSystem.DAL
{
    /// <summary>
    ///     Data access layer for managing test type-related operations in the database.
    /// </summary>
    public class TestTypeDAL
    {
        #region Methods

        /// <summary>
        ///     Retrieves all available test types from the database.
        /// </summary>
        /// <returns>
        ///     A list of strings representing test types in the format "test_code: test_type_name".
        /// </returns>
        public List<string> GetAllTestTypes()
        {
            var testTypes = new List<string>();

            using var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();

            var query = "SELECT test_code, test_type_name FROM test_type";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var testCodeOrdinal = reader.GetOrdinal("test_code");
            var testTypeNameOrdinal = reader.GetOrdinal("test_type_name");

            while (reader.Read())
            {
                var testCode = reader.GetString(testCodeOrdinal);
                var testTypeName = reader.GetString(testTypeNameOrdinal);
                testTypes.Add($"{testCode}: {testTypeName}");
            }

            return testTypes;
        }

        #endregion
    }
}