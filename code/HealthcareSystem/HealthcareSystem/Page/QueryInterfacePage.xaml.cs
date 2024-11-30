using DBAccess.DAL;
using HealthcareSystem.Page;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
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
    public sealed partial class QueryInterfacePage : BasePage
    {
        public QueryInterfacePage()
        {
            this.InitializeComponent();
        }

        private void ExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = SqlQueryTextBox.Text;

                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                using var command = new MySqlCommand(query, connection);
                using var adapter = new MySqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Clear previous results
                ResultsGrid.Children.Clear();
                ResultsGrid.RowDefinitions.Clear();
                ResultsGrid.ColumnDefinitions.Clear();

                // Create column headers
                for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                {
                    ResultsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    var headerText = new TextBlock
                    {
                        Text = dataTable.Columns[colIndex].ColumnName,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(5),
                        TextWrapping = TextWrapping.Wrap
                    };
                    Grid.SetRow(headerText, 0);
                    Grid.SetColumn(headerText, colIndex);
                    ResultsGrid.Children.Add(headerText);
                }

                // Populate rows with data
                for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    ResultsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                    {
                        var cellText = new TextBlock
                        {
                            Text = dataTable.Rows[rowIndex][colIndex]?.ToString(),
                            Margin = new Thickness(5),
                            TextWrapping = TextWrapping.Wrap
                        };
                        Grid.SetRow(cellText, rowIndex + 1); // +1 because row 0 is for headers
                        Grid.SetColumn(cellText, colIndex);
                        ResultsGrid.Children.Add(cellText);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

    }
}
