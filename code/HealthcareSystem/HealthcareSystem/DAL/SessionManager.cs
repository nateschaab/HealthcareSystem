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
    }
}