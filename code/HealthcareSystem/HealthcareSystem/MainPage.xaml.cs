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
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            string firstName = SessionManager.Instance.FirstName;
            string lastName = SessionManager.Instance.LastName;
            string username = SessionManager.Instance.Username;

            UserInfo.Text = $"Logged in as: {firstName} {lastName} (Username: {username})";
            Debug.WriteLine($"User Info Loaded: {firstName} {lastName} (Username: {username})");
        }

        private void ManagePatients_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PatientManagementPage));
        }

        private void SearchPatients_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPatientPage));
        }

        private void AdminFunctions_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdminPage));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear user information in SessionManager
            SessionManager.Instance.Username = null;
            SessionManager.Instance.FirstName = null;
            SessionManager.Instance.LastName = null;

            // Navigate back to the LoginPage
            Frame.Navigate(typeof(LoginPage));
        }

        private void CreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AppointmentPage));
        }
    }
}