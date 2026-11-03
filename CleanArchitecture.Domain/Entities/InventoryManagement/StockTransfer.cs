using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.WarehouseManagement;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities.InventoryManagement
{
    public sealed class StockTransfer : AuditableEntity
    {
        public string TransferNumber { get; private set; }
            = string.Empty;

        public Guid SourceWarehouseId { get; private set; }

        public Guid DestinationWarehouseId { get; private set; }

        public DateTime TransferDate { get; private set; }

        public Guid? RequestedBy { get; private set; }

        public Guid? ApprovedBy { get; private set; }

        public StockTransferStatus Status { get; private set; }

        public Warehouse SourceWarehouse { get; private set; } = null!;

        public Warehouse DestinationWarehouse { get; private set; } = null!;

        private readonly List<StockTransferItem> _items = [];

        public IReadOnlyCollection<StockTransferItem> Items =>
            _items.AsReadOnly();

        private StockTransfer()
        {
        }

        public StockTransfer(
            string transferNumber,
            Guid sourceWarehouseId,
            Guid destinationWarehouseId)
        {
            if (string.IsNullOrWhiteSpace(transferNumber))
                throw new DomainException(
                    "Transfer number is required.");

            if (sourceWarehouseId == Guid.Empty ||
                destinationWarehouseId == Guid.Empty)
                throw new DomainException(
                    "Both warehouses are required.");

            if (sourceWarehouseId == destinationWarehouseId)
                throw new DomainException(
                    "Source and destination cannot be the same.");

            TransferNumber = transferNumber.Trim();
            SourceWarehouseId = sourceWarehouseId;
            DestinationWarehouseId = destinationWarehouseId;

            TransferDate = DateTime.UtcNow;

            Status = StockTransferStatus.Draft;
        }

        public void AddItem(
            Guid productId,
            decimal quantity)
        {
            EnsureDraft();

            if (_items.Any(x => x.ProductId == productId))
                throw new DomainException(
                    "Product already exists in transfer.");

            _items.Add(
                new StockTransferItem(
                    Id,
                    productId,
                    quantity));
        }

        public void RemoveItem(Guid itemId)
        {
            EnsureDraft();

            var item = _items.FirstOrDefault(
                x => x.Id == itemId);

            if (item is null)
                throw new DomainException(
                    "Transfer item not found.");

            _items.Remove(item);
        }

        public void Request(Guid requestedBy)
        {
            EnsureDraft();

            if (!_items.Any())
                throw new DomainException(
                    "Transfer must contain items.");

            if (requestedBy == Guid.Empty)
                throw new DomainException(
                    "Requester is required.");

            RequestedBy = requestedBy;

            Status = StockTransferStatus.Requested;
        }

        public void Approve(Guid approvedBy)
        {
            if (Status != StockTransferStatus.Requested)
                throw new DomainException(
                    "Only requested transfers can be approved.");

            if (approvedBy == Guid.Empty)
                throw new DomainException(
                    "Approver is required.");

            ApprovedBy = approvedBy;

            Status = StockTransferStatus.Approved;
        }

        public void Reject()
        {
            if (Status != StockTransferStatus.Requested)
                throw new DomainException(
                    "Only requested transfers can be rejected.");

            Status = StockTransferStatus.Rejected;
        }

        public void Dispatch()
        {
            if (Status != StockTransferStatus.Approved)
                throw new DomainException(
                    "Only approved transfers can be dispatched.");

            Status = StockTransferStatus.InTransit;
        }

        public void ReceiveItem(
            Guid itemId,
            decimal receivedQuantity)
        {
            if (Status != StockTransferStatus.InTransit &&
                Status != StockTransferStatus.PartiallyReceived)
                throw new DomainException(
                    "Transfer is not available for receiving.");

            var item = _items.FirstOrDefault(
                x => x.Id == itemId);

            if (item is null)
                throw new DomainException(
                    "Transfer item not found.");

            item.Receive(receivedQuantity);

            Status = _items.All(
                x => x.RemainingQuantity == 0)
                ? StockTransferStatus.Completed
                : StockTransferStatus.PartiallyReceived;
        }

        public void Cancel()
        {
            if (Status != StockTransferStatus.Draft &&
                Status != StockTransferStatus.Requested)
                throw new DomainException(
                    "This transfer cannot be cancelled.");

            Status = StockTransferStatus.Cancelled;
        }

        private void EnsureDraft()
        {
            if (Status != StockTransferStatus.Draft)
                throw new DomainException(
                    "Only draft transfers can be modified.");
        }
    }
}
