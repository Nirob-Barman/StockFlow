using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.ProductManagement;

namespace CleanArchitecture.Domain.Entities.PurchaseManagement
{
    public sealed class PurchaseOrderItem : BaseEntity
    {
        public Guid PurchaseOrderId { get; private set; }

        public Guid ProductId { get; private set; }

        public decimal Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal TotalAmount => Quantity * UnitPrice;

        public Product Product { get; private set; } = null!;

        public PurchaseOrder PurchaseOrder { get; private set; } = null!;

        private readonly List<GoodsReceiptItem> _goodsReceiptItems = [];

        public IReadOnlyCollection<GoodsReceiptItem> GoodsReceiptItems =>
            _goodsReceiptItems.AsReadOnly();

        private PurchaseOrderItem()
        {
        }

        // We do not want external application code to create purchase order items independently.
        internal PurchaseOrderItem(
            Guid purchaseOrderId,
            Guid productId,
            decimal quantity,
            decimal unitPrice)
        {
            if (purchaseOrderId == Guid.Empty)
                throw new DomainException(
                    "Purchase order is required.");

            if (productId == Guid.Empty)
                throw new DomainException(
                    "Product is required.");

            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new DomainException(
                    "Unit price cannot be negative.");

            PurchaseOrderId = purchaseOrderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        internal void Update(
            decimal quantity,
            decimal unitPrice)
        {
            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new DomainException(
                    "Unit price cannot be negative.");

            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
