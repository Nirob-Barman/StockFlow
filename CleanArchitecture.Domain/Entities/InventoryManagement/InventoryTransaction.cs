using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.ProductManagement;
using CleanArchitecture.Domain.Entities.WarehouseManagement;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities.InventoryManagement
{
    public sealed class InventoryTransaction : BaseEntity
    {
        public Guid ProductId { get; private set; }

        public Guid WarehouseId { get; private set; }

        public InventoryTransactionType Type { get; private set; }

        public StockMovementDirection Direction { get; private set; }

        public decimal Quantity { get; private set; }

        public decimal SignedQuantity =>
            Direction == StockMovementDirection.In
                ? Quantity
                : -Quantity;

        public string ReferenceNumber { get; private set; }
            = string.Empty;

        public Guid? ReferenceId { get; private set; }

        public DateTime TransactionDate { get; private set; }

        public Guid? CreatedBy { get; private set; }

        public Product Product { get; private set; } = null!;

        public Warehouse Warehouse { get; private set; } = null!;

        private InventoryTransaction()
        {
        }

        public InventoryTransaction(
            Guid productId,
            Guid warehouseId,
            InventoryTransactionType type,
            StockMovementDirection direction,
            decimal quantity,
            string referenceNumber,
            Guid? referenceId = null,
            Guid? createdBy = null)
        {
            if (productId == Guid.Empty)
                throw new DomainException(
                    "Product is required.");

            if (warehouseId == Guid.Empty)
                throw new DomainException(
                    "Warehouse is required.");

            if (quantity <= 0)
                throw new DomainException(
                    "Transaction quantity must be positive.");

            if (string.IsNullOrWhiteSpace(referenceNumber))
                throw new DomainException(
                    "Reference number is required.");

            ProductId = productId;
            WarehouseId = warehouseId;
            Type = type;
            Direction = direction;
            Quantity = quantity;
            ReferenceNumber = referenceNumber;
            ReferenceId = referenceId;
            CreatedBy = createdBy;
            TransactionDate = DateTime.UtcNow;
        }
    }
}
