using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.ValueObjects
{
    public sealed record Address
    {
        public string Street { get; }

        public string City { get; }

        public string State { get; }

        public string Country { get; }

        public string PostalCode { get; }

        private Address()
        {
            Street = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Country = string.Empty;
            PostalCode = string.Empty;
        }

        public Address(
            string street,
            string city,
            string state,
            string country,
            string postalCode)
        {
            Street = Validate(street, "Street");
            City = Validate(city, "City");
            State = Validate(state, "State");
            Country = Validate(country, "Country");
            PostalCode = Validate(postalCode, "Postal code");
        }

        private static string Validate(
            string value,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(
                    $"{fieldName} is required.");

            return value.Trim();
        }
    }
}
