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
                reader.GetInt32(patientIdOrdinal),            // patient_id is an integer
                reader.GetInt32(pidOrdinal),                        // pid is an integer
                reader.GetString(phoneNumberOrdinal),
                reader.GetString(ssnOrdinal),
                reader.GetString(genderOrdinal),
                reader.GetString(firstNameOrdinal),           // first name is a string
                reader.GetString(lastNameOrdinal),
                reader.GetDateTime(dobOrdinal),
                reader.GetString(streetAddressOrdinal),
                reader.GetString(zipOrdinal),                   // zip is a string
                reader.GetString(cityOrdinal),                     // city is a string
                reader.GetString(stateOrdinal),                   // state is a string
                reader.GetString(countryOrdinal)    // phone_number is a string            // last name is a              // dob is a    // street_address is a               // country is a string
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
                using var transaction = connection.BeginTransaction();

                // Debugging: Output the connection state and information
                Debug.WriteLine($"Connection Opened: {connection.State == System.Data.ConnectionState.Open}");

                // Step 1: Insert mailing address without the PID
                var insertAddressQuery = @"
            INSERT INTO mailing_address (street_address, zip, city, state, country)
            VALUES (@street_address, @zip, @city, @state, @country);
        ";

                using (var insertAddressCommand = new MySqlCommand(insertAddressQuery, connection, transaction))
                {
                    insertAddressCommand.Parameters.AddWithValue("@street_address", patient.MailAddress.StreetAddress);
                    insertAddressCommand.Parameters.AddWithValue("@zip", patient.MailAddress.Zip);
                    insertAddressCommand.Parameters.AddWithValue("@city", patient.MailAddress.City);
                    insertAddressCommand.Parameters.AddWithValue("@state", patient.MailAddress.State);
                    insertAddressCommand.Parameters.AddWithValue("@country", patient.MailAddress.Country);

                    // Debugging: Output the parameter values being added
                    Debug.WriteLine("Inserting Mailing Address without PID with the following values:");
                    Debug.WriteLine($"Street Address: {patient.MailAddress.StreetAddress}");
                    Debug.WriteLine($"Zip: {patient.MailAddress.Zip}");
                    Debug.WriteLine($"City: {patient.MailAddress.City}");
                    Debug.WriteLine($"State: {patient.MailAddress.State}");
                    Debug.WriteLine($"Country: {patient.MailAddress.Country}");

                    insertAddressCommand.ExecuteNonQuery();
                    Debug.WriteLine("Mailing address inserted successfully.");
                }

                // Step 2: Insert person details and get the generated PID
                var insertPersonQuery = @"
            INSERT INTO person (ssn, fname, lname, gender, dob, street_address, zip)
            VALUES (@ssn, @fname, @lname, @gender, @dob, @street_address, @zip);
            SELECT LAST_INSERT_ID();
        ";

                using (var insertPersonCommand = new MySqlCommand(insertPersonQuery, connection, transaction))
                {
                    insertPersonCommand.Parameters.AddWithValue("@ssn", patient.SSN);
                    insertPersonCommand.Parameters.AddWithValue("@fname", patient.FirstName);
                    insertPersonCommand.Parameters.AddWithValue("@lname", patient.LastName);
                    insertPersonCommand.Parameters.AddWithValue("@gender", patient.Gender);
                    insertPersonCommand.Parameters.AddWithValue("@dob", patient.DateOfBirth);
                    insertPersonCommand.Parameters.AddWithValue("@street_address", patient.MailAddress.StreetAddress);
                    insertPersonCommand.Parameters.AddWithValue("@zip", patient.MailAddress.Zip);

                    // Retrieve the newly generated person ID (pid)
                    patient.PersonId = Convert.ToInt32(insertPersonCommand.ExecuteScalar());
                    Debug.WriteLine($"Generated Person ID (pid): {patient.PersonId}");
                }

                // Step 3: Update mailing address with the generated PID
                var updateAddressQuery = @"
            UPDATE mailing_address
            SET pid = @pid
            WHERE street_address = @street_address AND zip = @zip;
        ";

                using (var updateAddressCommand = new MySqlCommand(updateAddressQuery, connection, transaction))
                {
                    updateAddressCommand.Parameters.AddWithValue("@pid", patient.PersonId);
                    updateAddressCommand.Parameters.AddWithValue("@street_address", patient.MailAddress.StreetAddress);
                    updateAddressCommand.Parameters.AddWithValue("@zip", patient.MailAddress.Zip);

                    // Debugging: Output the parameter values being added
                    Debug.WriteLine("Updating Mailing Address with the following values:");
                    Debug.WriteLine($"pid: {patient.PersonId}");
                    Debug.WriteLine($"Street Address: {patient.MailAddress.StreetAddress}");
                    Debug.WriteLine($"Zip: {patient.MailAddress.Zip}");

                    updateAddressCommand.ExecuteNonQuery();
                    Debug.WriteLine("Mailing address updated with PID successfully.");
                }

                // Step 4: Insert patient details
                var insertPatientQuery = @"
            INSERT INTO patient (pid, phone_number)
            VALUES (@pid, @phoneNumber);
        ";

                using (var insertPatientCommand = new MySqlCommand(insertPatientQuery, connection, transaction))
                {
                    insertPatientCommand.Parameters.AddWithValue("@pid", patient.PersonId);
                    insertPatientCommand.Parameters.AddWithValue("@phoneNumber", patient.PhoneNumber);

                    // Debugging: Output the parameter values being added
                    Debug.WriteLine("Inserting Patient Details with the following values:");
                    Debug.WriteLine($"pid: {patient.PersonId}");
                    Debug.WriteLine($"Phone Number: {patient.PhoneNumber}");

                    insertPatientCommand.ExecuteNonQuery();
                    Debug.WriteLine("Patient details inserted successfully.");
                }

                // Commit transaction
                transaction.Commit();
                Debug.WriteLine("Patient registered successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error registering patient: {ex.Message}");
            }
        }


        public void UpdatePatientInDatabase(Patient patient)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());

            try
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                // Disable foreign key checks to avoid circular dependency issues during the update
                var disableFKChecks = "SET FOREIGN_KEY_CHECKS=0;";
                using (var disableCommand = new MySqlCommand(disableFKChecks, connection, transaction))
                {
                    disableCommand.ExecuteNonQuery();
                }

                Debug.WriteLine($"Connection Opened: {connection.State == System.Data.ConnectionState.Open}");

                // Step 1: Update mailing address first to ensure no foreign key violations
                var updateAddressQuery = @"
            UPDATE mailing_address
            SET street_address = @new_street_address,
                zip = @new_zip,
                city = @city,
                state = @state,
                country = @country
            WHERE pid = @pid;
        ";

                using (var updateAddressCommand = new MySqlCommand(updateAddressQuery, connection, transaction))
                {
                    updateAddressCommand.Parameters.AddWithValue("@pid", patient.PersonId);
                    updateAddressCommand.Parameters.AddWithValue("@new_street_address", patient.MailAddress.StreetAddress);
                    updateAddressCommand.Parameters.AddWithValue("@new_zip", patient.MailAddress.Zip);
                    updateAddressCommand.Parameters.AddWithValue("@city", patient.MailAddress.City);
                    updateAddressCommand.Parameters.AddWithValue("@state", patient.MailAddress.State);
                    updateAddressCommand.Parameters.AddWithValue("@country", patient.MailAddress.Country);

                    Debug.WriteLine("Updating Mailing Address with the following values:");
                    Debug.WriteLine($"pid: {patient.PersonId}");
                    Debug.WriteLine($"Street Address: {patient.MailAddress.StreetAddress}");
                    Debug.WriteLine($"Zip: {patient.MailAddress.Zip}");
                    Debug.WriteLine($"City: {patient.MailAddress.City}");
                    Debug.WriteLine($"State: {patient.MailAddress.State}");
                    Debug.WriteLine($"Country: {patient.MailAddress.Country}");

                    var addressRowsAffected = updateAddressCommand.ExecuteNonQuery();
                    Debug.WriteLine($"Mailing address rows affected: {addressRowsAffected}");
                }

                // Step 2: Update person details to match the changes in mailing_address
                var updatePersonQuery = @"
            UPDATE person 
            SET ssn = @ssn,
                fname = @fname,
                lname = @lname,
                gender = @gender,
                dob = @dob,
                street_address = @new_street_address,
                zip = @new_zip
            WHERE pid = @pid;
        ";

                using (var updatePersonCommand = new MySqlCommand(updatePersonQuery, connection, transaction))
                {
                    updatePersonCommand.Parameters.AddWithValue("@pid", patient.PersonId);
                    updatePersonCommand.Parameters.AddWithValue("@ssn", patient.SSN);
                    updatePersonCommand.Parameters.AddWithValue("@fname", patient.FirstName);
                    updatePersonCommand.Parameters.AddWithValue("@lname", patient.LastName);
                    updatePersonCommand.Parameters.AddWithValue("@gender", patient.Gender);
                    updatePersonCommand.Parameters.AddWithValue("@dob", patient.DateOfBirth);
                    updatePersonCommand.Parameters.AddWithValue("@new_street_address", patient.MailAddress.StreetAddress);
                    updatePersonCommand.Parameters.AddWithValue("@new_zip", patient.MailAddress.Zip);

                    Debug.WriteLine("Updating Person Details with the following values:");
                    Debug.WriteLine($"pid: {patient.PersonId}");
                    Debug.WriteLine($"SSN: {patient.SSN}");
                    Debug.WriteLine($"First Name: {patient.FirstName}");
                    Debug.WriteLine($"Last Name: {patient.LastName}");
                    Debug.WriteLine($"Gender: {patient.Gender}");
                    Debug.WriteLine($"Date of Birth: {patient.DateOfBirth}");
                    Debug.WriteLine($"Street Address: {patient.MailAddress.StreetAddress}");
                    Debug.WriteLine($"Zip: {patient.MailAddress.Zip}");

                    var personRowsAffected = updatePersonCommand.ExecuteNonQuery();
                    Debug.WriteLine($"Person rows affected: {personRowsAffected}");
                }

                // Step 3: Update patient details
                var updatePatientQuery = @"
            UPDATE patient
            SET phone_number = @phoneNumber
            WHERE patient_id = @patientId;
        ";

                using (var updatePatientCommand = new MySqlCommand(updatePatientQuery, connection, transaction))
                {
                    updatePatientCommand.Parameters.AddWithValue("@patientId", patient.PatientId);
                    updatePatientCommand.Parameters.AddWithValue("@phoneNumber", patient.PhoneNumber);

                    Debug.WriteLine("Updating Patient Details with the following values:");
                    Debug.WriteLine($"Patient ID: {patient.PatientId}");
                    Debug.WriteLine($"Phone Number: {patient.PhoneNumber}");

                    var patientRowsAffected = updatePatientCommand.ExecuteNonQuery();
                    Debug.WriteLine($"Patient rows affected: {patientRowsAffected}");
                }

                // Re-enable foreign key checks after completing the update
                var enableFKChecks = "SET FOREIGN_KEY_CHECKS=1;";
                using (var enableCommand = new MySqlCommand(enableFKChecks, connection, transaction))
                {
                    enableCommand.ExecuteNonQuery();
                }

                // Commit transaction
                transaction.Commit();
                Debug.WriteLine("Patient updated successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating patient: {ex.Message}");
            }
        }
    }
}

