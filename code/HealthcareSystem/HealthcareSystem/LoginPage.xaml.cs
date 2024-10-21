using DBAccess.DAL;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HealthcareSystem
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (IsLoginValid(username, password))
            {
                Debug.WriteLine("Login successful. Navigating to MainPage.");
                Frame.Navigate(typeof(MainPage), username);
            }
            else
            {
                LoginError.Text = "Invalid username or password. Please try again.";
            }
        }

        public bool IsLoginValid(string username, string password)
        {
            using var connection = new MySqlConnection(Connection.ConnectionString());
            connection.Open();

            var goodQuery = "select count(*) from account where username = @username and password =@password;";

            using var command = new MySqlCommand(goodQuery, connection);

            command.Parameters.Add("@username", (DbType)MySqlDbType.VarChar);
            command.Parameters["@username"].Value = username;

            command.Parameters.Add("@password", (DbType)MySqlDbType.VarChar);
            command.Parameters["@password"].Value = password;

            var count = Convert.ToInt32(command.ExecuteScalar());

            return count == 1;
        }
    }
}
