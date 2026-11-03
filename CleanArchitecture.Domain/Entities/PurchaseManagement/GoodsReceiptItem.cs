using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.ProductManagement;

namespace CleanArchitecture.Domain.Entities.PurchaseManagement
{
    public sealed class GoodsReceiptItem : BaseEntity
    {
        public Guid GoodsReceiptId { get; private set; }

        public Guid PurchaseOrderItemId { get; private set; }

        public Guid ProductId { get; private set; }

        public decimal ReceivedQuantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal TotalAmount =>
            ReceivedQuantity * UnitPrice;

        public GoodsReceipt GoodsReceipt { get; private set; } = null!;

        public PurchaseOrderItem PurchaseOrderItem { get; private set; } = null!;

        public Product Product { get; private set; } = null!;

        private GoodsReceiptItem()
        {
        }

        // We do not want external application code to create goods receipt items independently.
        // This constructor is internal to restrict instantiation to within the assembly.
        // GoodsReceipt should control the creation of its child entities.
        internal GoodsReceiptItem(
            Guid goodsReceiptId,
            Guid purchaseOrderItemId,
            Guid productId,
            decimal receivedQuantity,
            decimal unitPrice)
        {
            if (goodsReceiptId == Guid.Empty)
                throw new DomainException(
                    "Goods receipt is required.");

            if (purchaseOrderItemId == Guid.Empty)
                throw new DomainException(
                    "Purchase order item is required.");

            if (productId == Guid.Empty)
                throw new DomainException(
                    "Product is required.");

            if (receivedQuantity <= 0)
                throw new DomainException(
                    "Received quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new DomainException(
                    "Unit price cannot be negative.");

            GoodsReceiptId = goodsReceiptId;
            PurchaseOrderItemId = purchaseOrderItemId;
            ProductId = productId;
            ReceivedQuantity = receivedQuantity;
            UnitPrice = unitPrice;
        }
    }
}
