using DBAccess.Model;
using HealthcareSystem.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;


namespace DBAccess.DAL
{
    public class PatientDal
    {
        /// <summary>
        /// Retrieve data using the connected model : data reader
        /// 
        /// </summary>
        /// <returns> all the employees</returns>
        public List<Patient> GetPatientsFromReader()
        {
            var patientList = new List<Patient>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select city, state, zip from mailing_address;";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();
            var cityOrdinal = reader.GetOrdinal("city");
            Debug.WriteLine("City Ordinal: " + cityOrdinal);
            var stateOrdinal = reader.GetOrdinal("state");
            Debug.WriteLine("State Ordinal: " + stateOrdinal);
            var zipOrdinal = reader.GetOrdinal("zip");
            Debug.WriteLine("Zip Ordinal: " + zipOrdinal);

            while (reader.Read())
            {
                patientList.Add(CreatePatient(reader, cityOrdinal, stateOrdinal, zipOrdinal));

            }

            return patientList;
        }

        private static Patient CreatePatient(MySqlDataReader reader, int cityOrdinal, int stateOrdinal, int zipOrdinal)
        {
            //TODO
            /*return new Patient
            (
                reader.GetString(cityOrdinal), //reader.GetString(firstnameOrdinal),
                reader.GetString(stateOrdinal), //reader.IsDBNull(birthdateOrdinal) ? (DateTime?)null : reader.GetDateTime(birthdateOrdinal)
                reader.GetString(zipOrdinal)
            );*/
            return null;
        }

        /// <summary>
        /// Demo getting Scalar value (e.g. a single value) from the DB.
        /// Just use the ExecuteScalar() of the Command class.
        /// 
        /// </summary>
        /// <returns>total count of employees of the DB</returns>
        public int GetPatientCount()
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select count(*) from patient;";

            using var command = new MySqlCommand(query, connection);
            var count = Convert.ToInt32(command.ExecuteScalar());

            return count;
        }

        /// <summary>
        /// Demo the disconnected model
        /// </summary>
        /// <returns> list of all employees</returns>
        public List<Patient> GetPatientsFromDataSet()
        {
            var patientList = new List<Patient>();

            using var connection = new MySqlConnection(Connection.ConnectionString());

            var query = "select city, state, zip from mailing_address;";

            using var adapter = new MySqlDataAdapter(query, connection);

            var table = new DataTable();
            adapter.Fill(table);

            return patientList;

        }

        public List<MailingAddress> GetPatientBy(char gender, double salary)
        {
            var employeeList = new List<MailingAddress>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select city, state, zip from mailing_address where salary > @salary AND sex = @gender";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.Add("@gender", (DbType)MySqlDbType.VarChar).Value = gender;
            command.Parameters.Add("@salary", (DbType)MySqlDbType.Double).Value = salary;

            using var reader = command.ExecuteReader();
            var firstnameOrdinal = reader.GetOrdinal("fname");
            var birthdateOrdinal = reader.GetOrdinal("bdate");
            var departmentNumberOrdinal = reader.GetOrdinal("dno");

            while (reader.Read())
            {
                //TODO
                //employeeList.Add(CreatePatient(reader, firstnameOrdinal, birthdateOrdinal, departmentNumberOrdinal));
            }

            return employeeList;
        }

        public void RegisterPatient(Patient patient)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());

            try
            {
                connection.Open();
                using var transaction = connection.BeginTransaction(); // Start transaction

                // Step 1: Check if the mailing address already exists in the mailing_address table
                var checkAddressQuery = @"
            SELECT COUNT(*) FROM mailing_address 
            WHERE street_address = @street_address AND zip = @zip;";

                using var checkAddressCommand = new MySqlCommand(checkAddressQuery, connection, transaction);
                checkAddressCommand.Parameters.AddWithValue("@street_address", patient.MailAddress.StreetAddress);
                checkAddressCommand.Parameters.AddWithValue("@zip", patient.MailAddress.Zip);

                var addressExists = Convert.ToInt32(checkAddressCommand.ExecuteScalar()) > 0;

                // Step 2: If address does not exist, insert into mailing_address
                if (!addressExists)
                {
                    var insertAddressQuery = @"
                INSERT INTO mailing_address (street_address, zip, city, state, country) 
                VALUES (@street_address, @zip, @city, @state, @country);";

                    using var insertAddressCommand = new MySqlCommand(insertAddressQuery, connection, transaction);
                    insertAddressCommand.Parameters.AddWithValue("@street_address", patient.MailAddress.StreetAddress);
                    insertAddressCommand.Parameters.AddWithValue("@zip", patient.MailAddress.Zip);
                    insertAddressCommand.Parameters.AddWithValue("@city", patient.MailAddress.City);
                    insertAddressCommand.Parameters.AddWithValue("@state", patient.MailAddress.State);
                    insertAddressCommand.Parameters.AddWithValue("@country", patient.MailAddress.Country);

                    insertAddressCommand.ExecuteNonQuery();
                }

                // Step 3: Insert into Person table and get the generated pid
                var personQuery = @"
            INSERT INTO Person (ssn, gender, fname, lname, dob, street_address, zip) 
            VALUES (@ssn, @gender, @fname, @lname, @dob, @street_address, @zip);
            SELECT LAST_INSERT_ID();";  // Fetch the generated pid after insertion

                using var personCommand = new MySqlCommand(personQuery, connection, transaction);

                personCommand.Parameters.AddWithValue("@ssn", patient.SSN);
                personCommand.Parameters.AddWithValue("@gender", patient.Gender);
                personCommand.Parameters.AddWithValue("@fname", patient.FirstName);
                personCommand.Parameters.AddWithValue("@lname", patient.LastName);
                personCommand.Parameters.AddWithValue("@dob", patient.DateOfBirth);
                personCommand.Parameters.AddWithValue("@street_address", patient.MailAddress.StreetAddress);
                personCommand.Parameters.AddWithValue("@zip", patient.MailAddress.Zip);

                // Execute and retrieve the generated pid (personId)
                var personId = Convert.ToInt32(personCommand.ExecuteScalar());

                // Log the generated personId to ensure it's being retrieved correctly
                Debug.WriteLine("Generated Person ID (pid): " + personId);

                // Step 4: Insert into Patient table using the generated pid
                var patientQuery = @"
            INSERT INTO Patient (pid, phone_number) 
            VALUES (@pid, @phoneNumber);";

                using var patientCommand = new MySqlCommand(patientQuery, connection, transaction);
                patientCommand.Parameters.AddWithValue("@pid", personId);  // Use the pid from the Person table
                patientCommand.Parameters.AddWithValue("@phoneNumber", patient.PhoneNumber);

                // Execute the insert into Patient table
                patientCommand.ExecuteNonQuery();

                // Commit the transaction
                transaction.Commit();

                Debug.WriteLine("Patient registered successfully with PID: " + personId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error registering patient: " + ex.Message);
            }
        }

    }
}
