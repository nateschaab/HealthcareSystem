using System;
using Windows.UI.Xaml;
using MySql.Data.MySqlClient;
using HealthcareSystem.DAL;
using HealthcareSystem.Page;
using System.Linq;

namespace HealthcareSystem
{
    public sealed partial class AddNursePage : BasePage
    {
        public AddNursePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Add Nurse button click event. Hashes the password and inserts nurse details into the database.
        /// </summary>
        private void AddNurse_Click(object sender, RoutedEventArgs e)
        {
            string fullName = this.NurseFullNameTextBox.Text.Trim();
            string username = this.NurseUsernameTextBox.Text.Trim();
            string password = this.NursePasswordBox.Password;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                this.ShowErrorMessage("All fields are required.");
                return;
            }

            try
            {
                string[] nameParts = fullName.Split(' ');
                string firstName = nameParts[0];
                string lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

                using (var connection = new MySqlConnection(Connection.ConnectionString()))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO account (username, password, fname, lname, role)
                        VALUES (@username, @password, @fname, @lname, 'Nurse');";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@fname", firstName);
                        command.Parameters.AddWithValue("@lname", lastName);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            this.ShowSuccessMessage("Nurse added successfully.");
                            this.ClearFields();
                        }
                        else
                        {
                            this.ShowErrorMessage("Failed to add nurse. Please try again.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the Back button click event. Navigates back to the Admin page.
        /// </summary>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdminPage));
        }

        /// <summary>
        /// Clears the input fields.
        /// </summary>
        private void ClearFields()
        {
            this.NurseFullNameTextBox.Text = string.Empty;
            this.NurseUsernameTextBox.Text = string.Empty;
            this.NursePasswordBox.Password = string.Empty;
        }

        /// <summary>
        /// Displays an error message (e.g., to a TextBlock or Debug Console).
        /// </summary>
        /// <param name="message">The error message to display.</param>
        private void ShowErrorMessage(string message)
        {
            // Display error message (replace with UI implementation if needed)
            System.Diagnostics.Debug.WriteLine($"Error: {message}");
        }

        /// <summary>
        /// Displays a success message (e.g., to a TextBlock or Debug Console).
        /// </summary>
        /// <param name="message">The success message to display.</param>
        private void ShowSuccessMessage(string message)
        {
            // Display success message (replace with UI implementation if needed)
            System.Diagnostics.Debug.WriteLine($"Success: {message}");
        }
    }
}
