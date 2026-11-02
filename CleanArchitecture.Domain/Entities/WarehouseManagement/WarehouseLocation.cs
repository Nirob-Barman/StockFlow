using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities.WarehouseManagement
{
    public sealed class WarehouseLocation : AuditableEntity
    {
        private readonly List<WarehouseLocation> _children = [];

        public Guid WarehouseId { get; private set; }

        public Guid? ParentLocationId { get; private set; }

        public string LocationCode { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public LocationType Type { get; private set; }

        public decimal? Capacity { get; private set; }

        public bool IsActive { get; private set; }

        public IReadOnlyCollection<WarehouseLocation> Children =>
            _children.AsReadOnly();

        private WarehouseLocation()
        {
        }

        public WarehouseLocation(
            Guid warehouseId,
            Guid? parentLocationId,
            string locationCode,
            string name,
            LocationType type,
            decimal? capacity = null)
        {
            if (warehouseId == Guid.Empty)
                throw new ArgumentException("Warehouse is required.");

            if (parentLocationId == Id)
                throw new ArgumentException(
                    "A location cannot be its own parent.");

            if (string.IsNullOrWhiteSpace(locationCode))
                throw new ArgumentException("Location code is required.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Location name is required.");

            if (capacity <= 0)
                throw new ArgumentException(
                    "Capacity must be greater than zero.");

            WarehouseId = warehouseId;
            ParentLocationId = parentLocationId;
            LocationCode = locationCode.Trim();
            Name = name.Trim();
            Type = type;
            Capacity = capacity;
            IsActive = true;
        }

        public void UpdateInformation(
            string name,
            decimal? capacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Location name is required.");

            if (capacity <= 0)
                throw new ArgumentException(
                    "Capacity must be greater than zero.");

            Name = name.Trim();
            Capacity = capacity;
        }

        public void Deactivate()
        {
            if (_children.Any(x => x.IsActive))
                throw new InvalidOperationException(
                    "Deactivate child locations first.");

            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}
