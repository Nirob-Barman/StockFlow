using CleanArchitecture.Domain.Entities.InventoryManagement;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Factories
{
    public static class InventoryTransactionFactory
    {
        public static InventoryTransaction CreatePurchaseReceipt(
            Guid productId,
            Guid warehouseId,
            decimal quantity,
            string referenceNumber,
            Guid referenceId,
            Guid? createdBy = null)
        {
            return Create(
                productId,
                warehouseId,
                InventoryTransactionType.PurchaseReceipt,
                StockMovementDirection.In,
                quantity,
                referenceNumber,
                referenceId,
                createdBy);
        }

        public static InventoryTransaction CreateSalesIssue(
            Guid productId,
            Guid warehouseId,
            decimal quantity,
            string referenceNumber,
            Guid referenceId,
            Guid? createdBy = null)
        {
            return Create(
                productId,
                warehouseId,
                InventoryTransactionType.SalesIssue,
                StockMovementDirection.Out,
                quantity,
                referenceNumber,
                referenceId,
                createdBy);
        }

        public static InventoryTransaction CreateTransferOut(
            Guid productId,
            Guid warehouseId,
            decimal quantity,
            string referenceNumber,
            Guid referenceId,
            Guid? createdBy = null)
        {
            return Create(
                productId,
                warehouseId,
                InventoryTransactionType.StockTransferOut,
                StockMovementDirection.Out,
                quantity,
                referenceNumber,
                referenceId,
                createdBy);
        }

        public static InventoryTransaction CreateTransferIn(
            Guid productId,
            Guid warehouseId,
            decimal quantity,
            string referenceNumber,
            Guid referenceId,
            Guid? createdBy = null)
        {
            return Create(
                productId,
                warehouseId,
                InventoryTransactionType.StockTransferIn,
                StockMovementDirection.In,
                quantity,
                referenceNumber,
                referenceId,
                createdBy);
        }

        public static InventoryTransaction CreateAdjustment(
            Guid productId,
            Guid warehouseId,
            decimal quantity,
            StockMovementDirection direction,
            string referenceNumber,
            Guid referenceId,
            Guid? createdBy = null)
        {
            return Create(
                productId,
                warehouseId,
                InventoryTransactionType.StockAdjustment,
                direction,
                quantity,
                referenceNumber,
                referenceId,
                createdBy);
        }

        private static InventoryTransaction Create(
            Guid productId,
            Guid warehouseId,
            InventoryTransactionType type,
            StockMovementDirection direction,
            decimal quantity,
            string referenceNumber,
            Guid referenceId,
            Guid? createdBy)
        {
            return new InventoryTransaction(
                productId,
                warehouseId,
                type,
                direction,
                quantity,
                referenceNumber,
                referenceId,
                createdBy);
        }
    }
}
