using Windows.UI.Xaml;
using HealthcareSystem.Page;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HealthcareSystem
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNursePage : BasePage
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddNursePage" /> class and sets up the user interface for adding a
        ///     nurse.
        /// </summary>
        public AddNursePage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        private void AddNurse_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdminPage));
        }

        #endregion
    }
}