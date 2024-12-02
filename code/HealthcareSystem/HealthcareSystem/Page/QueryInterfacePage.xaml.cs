using System;
using System.Data;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using DBAccess.DAL;
using HealthcareSystem.DAL;
using HealthcareSystem.Page;
using MySql.Data.MySqlClient;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QueryInterfacePage : BasePage
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryInterfacePage" /> class.
        /// </summary>
        public QueryInterfacePage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        private void ExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var isDarkMode = SessionManager.Instance.IsDarkModeEnabled();

                var lightThemeBorderColor = Colors.Gray;
                var lightThemeRowColor1 = Colors.White;
                var lightThemeRowColor2 = Colors.WhiteSmoke;

                var darkThemeBorderColor = Colors.DarkGray;
                var darkThemeRowColor1 = Colors.Black;
                var darkThemeRowColor2 = Colors.DimGray;

                var borderColor = isDarkMode ? darkThemeBorderColor : lightThemeBorderColor;
                var rowColor1 = isDarkMode ? darkThemeRowColor1 : lightThemeRowColor1;
                var rowColor2 = isDarkMode ? darkThemeRowColor2 : lightThemeRowColor2;

                var query = this.SqlQueryTextBox.Text;

                using var connection = new MySqlConnection(Connection.ConnectionString());
                connection.Open();

                using var command = new MySqlCommand(query, connection);
                using var adapter = new MySqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                this.ResultsGrid.Children.Clear();
                this.ResultsGrid.RowDefinitions.Clear();
                this.ResultsGrid.ColumnDefinitions.Clear();

                for (var colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                {
                    this.ResultsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                }

                this.ResultsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                for (var colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                {
                    var headerBorder = new Border
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(borderColor),
                        Background = new SolidColorBrush(isDarkMode ? Colors.DarkSlateGray : Colors.LightGray),
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
                    this.ResultsGrid.Children.Add(headerBorder);
                }

                for (var rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    this.ResultsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    for (var colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                    {
                        var cellBorder = new Border
                        {
                            BorderThickness = new Thickness(1),
                            BorderBrush = new SolidColorBrush(borderColor),
                            Background = new SolidColorBrush(rowIndex % 2 == 0 ? rowColor1 : rowColor2),
                            Padding = new Thickness(5)
                        };

                        var cellText = new TextBlock
                        {
                            Text = dataTable.Rows[rowIndex][colIndex]?.ToString(),
                            TextWrapping = TextWrapping.Wrap
                        };

                        cellBorder.Child = cellText;

                        Grid.SetRow(cellBorder, rowIndex + 1);
                        Grid.SetColumn(cellBorder, colIndex);
                        this.ResultsGrid.Children.Add(cellBorder);
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        #endregion
    }
}