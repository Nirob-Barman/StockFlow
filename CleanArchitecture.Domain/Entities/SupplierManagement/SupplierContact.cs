using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities.SupplierManagement
{
    public sealed class SupplierContact : BaseEntity
    {
        public Guid SupplierId { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string? Designation { get; private set; }

        public string Email { get; private set; } = string.Empty;

        public string PhoneNumber { get; private set; } = string.Empty;

        public bool IsPrimary { get; private set; }

        public bool IsActive { get; private set; }

        public Supplier Supplier { get; private set; } = null!;

        private SupplierContact()
        {
        }

        internal SupplierContact(
            Guid supplierId,
            string name,
            string email,
            string phoneNumber,
            string? designation = null,
            bool isPrimary = false)
        {
            if (supplierId == Guid.Empty)
                throw new DomainException(
                    "Supplier is required.");

            SupplierId = supplierId;

            SetName(name);
            SetEmail(email);
            SetPhoneNumber(phoneNumber);
            SetDesignation(designation);

            IsPrimary = isPrimary;
            IsActive = true;
        }

        internal void UpdateInformation(
            string name,
            string email,
            string phoneNumber,
            string? designation)
        {
            EnsureActive();

            SetName(name);
            SetEmail(email);
            SetPhoneNumber(phoneNumber);
            SetDesignation(designation);
        }

        internal void SetPrimary(bool isPrimary)
        {
            EnsureActive();

            IsPrimary = isPrimary;
        }

        internal void Deactivate()
        {
            EnsureActive();

            IsActive = false;
            IsPrimary = false;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "Contact name is required.");

            name = name.Trim();

            if (name.Length > 150)
                throw new DomainException(
                    "Contact name cannot exceed 150 characters.");

            Name = name;
        }

        private void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException(
                    "Contact email is required.");

            email = email.Trim();

            if (email.Length > 254 ||
                !System.Net.Mail.MailAddress.TryCreate(
                    email, out var parsedEmail) ||
                !string.Equals(
                    parsedEmail.Address,
                    email,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException(
                    "Invalid contact email address.");
            }

            Email = email;
        }

        private void SetPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException(
                    "Contact phone number is required.");

            phoneNumber = phoneNumber.Trim();

            if (phoneNumber.Length > 30)
                throw new DomainException(
                    "Phone number cannot exceed 30 characters.");

            PhoneNumber = phoneNumber;
        }

        private void SetDesignation(string? designation)
        {
            if (string.IsNullOrWhiteSpace(designation))
            {
                Designation = null;
                return;
            }

            designation = designation.Trim();

            if (designation.Length > 100)
                throw new DomainException(
                    "Designation cannot exceed 100 characters.");

            Designation = designation;
        }

        private void EnsureActive()
        {
            if (!IsActive)
                throw new DomainException(
                    "Inactive supplier contacts cannot be modified.");
        }
    }
}
