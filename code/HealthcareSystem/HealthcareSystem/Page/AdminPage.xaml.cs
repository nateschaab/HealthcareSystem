using Windows.UI.Xaml;
using HealthcareSystem.DAL;
using HealthcareSystem.Page;

namespace HealthcareSystem
{
    /// <summary>
    ///     Represents the admin dashboard page, providing navigation to various admin functions such as adding a nurse,
    ///     accessing the query interface, generating reports, and logging out.
    /// </summary>
    public sealed partial class AdminPage : BasePage
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdminPage" /> class.
        /// </summary>
        /// <remarks>
        ///     This constructor sets up the admin dashboard page by initializing its components.
        /// </remarks>
        public AdminPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handles the event when the "Add Nurse" button is clicked. Navigates to the <see cref="AddNursePage" />.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void AddNurse_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddNursePage));
        }

        /// <summary>
        ///     Handles the event when the "Query Interface" button is clicked. Navigates to the <see cref="QueryInterfacePage" />.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void QueryInterface_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(QueryInterfacePage));
        }

        /// <summary>
        ///     Handles the event when the "Generate Report" button is clicked. Navigates to the <see cref="ReportPage" />.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ReportPage));
        }

        /// <summary>
        ///     Handles the event when the "Logout" button is clicked. Clears the current user session and navigates
        ///     to the <see cref="LoginPage" />.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Instance.Username = null;
            SessionManager.Instance.FirstName = null;
            SessionManager.Instance.LastName = null;

            Frame.Navigate(typeof(LoginPage));
        }

        #endregion
    }
}