using DBAccess.DAL;
using HealthcareSystem.Page;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportPage : BasePage
    {
        public ReportPage()
        {
            this.InitializeComponent();
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var startDate = StartDatePicker.SelectedDate?.ToString("yyyy-MM-dd");
                var endDate = EndDatePicker.SelectedDate?.ToString("yyyy-MM-dd");

                if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                {
                    //MessageBox.Show("Please select both start and end dates.");
                    return;
                }

                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                string query = @"
            SELECT 
                v.visit_date,
                p.patient_id,
                CONCAT(p.first_name, ' ', p.last_name) AS patient_name,
                d.name AS doctor_name,
                n.name AS nurse_name,
                t.test_name,
                t.time_performed AS test_date,
                t.result AS test_result,
                t.abnormality,
                v.diagnosis
            FROM 
                visit v
                JOIN patients p ON v.patient_id = p.patient_id
                LEFT JOIN doctors d ON v.doctor_id = d.doctor_id
                LEFT JOIN nurses n ON v.nurse_id = n.nurse_id
                LEFT JOIN lab_tests t ON v.visit_id = t.visit_id
            WHERE 
                v.visit_date BETWEEN @StartDate AND @EndDate
            ORDER BY 
                v.visit_date, p.last_name;";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                using var adapter = new MySqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Convert DataTable to List of dynamic objects
                var results = new List<dynamic>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var obj = new ExpandoObject() as IDictionary<string, object>;
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        obj[column.ColumnName] = row[column];
                    }
                    results.Add(obj);
                }

                VisitReportListView.ItemsSource = results;
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error: {ex.Message}");
            }
        }

    }
}
