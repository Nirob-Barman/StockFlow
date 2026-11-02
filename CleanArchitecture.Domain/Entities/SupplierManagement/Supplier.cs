using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities.SupplierManagement
{
    public sealed class Supplier : AuditableEntity
    {
        public string SupplierCode { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public Address Address { get; private set; } = null!;

        public SupplierStatus Status { get; private set; }

        private readonly List<SupplierContact> _contacts = [];

        public IReadOnlyCollection<SupplierContact> Contacts =>
            _contacts.AsReadOnly();

        private Supplier()
        {
        }

        public Supplier(
            string supplierCode,
            string name,
            Address address)
        {
            SetSupplierCode(supplierCode);
            SetName(name);
            SetAddress(address);

            Status = SupplierStatus.Active;
        }

        // Supplier Information

        public void UpdateInformation(
            string name,
            Address address)
        {
            EnsureNotBlocked();
            EnsureActive();

            SetName(name);
            SetAddress(address);
        }

        public void ChangeAddress(Address address)
        {
            EnsureNotBlocked();
            EnsureActive();

            SetAddress(address);
        }

        // Supplier Contact Management

        public Guid AddContact(
            string name,
            string email,
            string phoneNumber,
            string? designation = null,
            bool isPrimary = false)
        {
            EnsureActive();

            if (isPrimary && _contacts.Any(
                x => x.IsPrimary && x.IsActive))
            {
                throw new DomainException(
                    "Supplier already has a primary contact.");
            }

            var contact = new SupplierContact(
                Id,
                name,
                email,
                phoneNumber,
                designation,
                isPrimary);

            _contacts.Add(contact);

            return contact.Id;
        }

        public void UpdateContact(
            Guid contactId,
            string name,
            string email,
            string phoneNumber,
            string? designation = null)
        {
            EnsureActive();

            var contact = GetContact(contactId);

            contact.UpdateInformation(
                name,
                email,
                phoneNumber,
                designation);
        }

        public void SetPrimaryContact(Guid contactId)
        {
            EnsureActive();

            var contact = GetContact(contactId);

            foreach (var existingContact in _contacts)
            {
                if (existingContact.IsActive)
                    existingContact.SetPrimary(false);
            }

            contact.SetPrimary(true);
        }

        public void RemoveContact(Guid contactId)
        {
            EnsureActive();

            var contact = GetContact(contactId);

            contact.Deactivate();
        }

        // Supplier Status Management

        public void Activate()
        {
            if (Status == SupplierStatus.Active)
                throw new DomainException(
                    "Supplier is already active.");

            Status = SupplierStatus.Active;
        }

        public void Deactivate()
        {
            if (Status != SupplierStatus.Active)
                throw new DomainException(
                    "Only active suppliers can be deactivated.");

            Status = SupplierStatus.Inactive;
        }

        public void Block()
        {
            if (Status == SupplierStatus.Blocked)
                throw new DomainException(
                    "Supplier is already blocked.");

            Status = SupplierStatus.Blocked;
        }

        // Private Helper Methods

        private SupplierContact GetContact(Guid contactId)
        {
            var contact = _contacts.FirstOrDefault(
                x => x.Id == contactId && x.IsActive);

            return contact ?? throw new DomainException(
                "Supplier contact was not found.");
        }

        private void SetSupplierCode(string supplierCode)
        {
            if (string.IsNullOrWhiteSpace(supplierCode))
                throw new DomainException(
                    "Supplier code is required.");

            supplierCode = supplierCode.Trim().ToUpperInvariant();

            if (supplierCode.Length > 50)
                throw new DomainException(
                    "Supplier code cannot exceed 50 characters.");

            SupplierCode = supplierCode;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "Supplier name is required.");

            name = name.Trim();

            if (name.Length > 200)
                throw new DomainException(
                    "Supplier name cannot exceed 200 characters.");

            Name = name;
        }

        private void SetAddress(Address address)
        {
            ArgumentNullException.ThrowIfNull(address);

            Address = address;
        }

        private void EnsureNotBlocked()
        {
            if (Status == SupplierStatus.Blocked)
                throw new DomainException(
                    "Blocked suppliers cannot be modified.");
        }

        private void EnsureActive()
        {
            if (Status != SupplierStatus.Active)
                throw new DomainException(
                    "Only active suppliers can perform this operation.");
        }
    }
}
