namespace DBAccess.Model
{
    public class Nurse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        public string FullName => $"{this.FirstName} {this.LastName}";
    }
}
