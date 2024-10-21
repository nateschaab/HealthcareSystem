using DBAccess.DAL;
using DBAccess.Model;
using HealthcareSystem.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HealthcareSystem
{
    public sealed partial class MainPage : Page
    {
        private string LoggedInUsername { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                LoggedInUsername = e.Parameter.ToString();
                LoadUserInfo(LoggedInUsername);
            }
        }

        private void LoadUserInfo(string username)
        {
            try
            {
                Debug.WriteLine($"Received username: {username}");

                UserInfo.Text = $"Logged in as: {username}";

                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();
                Debug.WriteLine("Database connection opened successfully.");

                string query = "SELECT username FROM account WHERE username = @username";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string retrievedUsername = reader["username"].ToString();
                    Debug.WriteLine($"Database returned username: {retrievedUsername}");

                    UserInfo.Text = $"Logged in as: {retrievedUsername}";
                }
                else
                {
                    Debug.WriteLine("No rows returned from the database.");
                    UserInfo.Text = "User not found in the database.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                UserInfo.Text = "An error occurred while loading user info.";
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