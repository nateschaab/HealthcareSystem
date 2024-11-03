using DBAccess.DAL;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HealthcareSystem
{
    public sealed partial class RoutineCheckupPage : Page
    {
        private readonly AppointmentDAL _appointmentDAL = new AppointmentDAL();
        private readonly VisitDAL _visitDAL = new VisitDAL();

        public RoutineCheckupPage()
        {
            this.InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            var appointments = _appointmentDAL.GetAllAppointments();
            AppointmentComboBox.ItemsSource = appointments;
        }

        private void CompleteCheckupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AppointmentComboBox.SelectedItem == null)
                {
                    ErrorTextBlock.Text = "Please select an appointment.";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                string selectedAppointment = AppointmentComboBox.SelectedItem as string;
                int appointmentId = int.Parse(selectedAppointment.Split(':')[0]);

                int systolic = int.Parse(SystolicTextBox.Text);
                int diastolic = int.Parse(DiastolicTextBox.Text);
                string bloodPressureReading = $"{systolic}/{diastolic}";

                decimal bodyTemp = decimal.Parse(BodyTempTextBox.Text);
                decimal weight = decimal.Parse(WeightTextBox.Text);
                decimal height = decimal.Parse(HeightTextBox.Text);
                int pulse = int.Parse(PulseTextBox.Text);
                string symptoms = SymptomsTextBox.Text;
                string initialDiagnosis = InitialDiagnosisTextBox.Text;
                string finalDiagnosis = FinalDiagnosisTextBox.Text;
                int labTestId = int.Parse(LabTestIdTextBox.Text);

                bool success = _visitDAL.CompleteRoutineCheckup(
                    appointmentId, bloodPressureReading, bodyTemp, weight, height,
                    pulse, symptoms, initialDiagnosis, finalDiagnosis, labTestId);

                if (success)
                {
                    ErrorTextBlock.Text = "Routine checkup completed successfully!";
                    ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                }
                else
                {
                    ErrorTextBlock.Text = "Failed to complete routine checkup.";
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