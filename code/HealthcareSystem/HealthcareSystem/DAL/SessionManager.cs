using Windows.UI.ViewManagement;

namespace HealthcareSystem
{
    public class SessionManager
    {
        private static SessionManager _instance;

        private SessionManager() { }

        public static SessionManager Instance => _instance ??= new SessionManager();

        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsDarkModeEnabled()
        {
            var uiSettings = new UISettings();
            var backgroundColor = uiSettings.GetColorValue(UIColorType.Background);
            return backgroundColor == Windows.UI.Colors.Black;
        }

    }
}