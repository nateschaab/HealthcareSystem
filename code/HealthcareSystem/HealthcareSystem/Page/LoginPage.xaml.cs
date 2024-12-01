using DBAccess.DAL;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using HealthcareSystem.Page;

namespace HealthcareSystem
{
    public sealed partial class LoginPage : BasePage
    {
        private readonly LoginDAL _loginDAL;

        public LoginPage()
        {
            this.InitializeComponent();
            this._loginDAL = new LoginDAL();
        }

        private void InputField_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                LoginButton_Click(null, null);
            }
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = this.UsernameTextBox.Text;
            string password = this.PasswordBox.Password;

            var (isValid, firstName, lastName) = this._loginDAL.ValidateLoginAndGetName(username, password);

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
                this.LoginError.Text = "Invalid username or password. Please try again.";
            }
        }

    }
}
