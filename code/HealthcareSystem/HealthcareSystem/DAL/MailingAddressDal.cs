﻿using DBAccess.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;


namespace DBAccess.DAL
{
    public class MailingAddressDal
    {
        /// <summary>
        /// Get all the employees of the given department
        /// </summary>
        /// <param name="dno">department number</param>
        /// <returns> all the employees of the given department</returns>
        public List<MailingAddress> GetMailingAddressesOfPerson(string ssn)
        {
            var employeeList = new List<MailingAddress>();
            var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select street_address, city, state, zip , country from mailing_address where ssn = @ssn;";

            var command = new MySqlCommand(query, connection);
            command.Parameters.Add("@ssn", (DbType)MySqlDbType.String).Value = ssn;

            var reader = command.ExecuteReader();
            var streetAddressOrdinal = reader.GetOrdinal("street_address");
            var cityOrdinal = reader.GetOrdinal("city");
            var stateOrdinal = reader.GetOrdinal("state");
            var zipOrdinal = reader.GetOrdinal("zip");
            var countryOrdinal = reader.GetOrdinal("country");

            while (reader.Read())
            {
                employeeList.Add(CreateMailingAddress(reader, streetAddressOrdinal, cityOrdinal, stateOrdinal, countryOrdinal, zipOrdinal));
            }

            return employeeList;

        }

       

        /// <summary>
        /// Retrieve data using the connected model : data reader
        /// 
        /// </summary>
        /// <returns> all the employees</returns>
        public List<MailingAddress> GetMailingAddressesFromReader()
        {
            var employeeList = new List<MailingAddress>();
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select street_address, city, state, country, zip from mailing_address;";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();
            var streetAddressOrdinal = reader.GetOrdinal("street_address");
            Debug.WriteLine("Street Address Ordinal: " + streetAddressOrdinal);
            var cityOrdinal = reader.GetOrdinal("city");
            Debug.WriteLine("City Ordinal: " + cityOrdinal);
            var stateOrdinal = reader.GetOrdinal("state");
            Debug.WriteLine("State Ordinal: " + stateOrdinal);
            var countryOrdinal = reader.GetOrdinal("country");
            Debug.WriteLine("Zip Ordinal: " + countryOrdinal);
            var zipOrdinal = reader.GetOrdinal("zip");
            Debug.WriteLine("Zip Ordinal: " + zipOrdinal);


            while (reader.Read())
            {
                employeeList.Add(CreateMailingAddress(reader, streetAddressOrdinal, cityOrdinal, stateOrdinal, countryOrdinal, zipOrdinal));

            }
            
            return employeeList;
        }

        private static MailingAddress CreateMailingAddress(MySqlDataReader reader, int streetAddressOrdinal, int cityOrdinal, int stateOrdinal, int countryOrdinal, int zipOrdinal)
        {
            return new MailingAddress
            (
                reader.GetString(streetAddressOrdinal),
                reader.GetString(cityOrdinal), //reader.GetString(firstnameOrdinal),
                reader.GetString(stateOrdinal), //reader.IsDBNull(birthdateOrdinal) ? (DateTime?)null : reader.GetDateTime(birthdateOrdinal)
                reader.GetString(zipOrdinal),
                reader.GetString(countryOrdinal)
            );
        }

        /// <summary>
        /// Demo getting Scalar value (e.g. a single value) from the DB.
        /// Just use the ExecuteScalar() of the Command class.
        /// 
        /// </summary>
        /// <returns>total count of employees of the DB</returns>
        public int GetMailingAddressCount()
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select count(*) from mailing_address;";

            using var command = new MySqlCommand(query, connection);
            var count = Convert.ToInt32(command.ExecuteScalar());
            
            return count;
        }

        /// <summary>
        /// Demo the disconnected model
        /// </summary>
        /// <returns> list of all employees</returns>
        public List<MailingAddress> GetMailingAddressesFromDataSet()

        {
            var employeeList = new List<MailingAddress>();

            using var connection = new MySqlConnection(Connection.ConnectionString());

            var query = "select city, state, zip from mailing_address;";

            using var adapter = new MySqlDataAdapter(query, connection);

            var table = new DataTable();
            adapter.Fill(table);
            
            return employeeList;

        }

        public List<MailingAddress> GetMailingAddressBy(char gender, double salary)
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
                //employeeList.Add(CreateMailingAddress(reader, firstnameOrdinal, birthdateOrdinal, departmentNumberOrdinal));
            }

            return employeeList;
        }
    }
}
