using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.WarehouseManagement;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities.PurchaseManagement
{
    public sealed class GoodsReceipt : AuditableEntity
    {
        private readonly List<GoodsReceiptItem> _items = new();

        public string ReceiptNumber { get; private set; }
            = string.Empty;

        public Guid PurchaseOrderId { get; private set; }

        public Guid WarehouseId { get; private set; }

        public DateTime? ReceivedDate { get; private set; }

        public GoodsReceiptStatus Status { get; private set; }

        public PurchaseOrder PurchaseOrder { get; private set; } = null!;

        public Warehouse Warehouse { get; private set; } = null!;

        public IReadOnlyCollection<GoodsReceiptItem> Items =>
            _items.AsReadOnly();

        public decimal TotalAmount =>
            _items.Sum(x => x.TotalAmount);

        private GoodsReceipt()
        {
        }

        public GoodsReceipt(
            string receiptNumber,
            Guid purchaseOrderId,
            Guid warehouseId)
        {
            if (string.IsNullOrWhiteSpace(receiptNumber))
                throw new DomainException(
                    "Receipt number is required.");

            if (purchaseOrderId == Guid.Empty)
                throw new DomainException(
                    "Purchase order is required.");

            if (warehouseId == Guid.Empty)
                throw new DomainException(
                    "Warehouse is required.");

            ReceiptNumber = receiptNumber.Trim();
            PurchaseOrderId = purchaseOrderId;
            WarehouseId = warehouseId;

            Status = GoodsReceiptStatus.Draft;
        }

        public void AddItem(
            Guid purchaseOrderItemId,
            Guid productId,
            decimal receivedQuantity,
            decimal unitPrice)
        {
            EnsureDraft();

            if (_items.Any(x =>
                x.PurchaseOrderItemId == purchaseOrderItemId))
            {
                throw new DomainException(
                    "Purchase order item already exists in this receipt.");
            }

            var item = new GoodsReceiptItem(
                Id,
                purchaseOrderItemId,
                productId,
                receivedQuantity,
                unitPrice);

            _items.Add(item);
        }

        public void RemoveItem(Guid itemId)
        {
            EnsureDraft();

            var item = _items.FirstOrDefault(
                x => x.Id == itemId);

            if (item is null)
                throw new DomainException(
                    "Goods receipt item not found.");

            _items.Remove(item);
        }

        public void Post()
        {
            EnsureDraft();

            if (!_items.Any())
                throw new DomainException(
                    "Goods receipt must contain at least one item.");

            Status = GoodsReceiptStatus.Posted;
            ReceivedDate = DateTime.UtcNow;
        }

        public void Cancel()
        {
            EnsureDraft();

            Status = GoodsReceiptStatus.Cancelled;
        }

        private void EnsureDraft()
        {
            if (Status != GoodsReceiptStatus.Draft)
                throw new DomainException(
                    "Only draft goods receipts can be modified.");
        }
    }
}
