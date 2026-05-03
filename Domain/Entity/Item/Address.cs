using Domain.Entity.Mapping;
using Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity.Item
{
    public class Address
    {
        //ValueObject
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public Guid CompanyId { get; internal set; }
        internal Address(double latitude, double longitude, string street, string city, string postalCode, string country, Guid companyId) : base()
        {
            Guard.AgainstNullOrEmpty(street, nameof(street));
            Guard.AgainstNullOrEmpty(city, nameof(city));
            Guard.AgainstNullOrEmpty(postalCode, nameof(postalCode));
            Guard.AgainstNullOrEmpty(country, nameof(country));
            Latitude = latitude;
            Longitude = longitude;
            Street = street;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }
        public void UpdateAddress(double latitude, double longitude, string street, string city, string postalCode, string country)
        {
            Guard.AgainstNullOrEmpty(street, nameof(street));
            Guard.AgainstNullOrEmpty(city, nameof(city));
            Guard.AgainstNullOrEmpty(postalCode, nameof(postalCode));
            Guard.AgainstNullOrEmpty(country, nameof(country));
            Latitude = latitude;
            Longitude = longitude;
            Street = street;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }
    }
}
