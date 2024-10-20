using DBAccess.DAL;
using DBAccess.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HealthcareSystem
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            UserInfo.Text = "Logged in as: Nurse Jane Doe (jane.doe)";

            Debug.WriteLine("Initializing Database Connection");
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

            /*if (!demo.IsLoginValid("user1", "1234"))
            {
                Console.WriteLine("login failed");
                return;
            }*/
            Debug.WriteLine("Getting Mailing Addresses");
            var employees = dal.GetMailingAddressesFromReader();

            Debug.WriteLine("All the employees using the connected model:");
            PrintEmployees(employees);
            }

        private static void PrintEmployees(List<MailingAddress> employees)
        {
            Debug.WriteLine("====================================");
            foreach (var employee in employees)
            {
                Debug.WriteLine(employee.State + "\t\t" + employee.City + "\t\t" + employee.Zip);
            }
        }

        private void ManagePatients_Click(object sender, RoutedEventArgs e)
        {
        Frame.Navigate(typeof(PatientManagementPage));
        }

        private void AdminFunctions_Click(object sender, RoutedEventArgs e)
        {
        Frame.Navigate(typeof(AdminPage));
        }
    }
}