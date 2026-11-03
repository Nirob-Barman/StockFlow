namespace CleanArchitecture.Domain.Enums
{
    public enum InventoryTransactionType
    {
        OpeningStock = 1,
        PurchaseReceipt = 2,
        SalesIssue = 3,
        StockAdjustment = 4,
        StockTransferOut = 5,
        StockTransferIn = 6,
        CustomerReturn = 7,
        SupplierReturn = 8,
        StockReversal = 9
    }
}
