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
        private readonly LoginDAL _loginDAL;

        public LoginPage()
        {
            this.InitializeComponent();
            _loginDAL = new LoginDAL();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var (isValid, firstName, lastName) = _loginDAL.ValidateLoginAndGetName(username, password);

            if (isValid)
            {
                Debug.WriteLine("Login successful. Navigating to MainPage.");

                SessionManager.Instance.Username = username;
                SessionManager.Instance.FirstName = firstName;
                SessionManager.Instance.LastName = lastName;

                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                LoginError.Text = "Invalid username or password. Please try again.";
            }
        }

    }
}
