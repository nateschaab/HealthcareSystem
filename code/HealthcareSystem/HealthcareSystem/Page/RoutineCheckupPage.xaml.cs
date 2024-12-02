using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DBAccess.DAL;
using HealthcareSystem.DAL;
using HealthcareSystem.Model;
using HealthcareSystem.Page;

namespace HealthcareSystem
{
    public sealed partial class RoutineCheckupPage : BasePage
    {
        #region Data members

        /// <summary>
        ///     Data access layer for handling visit-related database operations.
        /// </summary>
        private readonly VisitDAL _visitDAL = new VisitDAL();

        /// <summary>
        ///     Data access layer for retrieving available test types from the database.
        /// </summary>
        private readonly TestTypeDAL _testTypeDAL = new TestTypeDAL();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RoutineCheckupPage" /> class and loads available test types.
        /// </summary>
        public RoutineCheckupPage()
        {
            this.InitializeComponent();
            this.LoadTestTypes();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handles navigation to this page, populating the list of appointments or displaying an error message if none are
        ///     found.
        /// </summary>
        /// <param name="e">Navigation event arguments containing parameter data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is List<Appointment> app)
            {
                this.AppointmentComboBox.ItemsSource = app;
            }
            else if (e.Parameter is Appointment appSingle)
            {
                this.AppointmentComboBox.ItemsSource = new List<Appointment> { appSingle };
            }
            else
            {
                this.ErrorTextBlock.Text = "No appointments found.";
                this.ErrorTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     Handles changes in the selected appointment from the appointment combo box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments containing details about the selection change.</param>
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

        /// <summary>
        ///     Clears all lab test-related checkboxes on the page.
        /// </summary>
        private void ClearCheckboxes()
        {
            this.LowDensityLipoproteinsCheckBox.IsChecked = false;
            this.HepatitisACheckBox.IsChecked = false;
            this.HepatitisBCheckBox.IsChecked = false;
            this.WhiteBloodCellCheckBox.IsChecked = false;
        }

        /// <summary>
        ///     Clears all error messages displayed on the page.
        /// </summary>
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

        /// <summary>
        ///     Populates the UI fields with the details of the specified routine checkup.
        /// </summary>
        /// <param name="checkup">The routine checkup to display in the UI fields.</param>
        private void PopulateCheckupFields(RoutineCheckup checkup)
        {
            var hasFinalDiagnosis = !string.IsNullOrWhiteSpace(checkup.FinalDiagnosis);

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
                    if (labTest.Result != null && labTest.TestTypeName != null &&
                        labTest.TestTypeName.Contains("Low Density Lipoproteins"))
                    {
                        this.LDLResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                    else if (labTest.Result != null && labTest.TestTypeName != null &&
                             labTest.TestTypeName.Contains("Hepatitis A"))
                    {
                        this.HAResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                    else if (labTest.Result != null && labTest.TestTypeName != null &&
                             labTest.TestTypeName.Contains("Hepatitis B"))
                    {
                        this.HBResultTextBox.Text = labTest.Result + " : " + labTest.TimePerformed;
                    }
                    else if (labTest.Result != null && labTest.TestTypeName != null &&
                             labTest.TestTypeName.Contains("White Blood Cell"))
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

            foreach (var test in checkup.LabTests)
            {
                if (test.TestTypeName != null && test.TestTypeName.Contains("Low Density Lipoproteins"))
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

        /// <summary>
        ///     Loads test types from the database and updates the content of the lab test checkboxes.
        /// </summary>
        private void LoadTestTypes()
        {
            var testTypes = this._testTypeDAL.GetAllTestTypes();

            this.LowDensityLipoproteinsCheckBox.Content =
                testTypes.FirstOrDefault(t => t.Contains("Low Density Lipoproteins")) ?? "Low Density Lipoproteins";
            this.HepatitisACheckBox.Content = testTypes.FirstOrDefault(t => t.Contains("Hepatitis A")) ?? "Hepatitis A";
            this.HepatitisBCheckBox.Content = testTypes.FirstOrDefault(t => t.Contains("Hepatitis B")) ?? "Hepatitis B";
            this.WhiteBloodCellCheckBox.Content =
                testTypes.FirstOrDefault(t => t.Contains("White Blood Cell")) ?? "White Blood Cell";
        }

        /// <summary>
        ///     Handles the completion of the routine checkup when the "Complete Checkup" button is clicked.
        ///     Validates input fields, performs database operations, and updates the UI with the result.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void CompleteCheckupButton_Click(object sender, RoutedEventArgs e)
        {
            var hasFinalDiagnosis = !string.IsNullOrWhiteSpace(this.FinalDiagnosisTextBox.Text);

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

                var appointmentId = (this.AppointmentComboBox.SelectedItem as Appointment).AppointmentId;

                var bloodPressureReading =
                    $"{int.Parse(this.SystolicTextBox.Text)}/{int.Parse(this.DiastolicTextBox.Text)}";
                var bodyTemp = decimal.Parse(this.BodyTempTextBox.Text);
                var weight = decimal.Parse(this.WeightTextBox.Text);
                var height = decimal.Parse(this.HeightTextBox.Text);
                var pulse = int.Parse(this.PulseTextBox.Text);
                var symptoms = this.SymptomsTextBox.Text;
                var initialDiagnosis = this.InitialDiagnosisTextBox.Text;
                var finalDiagnosis = this.FinalDiagnosisTextBox.Text;

                var selectedTestTypes = new List<string>();
                if (this.LowDensityLipoproteinsCheckBox.IsChecked == true)
                {
                    selectedTestTypes.Add("Low Density Lipoproteins");
                }

                if (this.HepatitisACheckBox.IsChecked == true)
                {
                    selectedTestTypes.Add("Hepatitis A");
                }

                if (this.HepatitisBCheckBox.IsChecked == true)
                {
                    selectedTestTypes.Add("Hepatitis B");
                }

                if (this.WhiteBloodCellCheckBox.IsChecked == true)
                {
                    selectedTestTypes.Add("White Blood Cell");
                }

                var testResults = new Dictionary<string, string>();

                if (this.LowDensityLipoproteinsCheckBox.IsChecked == true)
                {
                    var ldlResult = this.ParseResult(this.LDLResultTextBox.Text);
                    testResults.Add("Low Density Lipoproteins", ldlResult);
                }

                if (this.HepatitisACheckBox.IsChecked == true)
                {
                    var haResult = this.ParseResult(this.HAResultTextBox.Text);
                    testResults.Add("Hepatitis A", haResult);
                }

                if (this.HepatitisBCheckBox.IsChecked == true)
                {
                    var hbResult = this.ParseResult(this.HBResultTextBox.Text);
                    testResults.Add("Hepatitis B", hbResult);
                }

                if (this.WhiteBloodCellCheckBox.IsChecked == true)
                {
                    var wbcResult = this.ParseResult(this.WBCResultTextBox.Text);
                    testResults.Add("White Blood Cell", wbcResult);
                }

                var success = this._visitDAL.CompleteRoutineCheckupWithTests(
                    appointmentId, bloodPressureReading, bodyTemp, weight, height,
                    pulse, symptoms, initialDiagnosis, finalDiagnosis, selectedTestTypes, testResults);

                if (success)
                {
                    this.ErrorTextBlock.Text = "Routine checkup completed successfully!";
                    this.ErrorTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    this.ErrorTextBlock.Text = "Failed to complete routine checkup.";
                    this.ErrorTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                }

                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                this.ErrorTextBlock.Text = $"Error: {ex.Message}";
                this.ErrorTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     Parses the result text from a lab test input field, extracting the value before a colon if present.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <returns>The parsed result value.</returns>
        private string ParseResult(string input)
        {
            var colonIndex = input.IndexOf(':');
            if (colonIndex != -1)
            {
                return input.Substring(0, colonIndex).Trim();
            }

            return input.Trim();
        }

        /// <summary>
        ///     Validates the input fields for completing the routine checkup, highlighting errors if validation fails.
        /// </summary>
        /// <returns>True if all fields are valid; otherwise, false.</returns>
        private bool ValidateCheckupFields()
        {
            var isValid = true;
            this.ClearErrorMessages();

            if (this.AppointmentComboBox.SelectedItem == null)
            {
                this.AppointmentErrorTextBlock.Text =
                    "Please select an appointment from the dropdown (e.g., 'John Doe - 12/01/2024').";
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
                this.SystolicErrorTextBlock.Text =
                    "Please enter a valid systolic pressure between 90 and 200. Example: '120'.";
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
                this.DiastolicErrorTextBlock.Text =
                    "Please enter a valid diastolic pressure between 60 and 120. Example: '80'.";
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
                this.BodyTempErrorTextBlock.Text =
                    "Please enter a valid body temperature between 95°F and 107°F. Example: '98.6'.";
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
                this.WeightErrorTextBlock.Text =
                    "Please enter a valid weight (greater than 0 and less than 500). Example: '150'.";
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
                this.HeightErrorTextBlock.Text =
                    "Please enter a valid height (greater than 0 and less than 10). Example: '5.9'.";
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
                this.SymptomsErrorTextBlock.Text =
                    "Please describe the patient's symptoms. Example: 'Fever, fatigue, and cough.'";
                this.SymptomsErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(this.LDLResultTextBox.Text) &&
                this.LowDensityLipoproteinsCheckBox.IsChecked != true)
            {
                this.LDLResultErrorTextBlock.Text =
                    "You must check the 'Low Density Lipoproteins' checkbox if providing a result. Example: '120 mg/dL : 12:00 PM'.";
                this.LDLResultErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(this.WBCResultTextBox.Text) && this.WhiteBloodCellCheckBox.IsChecked != true)
            {
                this.WBCResultErrorTextBlock.Text =
                    "You must check the 'White Blood Cell' checkbox if providing a result. Example: '4500 cells/uL : 10:30 AM'.";
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

        /// <summary>
        ///     Navigates back to the main page when the "Back" button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        #endregion
    }
}