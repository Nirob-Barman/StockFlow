namespace CleanArchitecture.Domain.Enums
{
    public enum PurchaseOrderStatus
    {
        Draft = 1,
        Submitted = 2,
        Approved = 3,
        PartiallyReceived = 4,
        FullyReceived = 5,
        Cancelled = 6
    }
}
