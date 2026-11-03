using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.InventoryManagement;
using CleanArchitecture.Domain.Entities.PurchaseManagement;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities.WarehouseManagement
{
    public sealed class Warehouse : AuditableEntity
    {
        public string WarehouseCode { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public Address Address { get; private set; } = null!;

        public string? PhoneNumber { get; private set; }

        public string? Email { get; private set; }

        public bool IsActive { get; private set; }

        private readonly List<Stock> _stocks = []; // one warehouse containing stock records for multiple products.

        private readonly List<GoodsReceipt> _goodsReceipts = [];

        private readonly List<InventoryTransaction> _inventoryTransactions = [];

        private readonly List<StockTransfer> _sourceTransfers = [];

        private readonly List<StockTransfer> _destinationTransfers = [];

        public IReadOnlyCollection<Stock> Stocks =>
            _stocks.AsReadOnly();

        public IReadOnlyCollection<GoodsReceipt> GoodsReceipts =>
            _goodsReceipts.AsReadOnly();

        public IReadOnlyCollection<InventoryTransaction> InventoryTransactions =>
            _inventoryTransactions.AsReadOnly();

        public IReadOnlyCollection<StockTransfer> SourceTransfers =>
            _sourceTransfers.AsReadOnly();

        public IReadOnlyCollection<StockTransfer> DestinationTransfers =>
            _destinationTransfers.AsReadOnly();

        private Warehouse()
        {
        }

        public Warehouse(
            string warehouseCode,
            string name,
            Address address,
            string? phoneNumber = null,
            string? email = null)
        {
            SetWarehouseCode(warehouseCode);
            SetName(name);
            SetAddress(address);
            SetPhoneNumber(phoneNumber);
            SetEmail(email);

            IsActive = true;
        }

        public void UpdateInformation(
            string name,
            Address address,
            string? phoneNumber,
            string? email)
        {
            EnsureActive();

            SetName(name);
            SetAddress(address);
            SetPhoneNumber(phoneNumber);
            SetEmail(email);
        }

        public void ChangeAddress(Address address)
        {
            EnsureActive();
            SetAddress(address);
        }

        public void UpdateContactInformation(
            string? phoneNumber,
            string? email)
        {
            EnsureActive();

            SetPhoneNumber(phoneNumber);
            SetEmail(email);
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException(
                    "Warehouse is already active.");

            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException(
                    "Warehouse is already inactive.");

            IsActive = false;
        }

        private void SetWarehouseCode(string warehouseCode)
        {
            if (string.IsNullOrWhiteSpace(warehouseCode))
                throw new DomainException(
                    "Warehouse code is required.");

            warehouseCode = warehouseCode
                .Trim()
                .ToUpperInvariant();

            if (warehouseCode.Length > 50)
                throw new DomainException(
                    "Warehouse code cannot exceed 50 characters.");

            WarehouseCode = warehouseCode;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "Warehouse name is required.");

            name = name.Trim();

            if (name.Length > 200)
                throw new DomainException(
                    "Warehouse name cannot exceed 200 characters.");

            Name = name;
        }

        private void SetAddress(Address address)
        {
            ArgumentNullException.ThrowIfNull(address);

            Address = address;
        }

        private void SetPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                PhoneNumber = null;
                return;
            }

            phoneNumber = phoneNumber.Trim();

            if (phoneNumber.Length > 30)
                throw new DomainException(
                    "Phone number cannot exceed 30 characters.");

            PhoneNumber = phoneNumber;
        }

        private void SetEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                Email = null;
                return;
            }

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
                    "Invalid warehouse email address.");
            }

            Email = email;
        }

        private void EnsureActive()
        {
            if (!IsActive)
                throw new DomainException(
                    "Inactive warehouse cannot be modified.");
        }
    }
}