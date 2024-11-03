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
        private readonly TestTypeDAL _testTypeDAL = new TestTypeDAL();

        public RoutineCheckupPage()
        {
            this.InitializeComponent();
            LoadAppointments();
            LoadTestTypes();
        }

        private void LoadAppointments()
        {
            var appointments = _appointmentDAL.GetAllAppointments();
            AppointmentComboBox.ItemsSource = appointments;
        }

        private void LoadTestTypes()
        {
            var testTypes = _testTypeDAL.GetAllTestTypes();
            LabTestTypeComboBox.ItemsSource = testTypes;
        }

        private int GenerateRandomLabTestId()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
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

                if (LabTestTypeComboBox.SelectedItem == null)
                {
                    ErrorTextBlock.Text = "Please select a lab test type.";
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                string selectedAppointment = AppointmentComboBox.SelectedItem as string;
                int appointmentId = int.Parse(selectedAppointment.Split(':')[0]);

                if (_visitDAL.CheckIfRoutineCheckupExists(appointmentId))
                {
                    ErrorTextBlock.Text = "A routine checkup has already been completed for this appointment.";
                    ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                string bloodPressureReading = $"{int.Parse(SystolicTextBox.Text)}/{int.Parse(DiastolicTextBox.Text)}";
                decimal bodyTemp = decimal.Parse(BodyTempTextBox.Text);
                decimal weight = decimal.Parse(WeightTextBox.Text);
                decimal height = decimal.Parse(HeightTextBox.Text);
                int pulse = int.Parse(PulseTextBox.Text);
                string symptoms = SymptomsTextBox.Text;
                string initialDiagnosis = InitialDiagnosisTextBox.Text;
                string finalDiagnosis = FinalDiagnosisTextBox.Text;

                int labTestId = GenerateRandomLabTestId();
                string selectedTestType = LabTestTypeComboBox.SelectedItem as string;
                string testCode = selectedTestType.Split(':')[0];
                string testTypeName = selectedTestType.Split(':')[1].Trim();

                bool success = _visitDAL.CompleteRoutineCheckup(
                    appointmentId, bloodPressureReading, bodyTemp, weight, height,
                    pulse, symptoms, initialDiagnosis, finalDiagnosis, labTestId, testCode, testTypeName);

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
