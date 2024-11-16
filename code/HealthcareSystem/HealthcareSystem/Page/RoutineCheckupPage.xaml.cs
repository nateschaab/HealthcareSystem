using DBAccess.DAL;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HealthcareSystem.Model;
using System.Linq;

namespace HealthcareSystem
{
    public sealed partial class RoutineCheckupPage : Page
    {
        private readonly AppointmentDAL _appointmentDAL = new AppointmentDAL();
        private readonly VisitDAL _visitDAL = new VisitDAL();
        private readonly TestTypeDAL _testTypeDAL = new TestTypeDAL();

        private List<Appointment> Appointments { get; set; }

        public RoutineCheckupPage()
        {
            this.InitializeComponent();
            LoadTestTypes();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is List<Appointment> app)
            {
                this.Appointments = app;
                this.AppointmentComboBox.ItemsSource = app;
            }
            else
            {
                ErrorTextBlock.Text = "No appointments found.";
                ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.AppointmentComboBox.SelectedItem is Appointment app)
            {
                var dal = new VisitDAL();
                var checkups = dal.GetRoutineCheckups();
                foreach (var checkup in checkups) {
                    if (checkup.AppointmentId == app.AppointmentId)
                    {
                        this.PopulateCheckupFields(checkup);
                    }
                }
            }
        }

        private void PopulateCheckupFields(RoutineCheckup checkup)
        {
            this.SystolicTextBox.Text = checkup.Systolic.ToString();

            this.DiastolicTextBox.Text = checkup.Dystolic.ToString();

            this.BodyTempTextBox.Text = checkup.BodyTemp.ToString();

            this.WeightTextBox.Text = checkup.Weight.ToString();

            this.HeightTextBox.Text = checkup.Height.ToString();

            this.PulseTextBox.Text = checkup.Pulse.ToString();

            this.SymptomsTextBox.Text = checkup.Symptoms;

            this.InitialDiagnosisTextBox.Text = checkup.InitialDiagnosis;

            this.FinalDiagnosisTextBox.Text = checkup.FinalDiagnosis;

            if (checkup.TestTypeName != null)
            {
                this.LabTestTypeComboBox.SelectedItem = this.LabTestTypeComboBox.Items
                    .OfType<string>()
                    .FirstOrDefault(testType => testType.Contains(checkup.TestTypeName));
            }

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
