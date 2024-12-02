using System;
using System.Diagnostics;

namespace DBAccess.Model
{
    /// <summary>
    ///     Represents a mailing address, including street, city, state, country, and zip code.
    /// </summary>
    public class MailingAddress
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the street address of the mailing address.
        /// </summary>

        public string StreetAddress { get; set; }

        /// <summary>
        ///     Gets or sets the zip code of the mailing address.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if the zip code is less than 5 characters.
        /// </exception>

        public string Zip { get; set; }

        /// <summary>
        ///     Gets or sets the city of the mailing address.
        /// </summary>

        public string City { get; set; }

        /// <summary>
        ///     Gets or sets the state of the mailing address.
        /// </summary>

        public string State { get; set; }

        /// <summary>
        ///     Gets or sets the country of the mailing address.
        /// </summary>

        public string Country { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MailingAddress" /> class.
        /// </summary>
        /// <param name="streetAddress">The street address of the mailing address.</param>
        /// <param name="zip">The zip code of the mailing address.</param>
        /// <param name="city">The city of the mailing address.</param>
        /// <param name="state">The state of the mailing address.</param>
        /// <param name="country">The country of the mailing address.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if any of <paramref name="streetAddress" />, <paramref name="zip" />, <paramref name="city" />,
        ///     <paramref name="state" />, or <paramref name="country" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="zip" /> is less than 5 characters long.
        /// </exception>
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

        #endregion
    }
}