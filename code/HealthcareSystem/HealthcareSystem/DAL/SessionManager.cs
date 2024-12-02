using Windows.UI;
using Windows.UI.ViewManagement;

namespace HealthcareSystem.DAL
{
    /// <summary>
    ///     Gets the singleton instance of the <see cref="SessionManager" /> class.
    /// </summary>
    public class SessionManager
    {
        #region Data members

        private static SessionManager _instance;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the singleton instance of the <see cref="SessionManager" /> class.
        /// </summary>

        public static SessionManager Instance => _instance ??= new SessionManager();

        /// <summary>
        ///     Gets or sets the username of the current user.
        /// </summary>

        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the first name of the current user.
        /// </summary>

        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the last name of the current user.
        /// </summary>

        public string LastName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SessionManager" /> class.
        ///     This constructor is private to enforce the singleton pattern.
        /// </summary>
        private SessionManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Determines whether the operating system is currently using dark mode.
        /// </summary>
        /// <returns><c>true</c> if dark mode is enabled; otherwise, <c>false</c>.</returns>
        public bool IsDarkModeEnabled()
        {
            var uiSettings = new UISettings();
            var backgroundColor = uiSettings.GetColorValue(UIColorType.Background);
            return backgroundColor == Colors.Black;
        }

        #endregion
    }
}