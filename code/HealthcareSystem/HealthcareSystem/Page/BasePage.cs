﻿using System.Diagnostics;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

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

            this.Loaded += (sender, args) =>
            {
                var rootGrid = (Grid)this.Content;
                double containerWidth = rootGrid.ActualWidth;
                double containerHeight = rootGrid.ActualHeight;

                ApplicationView.GetForCurrentView().TryResizeView(new Windows.Foundation.Size(containerWidth, containerHeight));
            };

            if (this.FindName("UserInfo") is TextBlock userInfoTextBlock)
            {
                this.LoadUserInfo(userInfoTextBlock);
            }
        }
    }
}