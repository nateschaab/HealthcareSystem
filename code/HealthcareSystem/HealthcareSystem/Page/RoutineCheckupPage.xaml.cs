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
                        checkup.LabTests = dal.GetLabTestsForVisit(checkup.VisitId);
                        this.ClearErrorMessages();
                        this.ClearCheckboxes();
                        this.PopulateCheckupFields(checkup);
                    }
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

            LDLResultTextBox.Text = string.Empty;
            WBCResultTextBox.Text = string.Empty;
            HBResultTextBox.Text = string.Empty;
            HAResultTextBox.Text = string.Empty;

            if (checkup.LabTests != null)
            {
                foreach (var labTest in checkup.LabTests)
                {
                    if (labTest.Result != null && labTest.TestTypeName != null && labTest.TestTypeName.Contains("Low Density Lipoproteins"))
                    {
                        LDLResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                    else if (labTest.Result != null && labTest.TestTypeName != null && labTest.TestTypeName.Contains("Hepatitis A"))
                    {
                        HAResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                    else if (labTest.Result != null && labTest.TestTypeName != null && labTest.TestTypeName.Contains("Hepatitis B"))
                    {
                        HBResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                    else if (labTest.Result != null && labTest.TestTypeName != null && labTest.TestTypeName.Contains("White Blood Cell"))
                    {
                        WBCResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                }
            }

            LDLResultTextBox.IsReadOnly = hasFinalDiagnosis;
            LDLResultTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            WBCResultTextBox.IsReadOnly = hasFinalDiagnosis;
            WBCResultTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            HBResultTextBox.IsReadOnly = hasFinalDiagnosis;
            HBResultTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            HAResultTextBox.IsReadOnly = hasFinalDiagnosis;
            HAResultTextBox.IsHitTestVisible = !hasFinalDiagnosis;

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
            var testTypes = _testTypeDAL.GetAllTestTypes();

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

                int appointmentId = (AppointmentComboBox.SelectedItem as Appointment).AppointmentId;

                string bloodPressureReading = $"{int.Parse(SystolicTextBox.Text)}/{int.Parse(DiastolicTextBox.Text)}";
                decimal bodyTemp = decimal.Parse(BodyTempTextBox.Text);
                decimal weight = decimal.Parse(WeightTextBox.Text);
                decimal height = decimal.Parse(HeightTextBox.Text);
                int pulse = int.Parse(PulseTextBox.Text);
                string symptoms = SymptomsTextBox.Text;
                string initialDiagnosis = InitialDiagnosisTextBox.Text;
                string finalDiagnosis = FinalDiagnosisTextBox.Text;

                var selectedTestTypes = new List<string>();
                if (LowDensityLipoproteinsCheckBox.IsChecked == true)
                    selectedTestTypes.Add("Low Density Lipoproteins");
                if (HepatitisACheckBox.IsChecked == true)
                    selectedTestTypes.Add("Hepatitis A");
                if (HepatitisBCheckBox.IsChecked == true)
                    selectedTestTypes.Add("Hepatitis B");
                if (WhiteBloodCellCheckBox.IsChecked == true)
                    selectedTestTypes.Add("White Blood Cell");

                var testResults = new Dictionary<string, string>();

                if (LowDensityLipoproteinsCheckBox.IsChecked == true)
                {
                    string ldlResult = ParseResult(LDLResultTextBox.Text);
                    testResults.Add("Low Density Lipoproteins", ldlResult);
                }

                if (HepatitisACheckBox.IsChecked == true)
                {
                    string haResult = ParseResult(HAResultTextBox.Text);
                    testResults.Add("Hepatitis A", haResult);
                }

                if (HepatitisBCheckBox.IsChecked == true)
                {
                    string hbResult = ParseResult(HBResultTextBox.Text);
                    testResults.Add("Hepatitis B", hbResult);
                }

                if (WhiteBloodCellCheckBox.IsChecked == true)
                {
                    string wbcResult = ParseResult(WBCResultTextBox.Text);
                    testResults.Add("White Blood Cell", wbcResult);
                }

                bool success = _visitDAL.CompleteRoutineCheckupWithTests(
                    appointmentId, bloodPressureReading, bodyTemp, weight, height,
                    pulse, symptoms, initialDiagnosis, finalDiagnosis, selectedTestTypes, testResults);

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
            ClearErrorMessages();

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

            if (!string.IsNullOrWhiteSpace(LDLResultTextBox.Text) && LowDensityLipoproteinsCheckBox.IsChecked != true)
            {
                LDLResultErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                LDLResultErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(WBCResultTextBox.Text) && WhiteBloodCellCheckBox.IsChecked != true)
            {
                WBCResultErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                WBCResultErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(HBResultTextBox.Text) && HepatitisBCheckBox.IsChecked != true)
            {
                HBResultErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                HBResultErrorTextBlock.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrWhiteSpace(HAResultTextBox.Text) && HepatitisACheckBox.IsChecked != true)
            {
                HAResultErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                HAResultErrorTextBlock.Visibility = Visibility.Collapsed;
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
