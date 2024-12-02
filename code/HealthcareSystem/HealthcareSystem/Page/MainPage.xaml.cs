using Windows.UI.Xaml;
using HealthcareSystem.DAL;
using HealthcareSystem.Page;

namespace HealthcareSystem
{
    /// <summary>
    ///     Represents the main landing page of the Healthcare System application.
    ///     Provides navigation to various functional pages.
    /// </summary>
    public sealed partial class MainPage : BasePage
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.SetButtonStatesBasedOnRole();
        }

        #endregion

        #region Methods

        private void SetButtonStatesBasedOnRole()
        {
            var userRole = SessionManager.Instance.Role;

            // Admin role
            if (userRole == "Admin")
            {
                this.SearchPatientsButton.IsEnabled = false;
                this.ManagePatientsButton.IsEnabled = false;
                this.CreateAppointmentButton.IsEnabled = false;
                this.AdminFunctionsButton.IsEnabled = true;
                this.AdminFunctionsButton.Visibility = Visibility.Visible;
            }
            // Nurse role
            else if (userRole == "Nurse")
            {
                this.SearchPatientsButton.IsEnabled = true;
                this.ManagePatientsButton.IsEnabled = true;
                this.CreateAppointmentButton.IsEnabled = true;
                this.AdminFunctionsButton.IsEnabled = false;
                this.AdminFunctionsButton.Visibility = Visibility.Collapsed;
            }
            // Default case for unknown roles
            else
            {
                this.SearchPatientsButton.IsEnabled = false;
                this.ManagePatientsButton.IsEnabled = false;
                this.CreateAppointmentButton.IsEnabled = false;
                this.AdminFunctionsButton.IsEnabled = false;
            }
        }


        /// <summary>
        ///     Navigates to the <see cref="PatientManagementPage" /> for managing patient records.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ManagePatients_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PatientManagementPage));
        }

        /// <summary>
        ///     Navigates to the <see cref="SearchPatientPage" /> for searching and viewing patient details.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void SearchPatients_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPatientPage));
        }

        /// <summary>
        ///     Navigates to the <see cref="AdminPage" /> for performing administrative functions.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void AdminFunctions_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdminPage));
        }

        /// <summary>
        ///     Logs out the current user and navigates to the <see cref="LoginPage" />.
        ///     Clears the current session information.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Instance.Username = null;
            SessionManager.Instance.FirstName = null;
            SessionManager.Instance.LastName = null;

            Frame.Navigate(typeof(LoginPage));
        }

        /// <summary>
        ///     Navigates to the <see cref="AppointmentPage" /> for creating new appointments.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void CreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AppointmentPage));
        }

        #endregion
    }
}