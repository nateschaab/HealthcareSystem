using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DBAccess.DAL;
using HealthcareSystem.DAL;
using HealthcareSystem.Model;
using HealthcareSystem.Page;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    ///     Represents the page for searching and managing patients.
    /// </summary>
    public sealed partial class SearchPatientPage : BasePage
    {
        #region Data members

        /// <summary>
        ///     The currently selected patient.
        /// </summary>
        private Patient patient;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchPatientPage" /> class.
        /// </summary>
        public SearchPatientPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handles the Edit Patients button click event.
        ///     Navigates to the patient management page if a patient is selected.
        /// </summary>
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

        /// <summary>
        ///     Handles the Edit Appointment button click event.
        ///     Navigates to the appointment page with the selected patient's appointments.
        /// </summary>
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

        /// <summary>
        ///     Handles the Patient ListView selection changed event.
        ///     Updates the selected patient when the selection changes.
        /// </summary>
        private void PatientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PatientListView.SelectedItem is Patient selectedPatient)
            {
                this.patient = selectedPatient;
            }
        }

        /// <summary>
        ///     Handles the Search Patients button click event.
        ///     Searches for patients using the specified search criteria and updates the ListView.
        /// </summary>
        private void SearchPatientsButton_Click(object sender, RoutedEventArgs e)
        {
            var Dal = new PatientDal();

            var patients = Dal.SearchPatient(this.PatientFirstNameTextBox.Text, this.PatientLastNameTextBox.Text,
                this.DOBDatePicker.Date.DateTime);

            this.PatientListView.ItemsSource = patients;
        }

        /// <summary>
        ///     Handles the Back button click event.
        ///     Navigates back to the main page.
        /// </summary>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        /// <summary>
        ///     Handles the Clear button click event.
        ///     Clears all the search input fields.
        /// </summary>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            this.PatientFirstNameTextBox.Text = string.Empty;
            this.PatientLastNameTextBox.Text = string.Empty;
            this.DOBDatePicker.SelectedDate = null;
        }

        /// <summary>
        ///     Handles the Edit Checkup button click event.
        ///     Navigates to the routine checkup page with the selected patient's appointments.
        /// </summary>
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

        #endregion
    }
}