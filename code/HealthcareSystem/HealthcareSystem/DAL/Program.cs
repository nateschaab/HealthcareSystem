/*using System;
using System.Collections.Generic;
using DBAccess.DAL;
using DBAccess.Model;

namespace DBAccess
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var dal = new EmployeeDal();
            var demo = new SqlInjectionDemo();
            // List<Employee> employees = dal.GetEmployeesFromDataSet();

            // you will need to create a login table with (username, password ) as the columns
            // and insert ("user1, "1234") as a row
            // demo valid user name and password
            // if (!demo.IsLoginValid_SqlInjectionDemo("user1", "1234")) 
            // demo SQL Injection attack
            //if (!demo.IsLoginValid_SqlInjectionDemo("' or '1' = '1' ; -- ", "1234")) 
            // demo mitigating SQL Injection attack
            // if (!demo.IsLoginValid("' or '1' = '1' ; -- ", "1234"))
            // demo valid user name and password
            /***//*
            if (!demo.IsLoginValid("user1", "1234"))
            {
                Console.WriteLine("login failed");
                return;
            }
            **//*
            var employees = dal.GetEmployeesFromReader();

            Console.WriteLine("All the employees using the connected model:");
            PrintEmployees(employees);

            employees = dal.GetEmployeesFromDataSet();
            Console.WriteLine("All the employees using the disconnected model:");
            PrintEmployees(employees);

            PrintEmployeeCount(dal.GetEmployeeCount());

            employees = dal.GetEmployeesOfDepartment(5);

            Console.WriteLine("The employees of Department 5");
            PrintEmployees(employees);

            Console.WriteLine("All the employees by gender and salary:");
            employees = dal.GetEmployeesBy('M', 30000);
            PrintEmployees(employees);



        }
        private static void PrintEmployeeCount(int count)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("total number of employees is:" + count);
        }

        private static void PrintEmployees(List<Employee> employees)
        {
            Console.WriteLine("====================================");
            foreach (var employee in employees)
            {
                Console.WriteLine(employee.FirstName + "\t\t" + employee.BirthDate + "\t\t" + employee.DepartmentNumber);
            }
        }

    }
}
*/