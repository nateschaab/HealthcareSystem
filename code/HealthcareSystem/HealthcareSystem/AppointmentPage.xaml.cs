using DBAccess.DAL;
using HealthcareSystem.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HealthcareSystem
{
    public sealed partial class AppointmentPage : Page
    {
        private Patient patient;
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
                string patientInfo = $"{patient.PatientId}: {patient.FirstName} {patient.LastName}";
                patientInfoList.Add(patientInfo);
            }
            PatientComboBox.ItemsSource = patientInfoList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Patient selectedPatient)
            {
                this.patient = selectedPatient;
                this.PopulatePatientFields(patient);
            }
        }

        private void PopulatePatientFields(Patient patient)
        {
            this.DoctorComboBox.SelectedIndex = 0;
            this.PatientComboBox.SelectedIndex = 0;
            //this.AppointmentDatePicker.Date = patient.DateOfBirth;
            this.ReasonTextBox.Text = string.Empty;
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
