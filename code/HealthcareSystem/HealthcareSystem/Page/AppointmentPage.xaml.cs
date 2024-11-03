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
                    CreateErrorTextBlock.Text = "Please select both a doctor and a patient.";
                    CreateErrorTextBlock.Visibility = Visibility.Visible;
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
                    CreateErrorTextBlock.Text = "Appointment created successfully!";
                    CreateErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                }
                else
                {
                    CreateErrorTextBlock.Text = "Failed to create appointment. Check for double booking or try again.";
                    CreateErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }
                CreateErrorTextBlock.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                CreateErrorTextBlock.Text = $"Error: {ex.Message}";
                CreateErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                CreateErrorTextBlock.Visibility = Visibility.Visible;
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
                    EditErrorTextBlock.Text = "Please select both a doctor and a patient.";
                    EditErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                int doctorId = int.Parse(selectedDoctor.Split(':')[0]);
                int patientId = int.Parse(selectedPatient.Split(':')[0]);

                DateTime date = AppointmentDatePicker.Date.Date;
                TimeSpan time = AppointmentTimePicker.Time;
                DateTime newAppointmentDateTime = date + time;

                if (newAppointmentDateTime < DateTime.Now)
                {
                    EditErrorTextBlock.Text = "Cannot edit an appointment for a past date and time.";
                    EditErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    EditErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                string reason = ReasonTextBox.Text;
                int appointmentId = App.AppointmentId;

                bool isDateTimeChanged = newAppointmentDateTime != App.Date;

                bool success = _appointmentDAL.EditAppointment(appointmentId, doctorId, patientId, newAppointmentDateTime, reason, isDateTimeChanged);

                if (success)
                {
                    EditErrorTextBlock.Text = "Appointment updated successfully!";
                    EditErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                }
                else
                {
                    EditErrorTextBlock.Text = "Failed to update appointment. Check for double booking or try again.";
                    EditErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }
                EditErrorTextBlock.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                EditErrorTextBlock.Text = $"Error: {ex.Message}";
                EditErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                EditErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
