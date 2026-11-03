namespace CleanArchitecture.Domain.Enums
{
    public enum StockTransferStatus
    {
        Draft = 1,
        Requested = 2,
        Approved = 3,
        InTransit = 4,
        PartiallyReceived = 5,
        Completed = 6,
        Rejected = 7,
        Cancelled = 8
    }
}
