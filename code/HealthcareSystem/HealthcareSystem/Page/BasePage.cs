using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using HealthcareSystem.DAL;

namespace HealthcareSystem.Page
{
    /// <summary>
    ///     Serves as a base class for pages in the HealthcareSystem application, providing common functionality.
    /// </summary>
    public class BasePage : Windows.UI.Xaml.Controls.Page
    {
        #region Methods

        /// <summary>
        ///     Loads the logged-in user's information into a specified <see cref="TextBlock" />.
        /// </summary>
        /// <param name="userInfoTextBlock">The <see cref="TextBlock" /> to display the user's information.</param>
        protected void LoadUserInfo(TextBlock userInfoTextBlock)
        {
            var firstName = SessionManager.Instance.FirstName;
            var lastName = SessionManager.Instance.LastName;
            var username = SessionManager.Instance.Username;

            if (userInfoTextBlock != null)
            {
                userInfoTextBlock.Text = $"Logged in as: {firstName} {lastName} (Username: {username})";
            }

            Debug.WriteLine($"User Info Loaded: {firstName} {lastName} (Username: {username})");
        }

        /// <summary>
        ///     Invoked when the page is navigated to. Handles resizing the view and loading user information.
        /// </summary>
        /// <param name="e">Event data containing information about the navigation event.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Loaded += (sender, args) =>
            {
                var pageWidth = ActualWidth;
                var pageHeight = ActualHeight;

                if (!ApplicationView.GetForCurrentView().TryResizeView(new Size(pageWidth, pageHeight)))
                {
                    Debug.WriteLine("Failed to resize to page dimensions.");
                }
            };

            if (FindName("UserInfo") is TextBlock userInfoTextBlock)
            {
                this.LoadUserInfo(userInfoTextBlock);
            }
        }

        #endregion
    }
}