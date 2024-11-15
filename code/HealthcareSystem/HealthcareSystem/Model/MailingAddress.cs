using System;
using System.Diagnostics;

namespace DBAccess.Model
{
    /// <summary>
    ///     The Employee Class.
    /// </summary>
    public class MailingAddress
    {
        public string StreetAddress { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public MailingAddress(string streetAddress, string zip, string city, string state, string country)
        {
            this.StreetAddress = streetAddress ??
                                 throw new ArgumentNullException(nameof(streetAddress),
                                     $"{nameof(streetAddress)} cannot be null");
            Debug.WriteLine("State: " + this.State);
            this.City = city ?? throw new ArgumentNullException(nameof(city), $"{nameof(city)} cannot be null");
            Debug.WriteLine("City: " + this.City);
            this.State = state ?? throw new ArgumentNullException(nameof(state), $"{nameof(state)} cannot be null");
            Debug.WriteLine("State: " + this.State);
            this.Country = country ??
                           throw new ArgumentNullException(nameof(country), $"{nameof(country)} cannot be null");
            Debug.WriteLine("State: " + this.State);

            if (zip.Length < 5)
            {
                throw new ArgumentOutOfRangeException(nameof(zip), $"{nameof(zip)} cannot be negative");
            }

            this.Zip = zip;
            Debug.WriteLine("Zip: " + this.Zip);
        }
    }
}