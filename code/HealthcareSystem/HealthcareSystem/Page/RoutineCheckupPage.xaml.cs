using DBAccess.DAL;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HealthcareSystem.Model;
using System.Linq;
using Windows.UI.Xaml.Documents;

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
            else if (e.Parameter is Appointment appSingle)
            {
                this.Appointments = new List<Appointment> { appSingle };
                this.AppointmentComboBox.ItemsSource = new List<Appointment> { appSingle };
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
                        this.ClearErrorMessages();
                        this.PopulateCheckupFields(checkup);
                    }
                }
            }
        }

        private void ClearErrorMessages()
        {
            this.ErrorTextBlock.Visibility = Visibility.Collapsed;
            this.AppointmentErrorTextBlock.Visibility = Visibility.Collapsed;
            this.SystolicErrorTextBlock.Visibility = Visibility.Collapsed;
            this.DiastolicErrorTextBlock.Visibility = Visibility.Collapsed;
            this.BodyTempErrorTextBlock.Visibility = Visibility.Collapsed;
            this.WeightErrorTextBlock.Visibility = Visibility.Collapsed;
            this.HeightErrorTextBlock.Visibility = Visibility.Collapsed;
            this.PulseErrorTextBlock.Visibility = Visibility.Collapsed;
            this.SymptomsErrorTextBlock.Visibility = Visibility.Collapsed;
            this.InitialDiagnosisErrorTextBlock.Visibility = Visibility.Collapsed;
            this.FinalDiagnosisErrorTextBlock.Visibility = Visibility.Collapsed;
            this.LabTestTypeErrorComboBox.Visibility = Visibility.Collapsed;
        }

        private void PopulateCheckupFields(RoutineCheckup checkup)
        {
            bool hasFinalDiagnosis = !string.IsNullOrWhiteSpace(checkup.FinalDiagnosis);

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

            if (checkup.TestTypeName != null)
            {
                this.LabTestTypeComboBox.SelectedItem = this.LabTestTypeComboBox.Items
                    .OfType<string>()
                    .FirstOrDefault(testType => testType.Contains(checkup.TestTypeName));
            }
            else
            {
                this.LabTestTypeComboBox.SelectedItem = null;
            }

            this.LabTestTypeComboBox.IsEnabled = !hasFinalDiagnosis;
            this.CheckupButton.IsEnabled = !hasFinalDiagnosis;
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

        private async void CompleteCheckupButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasFinalDiagnosis = !string.IsNullOrWhiteSpace(this.FinalDiagnosisTextBox.Text);

            if (hasFinalDiagnosis)
            {
                ConfirmationDialog.Visibility = Visibility.Visible;
                var result = await ConfirmationDialog.ShowAsync();

                if (result != ContentDialogResult.Primary)
                {
                    return;
                }
            }

            try
            {
                if (!ValidateCheckupFields())
                {
                    return;
                }

                string selectedAppointment = AppointmentComboBox.SelectedItem as string;
                int appointmentId = (AppointmentComboBox.SelectedItem as Appointment).AppointmentId;

/*                if (_visitDAL.CheckIfRoutineCheckupExists(appointmentId))
                {
                    ErrorTextBlock.Text = "A routine checkup has already been completed for this appointment.";
                    ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    return;
                }*/

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

        private bool ValidateCheckupFields()
        {
            bool isValid = true;

            if (AppointmentComboBox.SelectedItem == null)
            {
                AppointmentErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                AppointmentErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(SystolicTextBox.Text))
            {
                SystolicErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                SystolicErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(DiastolicTextBox.Text))
            {
                DiastolicErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                DiastolicErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(BodyTempTextBox.Text))
            {
                BodyTempErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                BodyTempErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(WeightTextBox.Text))
            {
                WeightErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                WeightErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(HeightTextBox.Text))
            {
                HeightErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                HeightErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(PulseTextBox.Text))
            {
                PulseErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                PulseErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(SymptomsTextBox.Text))
            {
                SymptomsErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                SymptomsErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (LabTestTypeComboBox.SelectedItem == null)
            {
                LabTestTypeErrorComboBox.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                LabTestTypeErrorComboBox.Visibility = Visibility.Collapsed;
            }

            if (!isValid)
            {
                ErrorTextBlock.Text = "Please fill out all required fields.";
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
            return isValid;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
