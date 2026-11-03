using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.ProductManagement;
using CleanArchitecture.Domain.Entities.WarehouseManagement;

namespace CleanArchitecture.Domain.Entities.InventoryManagement
{
    public sealed class StockTransferItem : BaseEntity
    {
        public Guid StockTransferId { get; private set; }

        public Guid ProductId { get; private set; }

        public decimal Quantity { get; private set; }

        public decimal ReceivedQuantity { get; private set; }

        public decimal RemainingQuantity =>
            Quantity - ReceivedQuantity;

        public StockTransfer StockTransfer { get; private set; } = null!;

        public Product Product { get; private set; } = null!;

        public Warehouse SourceWarehouse { get; private set; } = null!;

        public Warehouse DestinationWarehouse { get; private set; } = null!;

        private StockTransferItem()
        {
        }

        internal StockTransferItem(
            Guid transferId,
            Guid productId,
            decimal quantity)
        {
            if (transferId == Guid.Empty)
                throw new DomainException(
                    "Transfer is required.");

            if (productId == Guid.Empty)
                throw new DomainException(
                    "Product is required.");

            if (quantity <= 0)
                throw new DomainException(
                    "Transfer quantity must be positive.");

            StockTransferId = transferId;
            ProductId = productId;
            Quantity = quantity;
            ReceivedQuantity = 0;
        }

        internal void Receive(decimal quantity)
        {
            if (quantity <= 0)
                throw new DomainException(
                    "Received quantity must be positive.");

            if (quantity > RemainingQuantity)
                throw new DomainException(
                    "Received quantity exceeds remaining quantity.");

            ReceivedQuantity += quantity;
        }
    }
}
