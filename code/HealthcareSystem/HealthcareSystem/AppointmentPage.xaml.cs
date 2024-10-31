using DBAccess.DAL;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HealthcareSystem
{
    public sealed partial class AppointmentPage : Page
    {
        private readonly AppointmentDAL _appointmentDAL;

        public AppointmentPage()
        {
            this.InitializeComponent();
            _appointmentDAL = new AppointmentDAL();
        }

        private void CreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;

            // Parse inputs
            if (!int.TryParse(DoctorIdTextBox.Text, out int doctorId) ||
                !int.TryParse(PatientIdTextBox.Text, out int patientId))
            {
                ErrorTextBlock.Text = "Invalid Doctor or Patient ID.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            DateTime date = AppointmentDatePicker.Date.DateTime;
            TimeSpan time = AppointmentTimePicker.Time;
            DateTime dateTime = date + time;

            string reason = ReasonTextBox.Text;

            // Call the DAL to create the appointment
            bool isCreated = _appointmentDAL.CreateAppointment(doctorId, patientId, dateTime, reason);

            if (isCreated)
            {
                ErrorTextBlock.Text = "Appointment successfully created!";
                ErrorTextBlock.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorTextBlock.Text = "Failed to create appointment. Doctor may already have an appointment at this time.";
                ErrorTextBlock.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
