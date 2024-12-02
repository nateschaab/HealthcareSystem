using System;
using System.Data;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using DBAccess.DAL;
using HealthcareSystem.DAL;
using HealthcareSystem.Page;
using MySql.Data.MySqlClient;

namespace HealthcareSystem
{
    public sealed partial class ReportPage : BasePage
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportPage" /> class.
        /// </summary>
        public ReportPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var startDate = this.StartDatePicker.SelectedDate?.ToString("yyyy-MM-dd");
                var endDate = this.EndDatePicker.SelectedDate?.ToString("yyyy-MM-dd");

                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    this.ShowErrorDialog("Please select both start and end dates.");
                    return;
                }

                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                var query = @"
                SELECT 
                    v.datetime AS VisitDate,
                    p.patient_id AS PatientId,
                    CONCAT(pr.fname, ' ', pr.lname) AS PatientName,
                    CONCAT(dpr.fname, ' ', dpr.lname) AS DoctorName,
                    CONCAT(npr.fname, ' ', npr.lname) AS NurseName,
                    t.test_type_name AS TestNames,
                    t.time_performed AS TestDates,
                    t.result AS TestResults,
                    t.abnormality AS Abnormality,
                    v.final_diagnosis AS Diagnosis
                FROM 
                    visit v
                    JOIN appointment a ON v.appt_id = a.appt_id
                    JOIN patient p ON a.patient_id = p.patient_id
                    JOIN person pr ON p.pid = pr.pid
                    LEFT JOIN doctor d ON a.doctor_id = d.doctor_id
                    LEFT JOIN person dpr ON d.pid = dpr.pid
                    LEFT JOIN nurse n ON a.doctor_id = n.nurse_id
                    LEFT JOIN person npr ON n.pid = npr.pid
                    LEFT JOIN lab_test t ON v.visit_id = t.visit_id
                WHERE 
                    v.datetime BETWEEN @StartDate AND @EndDate
                ORDER BY 
                    v.datetime, pr.lname;";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                using var adapter = new MySqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                this.VisitReportGrid.RowDefinitions.Clear();
                this.VisitReportGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                this.VisitReportGrid.Children.Clear();

                this.AddHeaderRow();

                var rowIndex = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    this.VisitReportGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    for (var colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                    {
                        var cellText = new TextBlock
                        {
                            Text = row[colIndex]?.ToString(),
                            Margin = new Thickness(5),
                            TextWrapping = TextWrapping.Wrap,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        Grid.SetRow(cellText, rowIndex);
                        Grid.SetColumn(cellText, colIndex);
                        this.VisitReportGrid.Children.Add(cellText);
                    }

                    rowIndex++;
                }

                this.ApplyThemeBasedStyles();
            }
            catch (Exception ex)
            {
                this.ShowErrorDialog($"Error: {ex.Message}");
            }
        }

        private void AddHeaderRow()
        {
            var headers = new[]
            {
                "Visit Date", "Patient ID", "Patient Name", "Doctor Name", "Nurse Name", "Test Names", "Test Dates",
                "Test Results", "Abnormality", "Diagnosis"
            };

            for (var colIndex = 0; colIndex < headers.Length; colIndex++)
            {
                var headerText = new TextBlock
                {
                    Text = headers[colIndex],
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center
                };

                Grid.SetRow(headerText, 0);
                Grid.SetColumn(headerText, colIndex);
                this.VisitReportGrid.Children.Add(headerText);
            }
        }

        private async void ShowErrorDialog(string message)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK"
            };

            await errorDialog.ShowAsync();
        }

        private bool IsDarkModeEnabled()
        {
            var uiSettings = new UISettings();
            var backgroundColor = uiSettings.GetColorValue(UIColorType.Background);
            return backgroundColor == Colors.Black;
        }

        private void ApplyThemeBasedStyles()
        {
            var isDarkMode = this.IsDarkModeEnabled();

            var lightRowColor = new SolidColorBrush(Colors.WhiteSmoke);
            var darkRowColor = new SolidColorBrush(Colors.Black);
            var alternateLightRowColor = new SolidColorBrush(Colors.White);
            var alternateDarkRowColor = new SolidColorBrush(Colors.DimGray);

            for (var rowIndex = 1; rowIndex < this.VisitReportGrid.RowDefinitions.Count; rowIndex++)
            {
                foreach (var child in this.VisitReportGrid.Children)
                {
                    if (child is Border border && Grid.GetRow(border) == rowIndex)
                    {
                        border.Background = isDarkMode
                            ? rowIndex % 2 == 0 ? darkRowColor : alternateDarkRowColor
                            : rowIndex % 2 == 0
                                ? lightRowColor
                                : alternateLightRowColor;
                    }
                }
            }
        }

        #endregion
    }
}