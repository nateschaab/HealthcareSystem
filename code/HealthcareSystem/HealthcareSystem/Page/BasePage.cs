using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using HealthcareSystem;

namespace HealthcareSystem.Page
{
    public class BasePage : Windows.UI.Xaml.Controls.Page
    {
        protected void LoadUserInfo(TextBlock userInfoTextBlock)
        {
            string firstName = SessionManager.Instance.FirstName;
            string lastName = SessionManager.Instance.LastName;
            string username = SessionManager.Instance.Username;

            if (userInfoTextBlock != null)
            {
                userInfoTextBlock.Text = $"Logged in as: {firstName} {lastName} (Username: {username})";
            }

            Debug.WriteLine($"User Info Loaded: {firstName} {lastName} (Username: {username})");
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (this.FindName("UserInfo") is TextBlock userInfoTextBlock)
            {
                this.LoadUserInfo(userInfoTextBlock);
            }
        }

    }
}