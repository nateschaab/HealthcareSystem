using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using DBAccess.DAL;
using HealthcareSystem.DAL;
using HealthcareSystem.Page;

namespace HealthcareSystem
{
    /// <summary>
    ///     Represents the login page, allowing users to authenticate by entering a username and password.
    /// </summary>
    public sealed partial class LoginPage : BasePage
    {
        #region Data members

        private readonly LoginDAL _loginDAL;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginPage" /> class and sets up the login data access layer.
        /// </summary>
        public LoginPage()

        {
            this.InitializeComponent();
            this._loginDAL = new LoginDAL();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handles the event triggered when a key is pressed in the input fields.
        ///     Logs in the user if the Enter key is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data containing information about the key press.</param>
        private void InputField_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.LoginButton_Click(null, null);
            }
        }

        /// <summary>
        ///     Handles the login button click event, authenticating the user based on the entered credentials.
        ///     Navigates to the <see cref="MainPage" /> on successful login or displays an error message on failure.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data containing information about the click event.</param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = this.UsernameTextBox.Text;
            var password = this.PasswordBox.Password;

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

        #endregion
    }
}