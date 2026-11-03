using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.SupplierManagement;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities.PurchaseManagement
{
    public sealed class PurchaseOrder : AuditableEntity
    {       
        public string PurchaseOrderNumber { get; private set; }
            = string.Empty;

        public Guid SupplierId { get; private set; }

        public DateTime OrderDate { get; private set; }

        public DateTime? ExpectedDeliveryDate { get; private set; }

        public PurchaseOrderStatus Status { get; private set; }

        public Supplier Supplier { get; private set; } = null!;

        private readonly List<PurchaseOrderItem> _items = [];

        private readonly List<GoodsReceipt> _goodsReceipts = [];        

        public IReadOnlyCollection<PurchaseOrderItem> Items =>
            _items.AsReadOnly();

        public IReadOnlyCollection<GoodsReceipt> GoodsReceipts =>
            _goodsReceipts.AsReadOnly();

        public decimal TotalAmount =>
            _items.Sum(x => x.TotalAmount);

        private PurchaseOrder()
        {
        }

        public PurchaseOrder(
            string purchaseOrderNumber,
            Guid supplierId,
            DateTime? expectedDeliveryDate = null)
        {
            if (string.IsNullOrWhiteSpace(purchaseOrderNumber))
                throw new DomainException(
                    "Purchase order number is required.");

            if (supplierId == Guid.Empty)
                throw new DomainException(
                    "Supplier is required.");

            if (expectedDeliveryDate.HasValue &&
                expectedDeliveryDate.Value.Date < DateTime.UtcNow.Date)
                throw new DomainException(
                    "Expected delivery date cannot be in the past.");

            PurchaseOrderNumber = purchaseOrderNumber.Trim();
            SupplierId = supplierId;
            OrderDate = DateTime.UtcNow;
            ExpectedDeliveryDate = expectedDeliveryDate;

            Status = PurchaseOrderStatus.Draft;
        }

        // Add product to purchase order
        public void AddItem(
            Guid productId,
            decimal quantity,
            decimal unitPrice)
        {
            EnsureDraft();

            if (productId == Guid.Empty)
                throw new DomainException(
                    "Product is required.");

            if (_items.Any(x => x.ProductId == productId))
                throw new DomainException(
                    "Product already exists in this purchase order.");

            var item = new PurchaseOrderItem(
                Id,
                productId,
                quantity,
                unitPrice);

            _items.Add(item);
        }

        // Update existing purchase order item
        public void UpdateItem(
            Guid itemId,
            decimal quantity,
            decimal unitPrice)
        {
            EnsureDraft();

            var item = _items.FirstOrDefault(
                x => x.Id == itemId);

            if (item is null)
                throw new DomainException(
                    "Purchase order item not found.");

            item.Update(quantity, unitPrice);
        }

        // Remove purchase order item
        public void RemoveItem(Guid itemId)
        {
            EnsureDraft();

            var item = _items.FirstOrDefault(
                x => x.Id == itemId);

            if (item is null)
                throw new DomainException(
                    "Purchase order item not found.");

            _items.Remove(item);
        }

        // Submit purchase order
        public void Submit()
        {
            EnsureDraft();

            if (!_items.Any())
                throw new DomainException(
                    "Purchase order must contain at least one item.");

            Status = PurchaseOrderStatus.Submitted;
        }

        // Approve purchase order
        public void Approve()
        {
            if (Status != PurchaseOrderStatus.Submitted)
                throw new DomainException(
                    "Only submitted orders can be approved.");

            Status = PurchaseOrderStatus.Approved;
        }

        // Record receiving progress
        public void RecordReceivedQuantity(
            Guid productId,
            decimal receivedQuantity,
            decimal previouslyReceivedQuantity)
        {
            if (Status != PurchaseOrderStatus.Approved &&
                Status != PurchaseOrderStatus.PartiallyReceived)
            {
                throw new DomainException(
                    "Purchase order is not available for receiving.");
            }

            if (receivedQuantity <= 0 ||
                previouslyReceivedQuantity < 0)
                throw new DomainException(
                    "Invalid receiving quantity.");

            var item = _items.FirstOrDefault(
                x => x.ProductId == productId);

            if (item is null)
                throw new DomainException(
                    "Product does not belong to this purchase order.");

            if (previouslyReceivedQuantity + receivedQuantity >
                item.Quantity)
            {
                throw new DomainException(
                    "Received quantity exceeds ordered quantity.");
            }

            var allReceived = _items.All(x =>
            {
                if (x.ProductId == productId)
                {
                    return previouslyReceivedQuantity +
                           receivedQuantity >= x.Quantity;
                }

                // In a complete implementation, use persisted
                // receiving totals for every item.
                return false;
            });

            Status = allReceived
                ? PurchaseOrderStatus.FullyReceived
                : PurchaseOrderStatus.PartiallyReceived;
        }

        // Cancel purchase order
        public void Cancel()
        {
            if (Status == PurchaseOrderStatus.FullyReceived ||
                Status == PurchaseOrderStatus.Cancelled)
            {
                throw new DomainException(
                    "Purchase order cannot be cancelled.");
            }

            Status = PurchaseOrderStatus.Cancelled;
        }

        private void EnsureDraft()
        {
            if (Status != PurchaseOrderStatus.Draft)
                throw new DomainException(
                    "Only draft purchase orders can be modified.");
        }
    }
}
