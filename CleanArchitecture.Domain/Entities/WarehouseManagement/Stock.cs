using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.ProductManagement;

namespace CleanArchitecture.Domain.Entities.WarehouseManagement
{
    public sealed class Stock : AuditableEntity
    {
        public Guid ProductId { get; private set; }

        public Guid WarehouseId { get; private set; }

        public decimal QuantityOnHand { get; private set; }

        public decimal ReservedQuantity { get; private set; }

        public decimal AvailableQuantity =>
            QuantityOnHand - ReservedQuantity;

        public Product Product { get; private set; } = null!;

        public Warehouse Warehouse { get; private set; } = null!;

        private Stock()
        {
        }

        public Stock(
            Guid productId,
            Guid warehouseId)
        {
            if (productId == Guid.Empty)
                throw new DomainException(
                    "Product is required.");

            if (warehouseId == Guid.Empty)
                throw new DomainException(
                    "Warehouse is required.");

            ProductId = productId;
            WarehouseId = warehouseId;

            QuantityOnHand = 0;
            ReservedQuantity = 0;
        }

        // Receive physical stock
        public void ReceiveStock(decimal quantity)
        {
            EnsurePositiveQuantity(quantity);

            QuantityOnHand += quantity;
        }

        // Issue physical stock
        public void IssueStock(decimal quantity)
        {
            EnsurePositiveQuantity(quantity);

            if (AvailableQuantity < quantity)
                throw new DomainException(
                    "Insufficient available stock.");

            QuantityOnHand -= quantity;
        }

        // Reserve stock for an order
        public void ReserveStock(decimal quantity)
        {
            EnsurePositiveQuantity(quantity);

            if (AvailableQuantity < quantity)
                throw new DomainException(
                    "Insufficient available stock for reservation.");

            ReservedQuantity += quantity;
        }

        // Release previously reserved stock
        public void ReleaseReservation(decimal quantity)
        {
            EnsurePositiveQuantity(quantity);

            if (ReservedQuantity < quantity)
                throw new DomainException(
                    "Reservation quantity is insufficient.");

            ReservedQuantity -= quantity;
        }

        // Complete an order using reserved stock
        public void IssueReservedStock(decimal quantity)
        {
            EnsurePositiveQuantity(quantity);

            if (ReservedQuantity < quantity)
                throw new DomainException(
                    "Insufficient reserved stock.");

            ReservedQuantity -= quantity;
            QuantityOnHand -= quantity;
        }

        // Physical inventory correction
        public void AdjustStock(decimal newQuantity)
        {
            if (newQuantity < 0)
                throw new DomainException(
                    "Stock cannot be negative.");

            if (newQuantity < ReservedQuantity)
                throw new DomainException(
                    "Stock cannot be adjusted below reserved quantity.");

            var difference = newQuantity - QuantityOnHand;

            if (difference == 0)
                return;

            QuantityOnHand = newQuantity;
        }

        private static void EnsurePositiveQuantity(
            decimal quantity)
        {
            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero.");
        }
    }
}
