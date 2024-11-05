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
using System.Transactions;


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

            var mailingAddess = new MailingAddress(
                reader.GetString(streetAddressOrdinal),
                reader.GetString(zipOrdinal),
                reader.GetString(cityOrdinal),
                reader.GetString(stateOrdinal),
                reader.GetString(countryOrdinal)
                );

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
                mailingAddess
            );
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
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@street_address", MySqlDbType.VarChar) { Value = patient.MailAddress.StreetAddress });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@street_address", MySqlDbType.VarChar) { Value = patient.MailAddress.StreetAddress });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@zip", MySqlDbType.String) { Value = patient.MailAddress.Zip });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@city", MySqlDbType.VarChar) { Value = patient.MailAddress.City });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@state", MySqlDbType.VarChar) { Value = patient.MailAddress.State });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@country", MySqlDbType.VarChar) { Value = patient.MailAddress.Country });


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
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@ssn", MySqlDbType.String) { Value = patient.SSN });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@fname", MySqlDbType.VarChar) { Value = patient.FirstName });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@lname", MySqlDbType.VarChar) { Value = patient.LastName });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@gender", MySqlDbType.String) { Value = patient.Gender });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@dob", MySqlDbType.Date) { Value = patient.DateOfBirth });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@street_address", MySqlDbType.VarChar) { Value = patient.MailAddress.StreetAddress });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@zip", MySqlDbType.String) { Value = patient.MailAddress.Zip });


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
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@pid", MySqlDbType.Int32) { Value = patient.PersonId });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@street_address", MySqlDbType.VarChar) { Value = patient.MailAddress.StreetAddress });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@zip", MySqlDbType.String) { Value = patient.MailAddress.Zip });


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
                    insertPatientCommand.Parameters.Add(new MySqlParameter("@pid", MySqlDbType.Int32) { Value = patient.PersonId });
                    insertPatientCommand.Parameters.Add(new MySqlParameter("@phoneNumber", MySqlDbType.VarChar) { Value = patient.PhoneNumber });


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
            MySqlTransaction transaction = null; // Declare transaction outside of try block

            try
            {
                Debug.WriteLine("Attempting to open connection to the database...");
                connection.Open();
                Debug.WriteLine($"Connection opened: {connection.State == System.Data.ConnectionState.Open}");

                transaction = connection.BeginTransaction(); // Initialize transaction here
                Debug.WriteLine("Transaction started.");

                // Disable foreign key checks to avoid circular dependency issues during the update
                var disableFKChecks = "SET FOREIGN_KEY_CHECKS=0;";
                using (var disableCommand = new MySqlCommand(disableFKChecks, connection, transaction))
                {
                    disableCommand.ExecuteNonQuery();
                    Debug.WriteLine("Foreign key checks disabled.");
                }

                // Step 1: Update mailing address
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
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@pid", MySqlDbType.Int32) { Value = patient.PersonId });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@new_street_address", MySqlDbType.VarChar) { Value = patient.MailAddress.StreetAddress });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@new_zip", MySqlDbType.String) { Value = patient.MailAddress.Zip });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@city", MySqlDbType.VarChar) { Value = patient.MailAddress.City });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@state", MySqlDbType.VarChar) { Value = patient.MailAddress.State });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@country", MySqlDbType.VarChar) { Value = patient.MailAddress.Country });

                    Debug.WriteLine("Executing mailing address update with parameters:");
                    Debug.WriteLine($"pid: {patient.PersonId}, Street Address: {patient.MailAddress.StreetAddress}, Zip: {patient.MailAddress.Zip}");
                    Debug.WriteLine($"City: {patient.MailAddress.City}, State: {patient.MailAddress.State}, Country: {patient.MailAddress.Country}");

                    var addressRowsAffected = updateAddressCommand.ExecuteNonQuery();
                    Debug.WriteLine($"Mailing address update completed, rows affected: {addressRowsAffected}");
                }

                // Step 2: Update person details
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
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@pid", MySqlDbType.Int32) { Value = patient.PersonId });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@ssn", MySqlDbType.String) { Value = patient.SSN });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@fname", MySqlDbType.VarChar) { Value = patient.FirstName });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@lname", MySqlDbType.VarChar) { Value = patient.LastName });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@gender", MySqlDbType.String) { Value = patient.Gender });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@dob", MySqlDbType.Date) { Value = patient.DateOfBirth });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@new_street_address", MySqlDbType.VarChar) { Value = patient.MailAddress.StreetAddress });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@new_zip", MySqlDbType.String) { Value = patient.MailAddress.Zip });

                    Debug.WriteLine("Executing person details update with parameters:");
                    Debug.WriteLine($"pid: {patient.PersonId}, SSN: {patient.SSN}, First Name: {patient.FirstName}, Last Name: {patient.LastName}");
                    Debug.WriteLine($"Gender: {patient.Gender}, Date of Birth: {patient.DateOfBirth}, Street Address: {patient.MailAddress.StreetAddress}, Zip: {patient.MailAddress.Zip}");

                    var personRowsAffected = updatePersonCommand.ExecuteNonQuery();
                    Debug.WriteLine($"Person details update completed, rows affected: {personRowsAffected}");
                }

                // Step 3: Update patient details
                var updatePatientQuery = @"
                    UPDATE patient
                    SET phone_number = @phoneNumber
                    WHERE patient_id = @patientId;
                ";

                using (var updatePatientCommand = new MySqlCommand(updatePatientQuery, connection, transaction))
                {
                    updatePatientCommand.Parameters.Add(new MySqlParameter("@patientId", MySqlDbType.Int32) { Value = patient.PatientId });
                    updatePatientCommand.Parameters.Add(new MySqlParameter("@phoneNumber", MySqlDbType.VarChar) { Value = patient.PhoneNumber });

                    Debug.WriteLine("Executing patient details update with parameters:");
                    Debug.WriteLine($"Patient ID: {patient.PatientId}, Phone Number: {patient.PhoneNumber}");

                    var patientRowsAffected = updatePatientCommand.ExecuteNonQuery();
                    Debug.WriteLine($"Patient details update completed, rows affected: {patientRowsAffected}");
                }

                // Re-enable foreign key checks after completing the update
                var enableFKChecks = "SET FOREIGN_KEY_CHECKS=1;";
                using (var enableCommand = new MySqlCommand(enableFKChecks, connection, transaction))
                {
                    enableCommand.ExecuteNonQuery();
                    Debug.WriteLine("Foreign key checks re-enabled.");
                }

                // Commit transaction
                transaction.Commit();
                Debug.WriteLine("Transaction committed successfully. Patient updated.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error occurred while updating patient: {ex.Message}");

                // Rollback transaction if it exists
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                        Debug.WriteLine("Transaction rolled back successfully.");
                    }
                    catch (Exception rollbackEx)
                    {
                        Debug.WriteLine($"Error occurred during rollback: {rollbackEx.Message}");
                    }
                }
            }
        }

        public List<Patient> SearchPatient(string firstName, string lastName, DateTime dob)
        {
            var patientList = new List<Patient>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            Debug.WriteLine("Opening connection to the database...");
            connection.Open();
            Debug.WriteLine("Connection opened successfully.");

            // Base query
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
            mailing_address ma ON per.street_address = ma.street_address AND per.zip = ma.zip
        WHERE 1=1";

            // Add conditions based on input values
            if (!string.IsNullOrEmpty(firstName))
            {
                query += " AND per.fname = @firstName";
                Debug.WriteLine("Adding filter for firstName.");
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query += " AND per.lname = @lastName";
                Debug.WriteLine("Adding filter for lastName.");
            }

            DateTime defaultDob = new DateTime(1600, 12, 31);
            if (dob.Date != defaultDob)
            {
                query += " AND per.dob = @dob";
                Debug.WriteLine("Adding filter for dob.");
            }

            // Log the query and parameter values
            Debug.WriteLine("Executing query: " + query);
            Debug.WriteLine($"Parameter firstName: {firstName}");
            Debug.WriteLine($"Parameter lastName: {lastName}");
            Debug.WriteLine($"Parameter dob: {dob.Date}");

            using var command = new MySqlCommand(query, connection);

            // Bind parameters
            if (!string.IsNullOrEmpty(firstName))
            {
                command.Parameters.Add(new MySqlParameter("@firstName", MySqlDbType.VarChar) { Value = firstName });
                Debug.WriteLine("Bound parameter @firstName.");
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                command.Parameters.Add(new MySqlParameter("@lastName", MySqlDbType.VarChar) { Value = lastName });
                Debug.WriteLine("Bound parameter @lastName.");
            }

            if (dob.Date != defaultDob)
            {
                command.Parameters.Add(new MySqlParameter("@dob", MySqlDbType.Date) { Value = dob.Date });
                Debug.WriteLine("Bound parameter @dob.");
            }

            Debug.WriteLine("Executing the command...");
            using var reader = command.ExecuteReader();

            // Retrieve ordinals to optimize reader
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

            Debug.WriteLine("Starting to read data from the database...");
            while (reader.Read())
            {
                Debug.WriteLine("Reading a record...");
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
                Debug.WriteLine("Record added to patient list.");
            }

            Debug.WriteLine("Data reading complete. Returning patient list.");
            return patientList;
        }

    }
}

