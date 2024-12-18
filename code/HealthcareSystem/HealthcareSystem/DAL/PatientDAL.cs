﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using DBAccess.DAL;
using DBAccess.Model;
using HealthcareSystem.Model;
using MySql.Data.MySqlClient;

namespace HealthcareSystem.DAL
{
    /// <summary>
    ///     Provides data access methods for managing patients in the database.
    /// </summary>
    public class PatientDal
    {
        #region Methods

        /// <summary>
        ///     Retrieves all patients from the database using a data reader.
        /// </summary>
        /// <returns>A list of <see cref="Patient" /> objects representing all patients in the database.</returns>
        public List<Patient> GetPatientsFromReader()
        {
            var patientList = new List<Patient>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();

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
                    ma.country,
                    p.is_active  -- Include is_active
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
            var isActiveOrdinal = reader.GetOrdinal("is_active");


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
                    countryOrdinal,
                    isActiveOrdinal
                ));
            }

            return patientList;
        }

        /// <summary>
        ///     Creates a <see cref="Patient" /> object from a database record.
        /// </summary>
        /// <param name="reader">The <see cref="MySqlDataReader" /> containing the data.</param>
        /// <param name="ssnOrdinal">The ordinal position of the SSN column.</param>
        /// <param name="patientIdOrdinal">The ordinal position of the patient ID column.</param>
        /// <param name="pidOrdinal">The ordinal position of the person ID column.</param>
        /// <param name="phoneNumberOrdinal">The ordinal position of the phone number column.</param>
        /// <param name="firstNameOrdinal">The ordinal position of the first name column.</param>
        /// <param name="lastNameOrdinal">The ordinal position of the last name column.</param>
        /// <param name="dobOrdinal">The ordinal position of the date of birth column.</param>
        /// <param name="genderOrdinal">The ordinal position of the gender column.</param>
        /// <param name="streetAddressOrdinal">The ordinal position of the street address column.</param>
        /// <param name="zipOrdinal">The ordinal position of the ZIP code column.</param>
        /// <param name="cityOrdinal">The ordinal position of the city column.</param>
        /// <param name="stateOrdinal">The ordinal position of the state column.</param>
        /// <param name="countryOrdinal">The ordinal position of the country column.</param>
        /// <param name="isActiveOrdinal">The ordinal position of the country column.</param>

        /// <returns>A <see cref="Patient" /> object populated with the data from the record.</returns>
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
            int countryOrdinal,
            int isActiveOrdinal) // New Parameter
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
            )
            {
                IsActive = reader.GetBoolean(isActiveOrdinal) // Set IsActive
            };
        }

        /// <summary>
        ///     Registers a new patient in the database.
        /// </summary>
        /// <param name="patient">The <see cref="Patient" /> object containing patient details.</param>
        /// <returns><c>true</c> if the patient was registered successfully; otherwise, <c>false</c>.</returns>
        public bool RegisterPatient(Patient patient)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());

            try
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                Debug.WriteLine($"Connection Opened: {connection.State == ConnectionState.Open}");

                var insertAddressQuery = @"
                    INSERT INTO mailing_address (street_address, zip, city, state, country)
                    VALUES (@street_address, @zip, @city, @state, @country);
                ";

                using (var insertAddressCommand = new MySqlCommand(insertAddressQuery, connection, transaction))
                {
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@street_address", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.StreetAddress });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@street_address", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.StreetAddress });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@zip", MySqlDbType.String)
                        { Value = patient.MailAddress.Zip });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@city", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.City });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@state", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.State });
                    insertAddressCommand.Parameters.Add(new MySqlParameter("@country", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.Country });

                    Debug.WriteLine("Inserting Mailing Address without PID with the following values:");
                    Debug.WriteLine($"Street Address: {patient.MailAddress.StreetAddress}");
                    Debug.WriteLine($"Zip: {patient.MailAddress.Zip}");
                    Debug.WriteLine($"City: {patient.MailAddress.City}");
                    Debug.WriteLine($"State: {patient.MailAddress.State}");
                    Debug.WriteLine($"Country: {patient.MailAddress.Country}");

                    insertAddressCommand.ExecuteNonQuery();
                    Debug.WriteLine("Mailing address inserted successfully.");
                }

                var insertPersonQuery = @"
                    INSERT INTO person (ssn, fname, lname, gender, dob, street_address, zip)
                    VALUES (@ssn, @fname, @lname, @gender, @dob, @street_address, @zip);
                    SELECT LAST_INSERT_ID();
                ";

                using (var insertPersonCommand = new MySqlCommand(insertPersonQuery, connection, transaction))
                {
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@ssn", MySqlDbType.String)
                        { Value = patient.SSN });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@fname", MySqlDbType.VarChar)
                        { Value = patient.FirstName });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@lname", MySqlDbType.VarChar)
                        { Value = patient.LastName });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@gender", MySqlDbType.String)
                        { Value = patient.Gender });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@dob", MySqlDbType.Date)
                        { Value = patient.DateOfBirth });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@street_address", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.StreetAddress });
                    insertPersonCommand.Parameters.Add(new MySqlParameter("@zip", MySqlDbType.String)
                        { Value = patient.MailAddress.Zip });

                    patient.PersonId = Convert.ToInt32(insertPersonCommand.ExecuteScalar());
                    Debug.WriteLine($"Generated Person ID (pid): {patient.PersonId}");
                }

                var updateAddressQuery = @"
                    UPDATE mailing_address
                    SET pid = @pid
                    WHERE street_address = @street_address AND zip = @zip;
                ";

                using (var updateAddressCommand = new MySqlCommand(updateAddressQuery, connection, transaction))
                {
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@pid", MySqlDbType.Int32)
                        { Value = patient.PersonId });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@street_address", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.StreetAddress });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@zip", MySqlDbType.String)
                        { Value = patient.MailAddress.Zip });

                    Debug.WriteLine("Updating Mailing Address with the following values:");
                    Debug.WriteLine($"pid: {patient.PersonId}");
                    Debug.WriteLine($"Street Address: {patient.MailAddress.StreetAddress}");
                    Debug.WriteLine($"Zip: {patient.MailAddress.Zip}");

                    updateAddressCommand.ExecuteNonQuery();
                    Debug.WriteLine("Mailing address updated with PID successfully.");
                }

                var insertPatientQuery = @"
                    INSERT INTO patient (pid, phone_number)
                    VALUES (@pid, @phoneNumber);
                ";

                int rowsAffected;
                using (var insertPatientCommand = new MySqlCommand(insertPatientQuery, connection, transaction))
                {
                    insertPatientCommand.Parameters.Add(new MySqlParameter("@pid", MySqlDbType.Int32)
                        { Value = patient.PersonId });
                    insertPatientCommand.Parameters.Add(new MySqlParameter("@phoneNumber", MySqlDbType.VarChar)
                        { Value = patient.PhoneNumber });

                    Debug.WriteLine("Inserting Patient Details with the following values:");
                    Debug.WriteLine($"pid: {patient.PersonId}");
                    Debug.WriteLine($"Phone Number: {patient.PhoneNumber}");

                    rowsAffected = insertPatientCommand.ExecuteNonQuery();
                    Debug.WriteLine("Patient details inserted successfully.");
                }

                if (rowsAffected > 0)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error registering patient: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Updates an existing patient's information in the database.
        /// </summary>
        /// <param name="patient">The <see cref="Patient" /> object containing the updated details.</param>
        public void UpdatePatientInDatabase(Patient patient)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());
            MySqlTransaction transaction = null;

            try
            {
                Debug.WriteLine("Attempting to open connection to the database...");
                connection.Open();
                Debug.WriteLine($"Connection opened: {connection.State == ConnectionState.Open}");

                transaction = connection.BeginTransaction();
                Debug.WriteLine("Transaction started.");

                var disableFKChecks = "SET FOREIGN_KEY_CHECKS=0;";
                using (var disableCommand = new MySqlCommand(disableFKChecks, connection, transaction))
                {
                    disableCommand.ExecuteNonQuery();
                    Debug.WriteLine("Foreign key checks disabled.");
                }

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
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@pid", MySqlDbType.Int32)
                        { Value = patient.PersonId });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@new_street_address", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.StreetAddress });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@new_zip", MySqlDbType.String)
                        { Value = patient.MailAddress.Zip });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@city", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.City });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@state", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.State });
                    updateAddressCommand.Parameters.Add(new MySqlParameter("@country", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.Country });

                    Debug.WriteLine("Executing mailing address update with parameters:");
                    Debug.WriteLine(
                        $"pid: {patient.PersonId}, Street Address: {patient.MailAddress.StreetAddress}, Zip: {patient.MailAddress.Zip}");
                    Debug.WriteLine(
                        $"City: {patient.MailAddress.City}, State: {patient.MailAddress.State}, Country: {patient.MailAddress.Country}");

                    var addressRowsAffected = updateAddressCommand.ExecuteNonQuery();
                    Debug.WriteLine($"Mailing address update completed, rows affected: {addressRowsAffected}");
                }

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
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@pid", MySqlDbType.Int32)
                        { Value = patient.PersonId });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@ssn", MySqlDbType.String)
                        { Value = patient.SSN });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@fname", MySqlDbType.VarChar)
                        { Value = patient.FirstName });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@lname", MySqlDbType.VarChar)
                        { Value = patient.LastName });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@gender", MySqlDbType.String)
                        { Value = patient.Gender });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@dob", MySqlDbType.Date)
                        { Value = patient.DateOfBirth });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@new_street_address", MySqlDbType.VarChar)
                        { Value = patient.MailAddress.StreetAddress });
                    updatePersonCommand.Parameters.Add(new MySqlParameter("@new_zip", MySqlDbType.String)
                        { Value = patient.MailAddress.Zip });

                    Debug.WriteLine("Executing person details update with parameters:");
                    Debug.WriteLine(
                        $"pid: {patient.PersonId}, SSN: {patient.SSN}, First Name: {patient.FirstName}, Last Name: {patient.LastName}");
                    Debug.WriteLine(
                        $"Gender: {patient.Gender}, Date of Birth: {patient.DateOfBirth}, Street Address: {patient.MailAddress.StreetAddress}, Zip: {patient.MailAddress.Zip}");

                    var personRowsAffected = updatePersonCommand.ExecuteNonQuery();
                    Debug.WriteLine($"Person details update completed, rows affected: {personRowsAffected}");
                }

                var updatePatientQuery = @"
                    UPDATE patient
                    SET phone_number = @phoneNumber
                    WHERE patient_id = @patientId;
                ";

                using (var updatePatientCommand = new MySqlCommand(updatePatientQuery, connection, transaction))
                {
                    updatePatientCommand.Parameters.Add(new MySqlParameter("@patientId", MySqlDbType.Int32)
                        { Value = patient.PatientId });
                    updatePatientCommand.Parameters.Add(new MySqlParameter("@phoneNumber", MySqlDbType.VarChar)
                        { Value = patient.PhoneNumber });

                    Debug.WriteLine("Executing patient details update with parameters:");
                    Debug.WriteLine($"Patient ID: {patient.PatientId}, Phone Number: {patient.PhoneNumber}");

                    var patientRowsAffected = updatePatientCommand.ExecuteNonQuery();
                    Debug.WriteLine($"Patient details update completed, rows affected: {patientRowsAffected}");
                }

                var enableFKChecks = "SET FOREIGN_KEY_CHECKS=1;";
                using (var enableCommand = new MySqlCommand(enableFKChecks, connection, transaction))
                {
                    enableCommand.ExecuteNonQuery();
                    Debug.WriteLine("Foreign key checks re-enabled.");
                }

                transaction.Commit();
                Debug.WriteLine("Transaction committed successfully. Patient updated.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error occurred while updating patient: {ex.Message}");

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

        /// <summary>
        ///     Searches for patients in the database based on the given criteria.
        /// </summary>
        /// <param name="firstName">The first name of the patient (optional).</param>
        /// <param name="lastName">The last name of the patient (optional).</param>
        /// <param name="dob">The date of birth of the patient (optional).</param>
        /// <returns>A list of <see cref="Patient" /> objects matching the search criteria.</returns>
        public List<Patient> SearchPatient(string firstName, string lastName, DateTime dob)
        {
            var patientList = new List<Patient>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            Debug.WriteLine("Opening connection to the database...");
            connection.Open();
            Debug.WriteLine("Connection opened successfully.");

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
            ma.country,
            p.is_active
        FROM 
            patient p
        JOIN 
            person per ON p.pid = per.pid
        JOIN 
            mailing_address ma ON per.street_address = ma.street_address AND per.zip = ma.zip
        WHERE 1=1";

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

            var defaultDob = new DateTime(1600, 12, 31);
            if (dob.Date != defaultDob)
            {
                query += " AND per.dob = @dob";
                Debug.WriteLine("Adding filter for dob.");
            }

            Debug.WriteLine("Executing query: " + query);
            Debug.WriteLine($"Parameter firstName: {firstName}");
            Debug.WriteLine($"Parameter lastName: {lastName}");
            Debug.WriteLine($"Parameter dob: {dob.Date}");

            using var command = new MySqlCommand(query, connection);

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
            var isActiveOrdinal = reader.GetOrdinal("is_active");


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
                    countryOrdinal,
                    isActiveOrdinal
                ));

                Debug.WriteLine("Record added to patient list.");
            }

            Debug.WriteLine("Data reading complete. Returning patient list.");
            return patientList;
        }

        public void ActivatePatient(int patientId)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();

            var query = "UPDATE patient SET is_active = TRUE WHERE patient_id = @patientId;";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.Add(new MySqlParameter("@patientId", MySqlDbType.Int32) { Value = patientId });

            command.ExecuteNonQuery();
        }

        public void DeactivatePatient(int patientId)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();

            var query = "UPDATE patient SET is_active = FALSE WHERE patient_id = @patientId;";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.Add(new MySqlParameter("@patientId", MySqlDbType.Int32) { Value = patientId });

            command.ExecuteNonQuery();
        }

        #endregion
    }
}