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
using HealthcareSystem.Page;

namespace HealthcareSystem
{
    public sealed partial class MainPage : BasePage
    {
        public MainPage()
        {
            this.InitializeComponent();
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
            SessionManager.Instance.Username = null;
            SessionManager.Instance.FirstName = null;
            SessionManager.Instance.LastName = null;

            Frame.Navigate(typeof(LoginPage));
        }

        private void CreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AppointmentPage));
        }
    }
}