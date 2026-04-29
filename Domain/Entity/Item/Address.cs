using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item
{
    public class Address : Base
    {
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        internal Address(double latitude, double longitude, string street, string city, string postalCode, string country) : base()
        {
            Latitude = latitude;
            Longitude = longitude;
            Street = street ?? throw new ArgumentNullException(nameof(street));
            City = city ?? throw new ArgumentNullException(nameof(city));
            PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
            Country = country ?? throw new ArgumentNullException(nameof(country));
        }
        public void UpdateAddress(double latitude, double longitude, string street, string city, string postalCode, string country)
        {
            Latitude = latitude;
            Longitude = longitude;
            Street = street ?? throw new ArgumentNullException(nameof(street));
            City = city ?? throw new ArgumentNullException(nameof(city));
            PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
            Country = country ?? throw new ArgumentNullException(nameof(country));
        }
    }
}
