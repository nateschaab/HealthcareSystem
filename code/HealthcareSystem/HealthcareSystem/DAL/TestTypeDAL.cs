using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DBAccess.DAL
{
    public class TestTypeDAL
    {
        public List<string> GetAllTestTypes()
        {
            var testTypes = new List<string>();

            using var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();

            string query = "SELECT test_code, test_type_name FROM test_type";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            int testCodeOrdinal = reader.GetOrdinal("test_code");
            int testTypeNameOrdinal = reader.GetOrdinal("test_type_name");

            while (reader.Read())
            {
                string testCode = reader.GetString(testCodeOrdinal);
                string testTypeName = reader.GetString(testTypeNameOrdinal);
                testTypes.Add($"{testCode}: {testTypeName}");
            }

            return testTypes;
        }
    }
}
