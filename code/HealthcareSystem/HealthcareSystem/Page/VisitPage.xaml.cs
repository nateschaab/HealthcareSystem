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
    public sealed partial class VisitPage : Page
    {
        public VisitPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Appointment app)
            {
                var dal = new VisitDAL();
                var checkups = dal.GetRoutineCheckups();
                foreach (var checkup in checkups)
                {
                    if (checkup.AppointmentId == app.AppointmentId)
                    {
                        this.AppointmentComboBox.ItemsSource = new List<Appointment> { app };
                        this.AppointmentComboBox.SelectedItem = AppointmentComboBox.Items[0];

                        checkup.LabTests = dal.GetLabTestsForVisit(checkup.VisitId);
                        this.ClearErrorMessages();
                        this.PopulateCheckupFields(checkup);
                    }
                }
            }
        }

        private void ClearErrorMessages()
        {
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
        }

        private void PopulateCheckupFields(RoutineCheckup checkup)
        {
            bool hasFinalDiagnosis = true;

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

            this.InitialDiagnosisTextBox.Text = checkup.InitialDiagnosis ?? null;
            this.InitialDiagnosisTextBox.IsReadOnly = hasFinalDiagnosis;
            this.InitialDiagnosisTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.FinalDiagnosisTextBox.Text = checkup.FinalDiagnosis ?? null;
            this.FinalDiagnosisTextBox.IsReadOnly = hasFinalDiagnosis;
            this.FinalDiagnosisTextBox.IsHitTestVisible = !hasFinalDiagnosis;

            this.LowDensityLipoproteinsCheckBox.IsChecked = checkup.LabTests.Count(l => l.TestTypeName.Contains("Low Density Lipoproteins")) > 0;
            this.HepatitisACheckBox.IsChecked = checkup.LabTests.Count(l => l.TestTypeName.Contains("Hepatitis A")) > 0;
            this.HepatitisBCheckBox.IsChecked = checkup.LabTests.Count(l => l.TestTypeName.Contains("Hepatitis B")) > 0;
            this.WhiteBloodCellCheckBox.IsChecked = checkup.LabTests.Count(l => l.TestTypeName.Contains("White Blood Cell")) > 0;

            this.LowDensityLipoproteinsCheckBox.IsEnabled = !hasFinalDiagnosis;
            this.HepatitisACheckBox.IsEnabled = !hasFinalDiagnosis;
            this.HepatitisBCheckBox.IsEnabled = !hasFinalDiagnosis;
            this.WhiteBloodCellCheckBox.IsEnabled = !hasFinalDiagnosis;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
