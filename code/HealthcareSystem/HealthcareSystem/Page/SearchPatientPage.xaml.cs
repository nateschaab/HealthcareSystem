using DBAccess.DAL;
using HealthcareSystem.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Windows.Networking;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPatientPage : Page
    {
        private Patient patient;

        public SearchPatientPage()
        {
            this.InitializeComponent();
        }

        private void EditPatients_Click(object sender, RoutedEventArgs e)
        {
            if (this.patient != null)
            {
                Frame.Navigate(typeof(PatientManagementPage), this.patient);
            }
            else
            {
                Debug.WriteLine("No patient selected.");
            }
        }

        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (this.patient != null)
            {
                var dal = new AppointmentDAL();
                var app = dal.GetPatientAppointments(this.patient);

                Frame.Navigate(typeof(AppointmentPage), app);
            }
            else
            {
                Debug.WriteLine("No patient selected.");
            }
        }

        private void PatientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientListView.SelectedItem is Patient selectedPatient)
            {
                this.patient = selectedPatient;
            }
        }

        private void SearchPatientsButton_Click(object sender, RoutedEventArgs e)
        {
            var Dal = new PatientDal();

            var patients = Dal.SearchPatient(
                PatientFirstNameTextBox.Text,
                PatientLastNameTextBox.Text,
                DOBDatePicker.Date.DateTime);

            PatientListView.ItemsSource = patients;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
