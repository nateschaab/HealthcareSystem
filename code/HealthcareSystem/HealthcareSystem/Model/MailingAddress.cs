using System;

namespace DBAccess.Model
{
/// <summary>
/// The Employee Class.
/// </summary>
   public class MailingAddress
    {
        public string City { get; }
        public string State { get;  }
        public string Zip { get;  }

        public MailingAddress(string city, string state, string zip)
        {
            this.City = City ?? throw new ArgumentNullException(nameof(city), $"{nameof(city)} cannot be null");
            this.State = state ?? throw new ArgumentNullException(nameof(state), $"{nameof(state)} cannot be null");


            if (zip.Length < 5)
            {
                throw new ArgumentOutOfRangeException(nameof(zip), $"{nameof(zip)} cannot be negative");
            }

            this.Zip = zip;
        }
    }
}
