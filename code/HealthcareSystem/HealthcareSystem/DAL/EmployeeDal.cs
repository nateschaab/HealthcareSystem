using DBAccess.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;


namespace DBAccess.DAL
{
    public class EmployeeDal
    {
        /// <summary>
        /// Get all the employees of the given department
        /// </summary>
        /// <param name="dno">department number</param>
        /// <returns> all the employees of the given department</returns>
        public List<Employee> GetEmployeesOfDepartment(int dno)
        {
            var employeeList = new List<Employee>();
            var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select fname, bdate, dno from employee where dno = @dno;";

            var command = new MySqlCommand(query, connection);
            command.Parameters.Add("@dno", MySqlDbType.Int32).Value = dno;

            var reader = command.ExecuteReader();
            var firstnameOrdinal = reader.GetOrdinal("fname");
            var birthdateOrdinal = reader.GetOrdinal("bdate");
            var departmentNumberOrdinal = reader.GetOrdinal("dno");

            while (reader.Read())
            {
                employeeList.Add(CreateEmployee(reader, firstnameOrdinal, birthdateOrdinal, departmentNumberOrdinal));
            }

            return employeeList;

        }

       

        /// <summary>
        /// Retrieve data using the connected model : data reader
        /// 
        /// </summary>
        /// <returns> all the employees</returns>
        public List<Employee> GetEmployeesFromReader()
        {
            var employeeList = new List<Employee>();
            var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select fname, bdate, dno from employee;";

            var command = new MySqlCommand(query, connection);
            var reader = command.ExecuteReader();
            var firstnameOrdinal = reader.GetOrdinal("fname");
            var birthdateOrdinal = reader.GetOrdinal("bdate");
            var departmentNumberOrdinal = reader.GetOrdinal("dno");

            while (reader.Read())
            {
                employeeList.Add(CreateEmployee(reader, firstnameOrdinal, birthdateOrdinal, departmentNumberOrdinal));

            }
            
            return employeeList;
        }

        private static Employee CreateEmployee(MySqlDataReader reader, int firstnameOrdinal, int birthdateOrdinal, int departmentNumberOrdinal)
        {
            return new Employee
            (
                reader.GetFieldValueCheckNull<string>(firstnameOrdinal), //reader.GetString(firstnameOrdinal),
                reader.GetFieldValueCheckNull<DateTime>( birthdateOrdinal), //reader.IsDBNull(birthdateOrdinal) ? (DateTime?)null : reader.GetDateTime(birthdateOrdinal)
                reader.GetFieldValueCheckNull<int>(departmentNumberOrdinal)

            );
        }

        /// <summary>
        /// Demo getting Scalar value (e.g. a single value) from the DB.
        /// Just use the ExecuteScalar() of the Command class.
        /// 
        /// </summary>
        /// <returns>total count of employees of the DB</returns>
        public int GetEmployeeCount()
        {
            var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select count(*) from employee;";

            var command = new MySqlCommand(query, connection);
            var count = Convert.ToInt32(command.ExecuteScalar());
            
            return count;
        }

        /// <summary>
        /// Demo the disconnected model
        /// </summary>
        /// <returns> list of all employees</returns>
        public List<Employee> GetEmployeesFromDataSet()

        {
            var employeeList = new List<Employee>();

            var connection = new MySqlConnection(Connection.ConnectionString());

            var query = "select fname, bdate, dno from employee;";

            var adapter = new MySqlDataAdapter(query, connection);

            var table = new DataTable();
            adapter.Fill(table);
            
            return employeeList;

        }

        public List<Employee> GetEmployeesBy(char gender, double salary)
        {
            var employeeList = new List<Employee>();
            var connection = new MySqlConnection(Connection.ConnectionString());

            connection.Open();
            var query = "select fname, bdate, dno from employee where salary > @salary AND sex = @gender";

            var command = new MySqlCommand(query, connection);
            command.Parameters.Add("@gender", MySqlDbType.VarChar).Value = gender;
            command.Parameters.Add("@salary", MySqlDbType.Double).Value = salary;

            var reader = command.ExecuteReader();
            var firstnameOrdinal = reader.GetOrdinal("fname");
            var birthdateOrdinal = reader.GetOrdinal("bdate");
            var departmentNumberOrdinal = reader.GetOrdinal("dno");

            while (reader.Read())
            {
                employeeList.Add(CreateEmployee(reader, firstnameOrdinal, birthdateOrdinal, departmentNumberOrdinal));
            }

            return employeeList;
        }
    }
}
