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

            // Updated query to retrieve all necessary fields
            var query = @"
        SELECT 
            p.patient_id, 
            p.pid, 
            p.phone_number,
            per.ssn,
            per.fname, 
            per.lname, 
            per.dob, 
            per.gender, 
            ma.street_address, 
            ma.zip, 
            ma.city, 
            ma.state, 
            ma.country
        FROM 
            patient p
        JOIN 
            person per ON p.pid = per.pid
        JOIN 
            mailing_address ma ON per.street_address = ma.street_address AND per.zip = ma.zip;";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            // Get ordinal positions for all necessary fields
            var ssnOrdinal = reader.GetOrdinal("ssn");
            var firstNameOrdinal = reader.GetOrdinal("fname");
            var lastNameOrdinal = reader.GetOrdinal("lname");
            var genderOrdinal = reader.GetOrdinal("gender");
            var dobOrdinal = reader.GetOrdinal("dob");
            var patientIdOrdinal = reader.GetOrdinal("patient_id");
            var pidOrdinal = reader.GetOrdinal("pid");
            var phoneNumberOrdinal = reader.GetOrdinal("phone_number");
            var streetAddressOrdinal = reader.GetOrdinal("street_address");
            var zipOrdinal = reader.GetOrdinal("zip");
            var cityOrdinal = reader.GetOrdinal("city");
            var stateOrdinal = reader.GetOrdinal("state");
            var countryOrdinal = reader.GetOrdinal("country");

            while (reader.Read())
            {
                patientList.Add(CreatePatient(
                    reader,
                    ssnOrdinal,
                    patientIdOrdinal,
                    pidOrdinal,
                    phoneNumberOrdinal,
                    firstNameOrdinal,
                    lastNameOrdinal,
                    dobOrdinal,
                    genderOrdinal,
                    streetAddressOrdinal,
                    zipOrdinal,
                    cityOrdinal,
                    stateOrdinal,
                    countryOrdinal
                ));
            }

            return patientList;
        }

        private static Patient CreatePatient(
            MySqlDataReader reader,
            int ssnOrdinal,
            int patientIdOrdinal,
            int pidOrdinal,
            int phoneNumberOrdinal,
            int firstNameOrdinal,
            int lastNameOrdinal,
            int dobOrdinal,
            int genderOrdinal,
            int streetAddressOrdinal,
            int zipOrdinal,
            int cityOrdinal,
            int stateOrdinal,
            int countryOrdinal)
        {
            return new Patient
            (
                reader.GetInt32(patientIdOrdinal),           
                reader.GetInt32(pidOrdinal),                     
                reader.GetString(phoneNumberOrdinal),
                reader.GetString(ssnOrdinal),
                reader.GetString(genderOrdinal),
                reader.GetString(firstNameOrdinal),          
                reader.GetString(lastNameOrdinal),
                reader.GetDateTime(dobOrdinal),
                reader.GetString(streetAddressOrdinal),
                reader.GetString(zipOrdinal),                  
                reader.GetString(cityOrdinal),                  
                reader.GetString(stateOrdinal),                 
                reader.GetString(countryOrdinal)
            );
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

        public void UpdatePatientInDatabase(Patient patient)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());

            try
            {
                connection.Open();

                // Debugging: Output the connection state and information
                Debug.WriteLine($"Connection Opened: {connection.State == System.Data.ConnectionState.Open}");

                var query = @"
                    UPDATE person 
                    SET fname = @fname, lname = @lname, dob = @dob, street_address = @street_address, zip = @zip
                    WHERE pid = @pid;

                    UPDATE Patient
                    SET phone_number = @phoneNumber
                    WHERE patient_id = @patientId;

                    UPDATE mailing_address
                    SET city = @city, state = @state, country = @country
                    WHERE street_address = @street_address AND zip = @zip;
                    ";

                using var command = new MySqlCommand(query, connection);

                // Debugging: Output the parameter values before execution
                Debug.WriteLine($"FirstName: {patient.FirstName}");
                Debug.WriteLine($"LastName: {patient.LastName}");
                Debug.WriteLine($"Date of Birth: {patient.DateOfBirth}");
                Debug.WriteLine($"Street Address: {patient.MailAddress.StreetAddress}");
                Debug.WriteLine($"Zip Code: {patient.MailAddress.Zip}");
                Debug.WriteLine($"Person ID (pid): {patient.PersonId}");
                Debug.WriteLine($"Phone Number: {patient.PhoneNumber}");
                Debug.WriteLine($"Patient ID: {patient.PatientId}");

                // Set parameters for the person table update
                command.Parameters.AddWithValue("@fname", patient.FirstName);
                command.Parameters.AddWithValue("@lname", patient.LastName);
                command.Parameters.AddWithValue("@dob", patient.DateOfBirth);
                command.Parameters.AddWithValue("@street_address", patient.MailAddress.StreetAddress);
                command.Parameters.AddWithValue("@zip", patient.MailAddress.Zip);
                command.Parameters.AddWithValue("@pid", patient.PersonId);  // Correctly set the pid for the person table

                // Set parameters for the patient table update
                command.Parameters.AddWithValue("@phoneNumber", patient.PhoneNumber);
                command.Parameters.AddWithValue("@patientId", patient.PatientId);  // Correctly set the patient_id for the patient table

                // Debugging: Output the command text
                Debug.WriteLine("Executing SQL Query:");
                Debug.WriteLine(command.CommandText);

                // Execute the query
                var rowsAffected = command.ExecuteNonQuery();

                // Debugging: Output the result of the query execution
                Debug.WriteLine($"Rows affected: {rowsAffected}");
                Debug.WriteLine("Patient updated successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating patient: {ex.Message}");
            }
        }
    }
}

