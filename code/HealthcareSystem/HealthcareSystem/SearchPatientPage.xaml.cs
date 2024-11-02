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
                // Handle the case where no patient is selected, if needed
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
    }
}
