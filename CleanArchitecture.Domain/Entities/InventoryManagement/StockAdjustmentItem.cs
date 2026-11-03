using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities.InventoryManagement
{
    public sealed class StockAdjustmentItem : BaseEntity
    {
        public Guid StockAdjustmentId { get; private set; }

        public Guid ProductId { get; private set; }

        public decimal SystemQuantity { get; private set; }

        public decimal PhysicalQuantity { get; private set; }

        public decimal AdjustmentQuantity =>
            PhysicalQuantity - SystemQuantity;

        private StockAdjustmentItem()
        {
        }

        internal StockAdjustmentItem(
            Guid adjustmentId,
            Guid productId,
            decimal systemQuantity,
            decimal physicalQuantity)
        {
            if (adjustmentId == Guid.Empty)
                throw new DomainException(
                    "Adjustment is required.");

            if (productId == Guid.Empty)
                throw new DomainException(
                    "Product is required.");

            if (systemQuantity < 0 || physicalQuantity < 0)
                throw new DomainException(
                    "Quantities cannot be negative.");

            StockAdjustmentId = adjustmentId;
            ProductId = productId;
            SystemQuantity = systemQuantity;
            PhysicalQuantity = physicalQuantity;
        }
    }
}
