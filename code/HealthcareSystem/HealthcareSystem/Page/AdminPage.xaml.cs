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
    public sealed partial class AdminPage : BasePage
    {
        public AdminPage()
        {
            this.InitializeComponent();
        }

        private void AddNurse_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddNursePage));
        }

        private void QueryInterface_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(QueryInterfacePage));
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ReportPage));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Instance.Username = null;
            SessionManager.Instance.FirstName = null;
            SessionManager.Instance.LastName = null;

            Frame.Navigate(typeof(LoginPage));
        }
    }
}