using DBAccess.DAL;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HealthcareSystem.Model;
using System.Linq;
using HealthcareSystem.Page;

namespace HealthcareSystem
{
    public sealed partial class RoutineCheckupPage : BasePage
    {
        private readonly VisitDAL _visitDAL = new VisitDAL();
        private readonly TestTypeDAL _testTypeDAL = new TestTypeDAL();

        private List<Appointment> Appointments { get; set; }

        public RoutineCheckupPage()
        {
            this.InitializeComponent();
            this.LoadTestTypes();
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
                this.ErrorTextBlock.Text = "No appointments found.";
                this.ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.AppointmentComboBox.SelectedItem is Appointment app)
            {
                try
                {
                    var dal = new VisitDAL();
                    var checkup = dal.GetRoutineCheckupByAppointmentId(app.AppointmentId);

                    if (checkup != null)
                    {
                        checkup.LabTests = dal.GetLabTestsForVisit(checkup.VisitId);
                        this.ClearErrorMessages();
                        this.ClearCheckboxes();
                        this.PopulateCheckupFields(checkup);
                    }
                    else
                    {
                        this.ErrorTextBlock.Text = "No routine checkup found for the selected appointment.";
                        this.ErrorTextBlock.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    this.ErrorTextBlock.Text = $"Error loading routine checkup: {ex.Message}";
                    this.ErrorTextBlock.Visibility = Visibility.Visible;
                }
            }
        }


        private void ClearCheckboxes()
        {
            this.LowDensityLipoproteinsCheckBox.IsChecked = false;
            this.HepatitisACheckBox.IsChecked = false;
            this.HepatitisBCheckBox.IsChecked = false;
            this.WhiteBloodCellCheckBox.IsChecked = false;
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
            this.LDLResultErrorTextBlock.Visibility = Visibility.Collapsed;
            this.WBCResultErrorTextBlock.Visibility = Visibility.Collapsed;
            this.HBResultErrorTextBlock.Visibility = Visibility.Collapsed;
            this.HAResultErrorTextBlock.Visibility = Visibility.Collapsed;
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

            this.LDLResultTextBox.Text = string.Empty;
            this.WBCResultTextBox.Text = string.Empty;
            this.HBResultTextBox.Text = string.Empty;
            this.HAResultTextBox.Text = string.Empty;

            if (checkup.LabTests != null)
            {
                foreach (var labTest in checkup.LabTests)
                {
                    if (labTest.Result != null && labTest.TestTypeName != null && labTest.TestTypeName.Contains("Low Density Lipoproteins"))
                    {
                        this.LDLResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                    else if (labTest.Result != null && labTest.TestTypeName != null && labTest.TestTypeName.Contains("Hepatitis A"))
                    {
                        this.HAResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                    else if (labTest.Result != null && labTest.TestTypeName != null && labTest.TestTypeName.Contains("Hepatitis B"))
                    {
                        this.HBResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                    else if (labTest.Result != null && labTest.TestTypeName != null && labTest.TestTypeName.Contains("White Blood Cell"))
                    {
                        this.WBCResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                }
            }

            this.LDLResultTextBox.IsReadOnly = hasFinalDiagnosis;
            this.LDLResultTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.WBCResultTextBox.IsReadOnly = hasFinalDiagnosis;
            this.WBCResultTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.HBResultTextBox.IsReadOnly = hasFinalDiagnosis;
            this.HBResultTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.HAResultTextBox.IsReadOnly = hasFinalDiagnosis;
            this.HAResultTextBox.IsHitTestVisible = !hasFinalDiagnosis;

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

            foreach(var test in checkup.LabTests)
            {
                if (test.TestTypeName != null &&  test.TestTypeName.Contains("Low Density Lipoproteins"))
                {
                    this.LowDensityLipoproteinsCheckBox.IsChecked = true;
                }
                if (test.TestTypeName != null && test.TestTypeName.Contains("Hepatitis A"))
                {
                    this.HepatitisACheckBox.IsChecked = true;
                }
                if (test.TestTypeName != null && test.TestTypeName.Contains("Hepatitis B"))
                {
                    this.HepatitisBCheckBox.IsChecked = true;
                }
                if (test.TestTypeName != null && test.TestTypeName.Contains("White Blood Cell"))
                {
                    this.WhiteBloodCellCheckBox.IsChecked = true;
                }
            }

            this.LowDensityLipoproteinsCheckBox.IsEnabled = !hasFinalDiagnosis;
            this.HepatitisACheckBox.IsEnabled = !hasFinalDiagnosis;
            this.HepatitisBCheckBox.IsEnabled = !hasFinalDiagnosis;
            this.WhiteBloodCellCheckBox.IsEnabled = !hasFinalDiagnosis;

            this.CheckupButton.IsEnabled = !hasFinalDiagnosis;
        }

        private void LoadTestTypes()
        {
            var testTypes = this._testTypeDAL.GetAllTestTypes();

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

        private async void CompleteCheckupButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasFinalDiagnosis = !string.IsNullOrWhiteSpace(this.FinalDiagnosisTextBox.Text);

            if (hasFinalDiagnosis)
            {
                this.ConfirmationDialog.Visibility = Visibility.Visible;
                var result = await this.ConfirmationDialog.ShowAsync();

                if (result != ContentDialogResult.Primary)
                {
                    return;
                }
            }

            try
            {
                if (!this.ValidateCheckupFields())
                {
                    return;
                }

                int appointmentId = (this.AppointmentComboBox.SelectedItem as Appointment).AppointmentId;

                string bloodPressureReading = $"{int.Parse(this.SystolicTextBox.Text)}/{int.Parse(this.DiastolicTextBox.Text)}";
                decimal bodyTemp = decimal.Parse(this.BodyTempTextBox.Text);
                decimal weight = decimal.Parse(this.WeightTextBox.Text);
                decimal height = decimal.Parse(this.HeightTextBox.Text);
                int pulse = int.Parse(this.PulseTextBox.Text);
                string symptoms = this.SymptomsTextBox.Text;
                string initialDiagnosis = this.InitialDiagnosisTextBox.Text;
                string finalDiagnosis = this.FinalDiagnosisTextBox.Text;

                var selectedTestTypes = new List<string>();
                if (this.LowDensityLipoproteinsCheckBox.IsChecked == true)
                    selectedTestTypes.Add("Low Density Lipoproteins");
                if (this.HepatitisACheckBox.IsChecked == true)
                    selectedTestTypes.Add("Hepatitis A");
                if (this.HepatitisBCheckBox.IsChecked == true)
                    selectedTestTypes.Add("Hepatitis B");
                if (this.WhiteBloodCellCheckBox.IsChecked == true)
                    selectedTestTypes.Add("White Blood Cell");

                var testResults = new Dictionary<string, string>();

                if (this.LowDensityLipoproteinsCheckBox.IsChecked == true)
                {
                    string ldlResult = this.ParseResult(this.LDLResultTextBox.Text);
                    testResults.Add("Low Density Lipoproteins", ldlResult);
                }

                if (this.HepatitisACheckBox.IsChecked == true)
                {
                    string haResult = this.ParseResult(this.HAResultTextBox.Text);
                    testResults.Add("Hepatitis A", haResult);
                }

                if (this.HepatitisBCheckBox.IsChecked == true)
                {
                    string hbResult = this.ParseResult(this.HBResultTextBox.Text);
                    testResults.Add("Hepatitis B", hbResult);
                }

                if (this.WhiteBloodCellCheckBox.IsChecked == true)
                {
                    string wbcResult = this.ParseResult(this.WBCResultTextBox.Text);
                    testResults.Add("White Blood Cell", wbcResult);
                }

                bool success = this._visitDAL.CompleteRoutineCheckupWithTests(
                    appointmentId, bloodPressureReading, bodyTemp, weight, height,
                    pulse, symptoms, initialDiagnosis, finalDiagnosis, selectedTestTypes, testResults);

                if (success)
                {
                    this.ErrorTextBlock.Text = "Routine checkup completed successfully!";
                    this.ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                }
                else
                {
                    this.ErrorTextBlock.Text = "Failed to complete routine checkup.";
                    this.ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                }

                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                this.ErrorTextBlock.Text = $"Error: {ex.Message}";
                this.ErrorTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private string ParseResult(string input)
        {
            int colonIndex = input.IndexOf(':');
            if (colonIndex != -1)
            {
                return input.Substring(0, colonIndex).Trim();
            }
            return input.Trim();
        }

        private bool ValidateCheckupFields()
        {
            bool isValid = true;
            this.ClearErrorMessages();

            if (this.AppointmentComboBox.SelectedItem == null)
            {
                this.AppointmentErrorTextBlock.Text = "Please select an appointment from the dropdown (e.g., 'John Doe - 12/01/2024').";
                this.AppointmentErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.SystolicTextBox.Text))
            {
                this.SystolicErrorTextBlock.Text = "Systolic blood pressure is required. Example: '120'.";
                this.SystolicErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!int.TryParse(this.SystolicTextBox.Text, out var systolic) || systolic < 90 || systolic > 200)
            {
                this.SystolicErrorTextBlock.Text = "Please enter a valid systolic pressure between 90 and 200. Example: '120'.";
                this.SystolicErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.DiastolicTextBox.Text))
            {
                this.DiastolicErrorTextBlock.Text = "Diastolic blood pressure is required. Example: '80'.";
                this.DiastolicErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!int.TryParse(this.DiastolicTextBox.Text, out var diastolic) || diastolic < 60 || diastolic > 120)
            {
                this.DiastolicErrorTextBlock.Text = "Please enter a valid diastolic pressure between 60 and 120. Example: '80'.";
                this.DiastolicErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.BodyTempTextBox.Text))
            {
                this.BodyTempErrorTextBlock.Text = "Body temperature is required. Example: '98.6'.";
                this.BodyTempErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!decimal.TryParse(this.BodyTempTextBox.Text, out var temp) || temp < 95 || temp > 107)
            {
                this.BodyTempErrorTextBlock.Text = "Please enter a valid body temperature between 95°F and 107°F. Example: '98.6'.";
                this.BodyTempErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.WeightTextBox.Text))
            {
                this.WeightErrorTextBlock.Text = "Weight is required. Example: '150'.";
                this.WeightErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!decimal.TryParse(this.WeightTextBox.Text, out var weight) || weight <= 0 || weight > 500)
            {
                this.WeightErrorTextBlock.Text = "Please enter a valid weight (greater than 0 and less than 500). Example: '150'.";
                this.WeightErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.HeightTextBox.Text))
            {
                this.HeightErrorTextBlock.Text = "Height is required. Example: '5.9'.";
                this.HeightErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!decimal.TryParse(this.HeightTextBox.Text, out var height) || height <= 0 || height > 10)
            {
                this.HeightErrorTextBlock.Text = "Please enter a valid height (greater than 0 and less than 10). Example: '5.9'.";
                this.HeightErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.PulseTextBox.Text))
            {
                this.PulseErrorTextBlock.Text = "Pulse rate is required. Example: '72'.";
                this.PulseErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!int.TryParse(this.PulseTextBox.Text, out var pulse) || pulse < 30 || pulse > 200)
            {
                this.PulseErrorTextBlock.Text = "Please enter a valid pulse rate between 30 and 200. Example: '72'.";
                this.PulseErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(this.SymptomsTextBox.Text))
            {
                this.SymptomsErrorTextBlock.Text = "Please describe the patient's symptoms. Example: 'Fever, fatigue, and cough.'";
                this.SymptomsErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(this.LDLResultTextBox.Text) && this.LowDensityLipoproteinsCheckBox.IsChecked != true)
            {
                this.LDLResultErrorTextBlock.Text = "You must check the 'Low Density Lipoproteins' checkbox if providing a result. Example: '120 mg/dL : 12:00 PM'.";
                this.LDLResultErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(this.WBCResultTextBox.Text) && this.WhiteBloodCellCheckBox.IsChecked != true)
            {
                this.WBCResultErrorTextBlock.Text = "You must check the 'White Blood Cell' checkbox if providing a result. Example: '4500 cells/uL : 10:30 AM'.";
                this.WBCResultErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!isValid)
            {
                this.ErrorTextBlock.Text = "Please correct the errors highlighted above.";
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }

            return isValid;
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void WBCLowValTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
