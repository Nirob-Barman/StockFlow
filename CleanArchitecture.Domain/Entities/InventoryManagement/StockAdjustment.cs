using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities.InventoryManagement
{
    public sealed class StockAdjustment : AuditableEntity
    {
        private readonly List<StockAdjustmentItem> _items = new();

        public string AdjustmentNumber { get; private set; }
            = string.Empty;

        public Guid WarehouseId { get; private set; }

        public AdjustmentReason Reason { get; private set; }

        public string? Description { get; private set; }

        public Guid? ApprovedBy { get; private set; }

        public DateTime AdjustmentDate { get; private set; }

        public StockAdjustmentStatus Status { get; private set; }

        public IReadOnlyCollection<StockAdjustmentItem> Items =>
            _items.AsReadOnly();

        private StockAdjustment()
        {
        }

        public StockAdjustment(
            string adjustmentNumber,
            Guid warehouseId,
            AdjustmentReason reason,
            string? description = null)
        {
            if (string.IsNullOrWhiteSpace(adjustmentNumber))
                throw new DomainException(
                    "Adjustment number is required.");

            if (warehouseId == Guid.Empty)
                throw new DomainException(
                    "Warehouse is required.");

            AdjustmentNumber = adjustmentNumber.Trim();
            WarehouseId = warehouseId;
            Reason = reason;
            Description = description;

            Status = StockAdjustmentStatus.Draft;
            AdjustmentDate = DateTime.UtcNow;
        }

        public void AddItem(
            Guid productId,
            decimal systemQuantity,
            decimal physicalQuantity)
        {
            EnsureDraft();

            if (_items.Any(x => x.ProductId == productId))
                throw new DomainException(
                    "Product already exists in this adjustment.");

            var item = new StockAdjustmentItem(
                Id,
                productId,
                systemQuantity,
                physicalQuantity);

            _items.Add(item);
        }

        public void RemoveItem(Guid itemId)
        {
            EnsureDraft();

            var item = _items.FirstOrDefault(
                x => x.Id == itemId);

            if (item is null)
                throw new DomainException(
                    "Adjustment item not found.");

            _items.Remove(item);
        }

        public void Submit()
        {
            EnsureDraft();

            if (!_items.Any())
                throw new DomainException(
                    "Adjustment must contain at least one item.");

            if (_items.All(x => x.AdjustmentQuantity == 0))
                throw new DomainException(
                    "Adjustment must contain at least one quantity difference.");

            Status = StockAdjustmentStatus.Submitted;
        }

        public void Approve(Guid approvedBy)
        {
            if (Status != StockAdjustmentStatus.Submitted)
                throw new DomainException(
                    "Only submitted adjustments can be approved.");

            if (approvedBy == Guid.Empty)
                throw new DomainException(
                    "Approver is required.");

            ApprovedBy = approvedBy;

            Status = StockAdjustmentStatus.Approved;
        }

        public void Reject()
        {
            if (Status != StockAdjustmentStatus.Submitted)
                throw new DomainException(
                    "Only submitted adjustments can be rejected.");

            Status = StockAdjustmentStatus.Rejected;
        }

        public void Post()
        {
            if (Status != StockAdjustmentStatus.Approved)
                throw new DomainException(
                    "Only approved adjustments can be posted.");

            Status = StockAdjustmentStatus.Posted;
        }

        public void Cancel()
        {
            if (Status != StockAdjustmentStatus.Draft)
                throw new DomainException(
                    "Only draft adjustments can be cancelled.");

            Status = StockAdjustmentStatus.Cancelled;
        }

        private void EnsureDraft()
        {
            if (Status != StockAdjustmentStatus.Draft)
                throw new DomainException(
                    "Only draft adjustments can be modified.");
        }
    }
}
