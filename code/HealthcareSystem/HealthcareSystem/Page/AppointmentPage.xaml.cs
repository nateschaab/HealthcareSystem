using DBAccess.DAL;
using HealthcareSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HealthcareSystem
{
    public sealed partial class AppointmentPage : Page
    {
        private Appointment App { get; set; }
        private List<Appointment> Appointments { get; set; }
        private readonly DoctorDAL _doctorDAL = new DoctorDAL();
        private readonly PatientDal _patientDAL = new PatientDal();
        private readonly AppointmentDAL _appointmentDAL = new AppointmentDAL();

        public AppointmentPage()
        {
            this.InitializeComponent();
            LoadDoctorsAndPatients();
        }

        private void LoadDoctorsAndPatients()
        {
            var doctors = _doctorDAL.GetAllDoctors();
            DoctorComboBox.ItemsSource = doctors;

            var patients = _patientDAL.GetPatientsFromReader();
            var patientInfoList = new List<string>();
            foreach (var patient in patients)
            {
                string patientInfo = $"{patient.PatientId}";
                patientInfoList.Add(patientInfo);
            }
            PatientComboBox.ItemsSource = patientInfoList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is List<Appointment> app)
            {
                this.Appointments = app;
                this.PatientListView.ItemsSource = app;
            }
        }

        private void PatientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientListView.SelectedItem is Appointment app)
            {
                PopulateAppFields(app);
                this.App = app;
            }
        }

        private void PopulateAppFields(Appointment app)
        {
            var doctorInfo = $"{app.DoctorId}";
            DoctorComboBox.SelectedItem = doctorInfo;

            var patientId = $"{app.PatientId}";
            PatientComboBox.SelectedItem = patientId;

            this.AppointmentDatePicker.Date = app.Date;

            this.AppointmentTimePicker.Time = app.Date.TimeOfDay;

            this.ReasonTextBox.Text = app.Reason;
        }

        private void CreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedDoctor = DoctorComboBox.SelectedItem as string;
                string selectedPatient = PatientComboBox.SelectedItem as string;
                if (selectedDoctor == null || selectedPatient == null)
                {
                    ErrorTextBlock.Text = "Please select both a doctor and a patient.";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                int doctorId = int.Parse(selectedDoctor.Split(':')[0]);
                int patientId = int.Parse(selectedPatient.Split(':')[0]);

                DateTime date = AppointmentDatePicker.Date.Date;
                TimeSpan time = AppointmentTimePicker.Time;
                DateTime appointmentDateTime = date + time;

                string reason = ReasonTextBox.Text;

                bool success = _appointmentDAL.CreateAppointment(doctorId, patientId, appointmentDateTime, reason);

                if (success)
                {
                    ErrorTextBlock.Text = "Appointment created successfully!";
                    ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                }
                else
                {
                    ErrorTextBlock.Text = "Failed to create appointment. Check for double booking or try again.";
                    ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"Error: {ex.Message}";
                ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedDoctor = DoctorComboBox.SelectedItem as string;
                string selectedPatient = PatientComboBox.SelectedItem as string;
                if (selectedDoctor == null || selectedPatient == null)
                {
                    ErrorTextBlock.Text = "Please select both a doctor and a patient.";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                // Extract IDs from selected values
                int doctorId = int.Parse(selectedDoctor.Split(':')[0]);
                int patientId = int.Parse(selectedPatient.Split(':')[0]);

                // Retrieve date and time from DatePicker and TimePicker
                DateTime date = AppointmentDatePicker.Date.Date;
                TimeSpan time = AppointmentTimePicker.Time;
                DateTime appointmentDateTime = date + time;

                // Check if the appointment date and time have already passed
                if (appointmentDateTime < DateTime.Now)
                {
                    ErrorTextBlock.Text = "Cannot edit an appointment for a past date and time.";
                    ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                // Get the reason and appointment ID
                string reason = ReasonTextBox.Text;
                int appointmentId = this.App.AppointmentId;

                // Call the EditAppointment method
                bool success = _appointmentDAL.EditAppointment(appointmentId, doctorId, patientId, appointmentDateTime, reason);

                // Show success or failure message
                if (success)
                {
                    ErrorTextBlock.Text = "Appointment updated successfully!";
                    ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                }
                else
                {
                    ErrorTextBlock.Text = "Failed to update appointment. Check for double booking or try again.";
                    ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"Error: {ex.Message}";
                ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
