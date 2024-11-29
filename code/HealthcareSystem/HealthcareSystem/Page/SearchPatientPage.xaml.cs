using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DBAccess.DAL;
using HealthcareSystem.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
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

                var parameter = new Dictionary<string, object>
                {
                    { "Appointments", app },
                    { "Patient", this.patient }
                };
                Frame.Navigate(typeof(AppointmentPage), parameter);
            }
            else
            {
                Debug.WriteLine("No patient selected.");
            }
        }

        private void PatientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PatientListView.SelectedItem is Patient selectedPatient)
            {
                this.patient = selectedPatient;
            }
        }

        private void SearchPatientsButton_Click(object sender, RoutedEventArgs e)
        {
            var Dal = new PatientDal();

            var patients = Dal.SearchPatient(this.PatientFirstNameTextBox.Text, this.PatientLastNameTextBox.Text,
                this.DOBDatePicker.Date.DateTime);

            this.PatientListView.ItemsSource = patients;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            this.PatientFirstNameTextBox.Text = string.Empty;
            this.PatientLastNameTextBox.Text = string.Empty;
            this.DOBDatePicker.SelectedDate = null;
        }

        private void EditCheckup_Click(object sender, RoutedEventArgs e)
        {
            if (this.patient != null)
            {
                var dal = new AppointmentDAL();
                var app = dal.GetPatientAppointments(this.patient);

                Frame.Navigate(typeof(RoutineCheckupPage), app);
            }
            else
            {
                Debug.WriteLine("No patient selected.");
            }
        }
    }
}