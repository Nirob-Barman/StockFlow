namespace CleanArchitecture.Domain.Enums
{
    public enum SupplierStatus
    {
        Active = 1,
        Inactive = 2, // Temporarily unavailable for new purchasing operations.
        Blocked = 3 // Supplier is prohibited from new purchasing operations.
    }
}
