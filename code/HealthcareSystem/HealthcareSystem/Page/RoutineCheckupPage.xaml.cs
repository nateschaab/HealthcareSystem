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

        private async void PopulateCheckupFields(RoutineCheckup checkup)
        {
            bool hasFinalDiagnosis = !string.IsNullOrWhiteSpace(checkup.FinalDiagnosis);

            if (hasFinalDiagnosis)
            {
                ConfirmationDialog.Visibility = Visibility.Visible;
                var result = await ConfirmationDialog.ShowAsync();

                if (result != ContentDialogResult.Primary)
                {
                    return;
                }
            }

            if (checkup.BloodPressureReading != null)
            {
                this.SystolicTextBox.Text = checkup.Systolic.ToString();
                this.SystolicTextBox.IsReadOnly = hasFinalDiagnosis;
                this.SystolicTextBox.IsHitTestVisible = !hasFinalDiagnosis;

                this.DiastolicTextBox.Text = checkup.Diastolic.ToString();
                this.DiastolicTextBox.IsReadOnly = hasFinalDiagnosis;
                this.DiastolicTextBox.IsHitTestVisible = !hasFinalDiagnosis;
            }
            else
            {
                this.SystolicTextBox.Text = string.Empty;
                this.SystolicTextBox.IsReadOnly = false;
                this.SystolicTextBox.IsHitTestVisible = !hasFinalDiagnosis;

                this.DiastolicTextBox.Text = string.Empty;
                this.DiastolicTextBox.IsReadOnly = false;
                this.DiastolicTextBox.IsHitTestVisible = !hasFinalDiagnosis;
            }

            this.BodyTempTextBox.Text = checkup.BodyTemp?.ToString() ?? string.Empty;
            this.BodyTempTextBox.IsReadOnly = hasFinalDiagnosis;
            this.BodyTempTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.WeightTextBox.Text = checkup.Weight?.ToString() ?? string.Empty;
            this.WeightTextBox.IsReadOnly = hasFinalDiagnosis;
            this.WeightTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.HeightTextBox.Text = checkup.Height?.ToString() ?? string.Empty;
            this.HeightTextBox.IsReadOnly = hasFinalDiagnosis;
            this.HeightTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.PulseTextBox.Text = checkup.Pulse?.ToString() ?? string.Empty;
            this.PulseTextBox.IsReadOnly = hasFinalDiagnosis;
            this.PulseTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.SymptomsTextBox.Text = checkup.Symptoms ?? string.Empty;
            this.SymptomsTextBox.IsReadOnly = hasFinalDiagnosis;
            this.SymptomsTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.InitialDiagnosisTextBox.Text = checkup.InitialDiagnosis ?? string.Empty;
            this.InitialDiagnosisTextBox.IsReadOnly = hasFinalDiagnosis;
            this.InitialDiagnosisTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.FinalDiagnosisTextBox.Text = checkup.FinalDiagnosis ?? string.Empty;
            this.FinalDiagnosisTextBox.IsReadOnly = hasFinalDiagnosis;
            this.FinalDiagnosisTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            // Handle checkboxes for test types
            this.LowDensityLipoproteinsCheckBox.IsChecked = checkup.TestTypeName?.Contains("Low Density Lipoproteins") ?? false;
            this.HepatitisACheckBox.IsChecked = checkup.TestTypeName?.Contains("Hepatitis A") ?? false;
            this.HepatitisBCheckBox.IsChecked = checkup.TestTypeName?.Contains("Hepatitis B") ?? false;
            this.WhiteBloodCellCheckBox.IsChecked = checkup.TestTypeName?.Contains("White Blood Cell") ?? false;

            this.LowDensityLipoproteinsCheckBox.IsEnabled = !hasFinalDiagnosis;
            this.HepatitisACheckBox.IsEnabled = !hasFinalDiagnosis;
            this.HepatitisBCheckBox.IsEnabled = !hasFinalDiagnosis;
            this.WhiteBloodCellCheckBox.IsEnabled = !hasFinalDiagnosis;

            this.CheckupButton.IsEnabled = !hasFinalDiagnosis;
        }


        private bool AreFieldsEditable()
        {
            return !this.SystolicTextBox.IsReadOnly &&
                   !this.DiastolicTextBox.IsReadOnly &&
                   !this.BodyTempTextBox.IsReadOnly &&
                   !this.WeightTextBox.IsReadOnly &&
                   !this.HeightTextBox.IsReadOnly &&
                   !this.PulseTextBox.IsReadOnly &&
                   !this.SymptomsTextBox.IsReadOnly &&
                   !this.InitialDiagnosisTextBox.IsReadOnly &&
                   (this.LowDensityLipoproteinsCheckBox.IsEnabled ||
                    this.HepatitisACheckBox.IsEnabled ||
                    this.HepatitisBCheckBox.IsEnabled ||
                    this.WhiteBloodCellCheckBox.IsEnabled);
        }




        private void LoadTestTypes()
        {
            var testTypes = _testTypeDAL.GetAllTestTypes();

            // Example for dynamic checkbox creation (if needed in the future)
            this.LowDensityLipoproteinsCheckBox.Content = testTypes.FirstOrDefault(t => t.Contains("Low Density Lipoproteins")) ?? "Low Density Lipoproteins";
            this.HepatitisACheckBox.Content = testTypes.FirstOrDefault(t => t.Contains("Hepatitis A")) ?? "Hepatitis A";
            this.HepatitisBCheckBox.Content = testTypes.FirstOrDefault(t => t.Contains("Hepatitis B")) ?? "Hepatitis B";
            this.WhiteBloodCellCheckBox.Content = testTypes.FirstOrDefault(t => t.Contains("White Blood Cell")) ?? "White Blood Cell";
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

                string selectedAppointment = AppointmentComboBox.SelectedItem as string;
                int appointmentId = (AppointmentComboBox.SelectedItem as Appointment).AppointmentId;

                // Check if routine checkup is already completed
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

                // Retrieve selected test types
                var selectedTestTypes = new List<string>();
                if (LowDensityLipoproteinsCheckBox.IsChecked == true)
                    selectedTestTypes.Add("Low Density Lipoproteins");
                if (HepatitisACheckBox.IsChecked == true)
                    selectedTestTypes.Add("Hepatitis A");
                if (HepatitisBCheckBox.IsChecked == true)
                    selectedTestTypes.Add("Hepatitis B");
                if (WhiteBloodCellCheckBox.IsChecked == true)
                    selectedTestTypes.Add("White Blood Cell");

                // Complete the routine checkup with multiple lab tests
                bool success = _visitDAL.CompleteRoutineCheckupWithTests(
                    appointmentId, bloodPressureReading, bodyTemp, weight, height,
                    pulse, symptoms, initialDiagnosis, finalDiagnosis, selectedTestTypes);

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
