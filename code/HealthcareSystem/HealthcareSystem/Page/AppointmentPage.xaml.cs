using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DBAccess.DAL;
using HealthcareSystem.DAL;
using HealthcareSystem.Model;
using HealthcareSystem.Page;

namespace HealthcareSystem
{
    /// <summary>
    ///     Represents a page for managing appointments, allowing users to view, create, and modify appointments.
    ///     Inherits from <see cref="BasePage" />.
    /// </summary>
    public sealed partial class AppointmentPage : BasePage
    {
        #region Data members

        private readonly DoctorDAL _doctorDAL = new DoctorDAL();
        private readonly PatientDal _patientDAL = new PatientDal();
        private readonly AppointmentDAL _appointmentDAL = new AppointmentDAL();

        #endregion

        #region Properties

        private Appointment App { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppointmentPage" /> class.
        /// </summary>
        public AppointmentPage()
        {
            this.InitializeComponent();
            this.LoadDoctorsAndPatients();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Loads the list of doctors and patients from the database into their respective combo boxes.
        /// </summary>
        private void LoadDoctorsAndPatients()
        {
            var doctors = this._doctorDAL.GetAllDoctors();
            this.DoctorComboBox.ItemsSource = doctors;

            var patients = this._patientDAL.GetPatientsFromReader();

            this.PatientComboBox.ItemsSource = patients;
        }

        /// <summary>
        ///     Handles the navigation event to this page, populating the UI with appointments and patient data.
        /// </summary>
        /// <param name="e">Navigation event arguments containing parameter data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Dictionary<string, object> parameters)
            {
                var apps = parameters["Appointments"] as Appointment;
                var patient = parameters["Patient"] as Patient;

                this.PatientListView.ItemsSource = apps;
                this.PopulatePatientField(patient);

                this.PatientComboBox.IsHitTestVisible = false;
            }
        }

        /// <summary>
        ///     Handles selection changes in the patient list view and populates the fields for the selected appointment.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments containing information about the selection change.</param>
        private void PatientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PatientListView.SelectedItem is Appointment app)
            {
                this.PopulateAppFields(app);
                this.App = app;
            }
        }

        /// <summary>
        ///     Populates the UI fields with details of the specified appointment.
        /// </summary>
        /// <param name="app">The appointment to populate fields with.</param>
        private void PopulateAppFields(Appointment app)
        {
            var doctorId = app.DoctorId;

            this.DoctorComboBox.SelectedItem = this.DoctorComboBox.Items
                .OfType<string>()
                .FirstOrDefault(doctorInfo =>
                {
                    var idPart = doctorInfo.Split(':')[0].Trim();

                    return int.TryParse(idPart, out var parsedId) && parsedId == doctorId;
                });

            var patientId = app.PatientId;

            this.PatientComboBox.SelectedItem = this.PatientComboBox.Items?
                .OfType<Patient>()
                .FirstOrDefault(patient => patient.PatientId == patientId);

            this.AppointmentDatePicker.Date = app.Date;

            this.AppointmentTimePicker.Time = app.Date.TimeOfDay;

            this.ReasonTextBox.Text = app.Reason;
        }

        /// <summary>
        ///     Selects the appropriate patient in the combo box based on the provided patient details.
        /// </summary>
        /// <param name="patient">The patient whose details are used to populate the combo box selection.</param>
        private void PopulatePatientField(Patient patient)
        {
            var patientName = patient.FirstName;

            this.PatientComboBox.SelectedItem = this.PatientComboBox.Items?
                .OfType<Patient>()
                .FirstOrDefault(patient => patient.FirstName == patientName);
        }

        /// <summary>
        ///     Handles the creation of a new appointment when the "Create Appointment" button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void CreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(this.DoctorComboBox.SelectedItem is string selectedDoctor) ||
                    !(this.PatientComboBox.SelectedItem is Patient selectedPatient))
                {
                    this.CreateErrorTextBlock.Text = "Please select both a doctor and a patient.";
                    this.CreateErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                var doctorId = int.Parse(selectedDoctor.Split(':')[0]);
                var patientId = selectedPatient.PatientId;

                var date = this.AppointmentDatePicker.Date.Date;
                var time = this.AppointmentTimePicker.Time;
                var appointmentDateTime = date + time;

                var reason = this.ReasonTextBox.Text;

                var success = this._appointmentDAL.CreateAppointment(doctorId, patientId, appointmentDateTime, reason);

                if (success)
                {
                    this.CreateErrorTextBlock.Text = "Appointment created successfully!";
                    this.CreateErrorTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    this.CreateErrorTextBlock.Text =
                        "Failed to create appointment. Check for double booking or try again.";
                    this.CreateErrorTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                }

                this.CreateErrorTextBlock.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                this.CreateErrorTextBlock.Text = $"Error: {ex.Message}";
                this.CreateErrorTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                this.CreateErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     Handles the editing of an existing appointment when the "Edit Appointment" button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var date = this.AppointmentDatePicker.Date.Date;
                var time = this.AppointmentTimePicker.Time;
                var newAppointmentDateTime = date + time;

                if (newAppointmentDateTime < DateTime.Now)
                {
                    this.EditErrorTextBlock.Text = "Cannot edit an appointment for a past date and time.";
                    this.EditErrorTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    this.EditErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                if (!(this.DoctorComboBox.SelectedItem is string selectedDoctor) ||
                    !(this.PatientComboBox.SelectedItem is Patient selectedPatient))
                {
                    this.EditErrorTextBlock.Text = "Please select both a doctor and a patient.";
                    this.EditErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                var doctorId = int.Parse(selectedDoctor.Split(':')[0]);
                var patientId = selectedPatient.PatientId;

                var reason = this.ReasonTextBox.Text;
                var appointmentId = this.App.AppointmentId;

                var isDateTimeChanged = newAppointmentDateTime != this.App.Date;

                var success = this._appointmentDAL.EditAppointment(appointmentId, doctorId, patientId,
                    newAppointmentDateTime, reason, isDateTimeChanged);

                if (success)
                {
                    this.EditErrorTextBlock.Text = "Appointment updated successfully!";
                    this.EditErrorTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    this.EditErrorTextBlock.Text =
                        "Failed to update appointment. Check for double booking or try again.";
                    this.EditErrorTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                }

                this.EditErrorTextBlock.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                this.EditErrorTextBlock.Text = $"Error: {ex.Message}";
                this.EditErrorTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                this.EditErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     Navigates back to the main page when the "Back" button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        #endregion
    }
}