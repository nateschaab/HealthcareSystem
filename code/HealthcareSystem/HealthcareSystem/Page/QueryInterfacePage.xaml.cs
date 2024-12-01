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
                // Determine if dark mode is enabled
                bool isDarkMode = SessionManager.Instance.IsDarkModeEnabled();

                // Define colors for light and dark themes
                var lightThemeBorderColor = Windows.UI.Colors.Gray;
                var lightThemeRowColor1 = Windows.UI.Colors.White;
                var lightThemeRowColor2 = Windows.UI.Colors.WhiteSmoke;

                var darkThemeBorderColor = Windows.UI.Colors.DarkGray;
                var darkThemeRowColor1 = Windows.UI.Colors.Black;
                var darkThemeRowColor2 = Windows.UI.Colors.DimGray;

                var borderColor = isDarkMode ? darkThemeBorderColor : lightThemeBorderColor;
                var rowColor1 = isDarkMode ? darkThemeRowColor1 : lightThemeRowColor1;
                var rowColor2 = isDarkMode ? darkThemeRowColor2 : lightThemeRowColor2;

                // Get the query from the text box
                string query = SqlQueryTextBox.Text;

                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                using var command = new MySqlCommand(query, connection);
                using var adapter = new MySqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                ResultsGrid.Children.Clear();
                ResultsGrid.RowDefinitions.Clear();
                ResultsGrid.ColumnDefinitions.Clear();

                for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                {
                    ResultsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                }

                ResultsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                {
                    var headerBorder = new Border
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(borderColor),
                        Background = new SolidColorBrush(isDarkMode ? Windows.UI.Colors.DarkSlateGray : Windows.UI.Colors.LightGray),
                        Padding = new Thickness(5)
                    };

                    var headerText = new TextBlock
                    {
                        Text = dataTable.Columns[colIndex].ColumnName,
                        FontWeight = FontWeights.Bold,
                        TextWrapping = TextWrapping.Wrap
                    };

                    headerBorder.Child = headerText;

                    Grid.SetRow(headerBorder, 0);
                    Grid.SetColumn(headerBorder, colIndex);
                    ResultsGrid.Children.Add(headerBorder);
                }

                for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    ResultsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                    {
                        var cellBorder = new Border
                        {
                            BorderThickness = new Thickness(1),
                            BorderBrush = new SolidColorBrush(borderColor),
                            Background = new SolidColorBrush((rowIndex % 2 == 0) ? rowColor1 : rowColor2),
                            Padding = new Thickness(5)
                        };

                        var cellText = new TextBlock
                        {
                            Text = dataTable.Rows[rowIndex][colIndex]?.ToString(),
                            TextWrapping = TextWrapping.Wrap
                        };

                        cellBorder.Child = cellText;

                        Grid.SetRow(cellBorder, rowIndex + 1); // Offset by 1 to account for headers
                        Grid.SetColumn(cellBorder, colIndex);
                        ResultsGrid.Children.Add(cellBorder);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog($"Error: {ex.Message}");
            }

            async void ShowErrorDialog(string message)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = message,
                    CloseButtonText = "OK"
                };

                await errorDialog.ShowAsync();
            }
        }

    }
}
