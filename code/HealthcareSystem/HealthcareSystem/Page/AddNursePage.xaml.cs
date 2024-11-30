using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HealthcareSystem.Page;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNursePage : BasePage
    {
        public AddNursePage()
        {
            this.InitializeComponent();
        }

        private void AddNurse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdminPage));
        }
    }
}
