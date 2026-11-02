using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities.ProductManagement
{
    public sealed class ProductCategory : AuditableEntity
    {
        public string Name { get; private set; } = string.Empty;

        public string? Description { get; private set; }

        public bool IsActive { get; private set; }

        private readonly List<Product> _products = new();

        public IReadOnlyCollection<Product> Products =>
            _products.AsReadOnly();

        // EF Core constructor
        private ProductCategory()
        {
        }

        // Create category
        public ProductCategory(
            string name,
            string? description = null)
        {
            SetName(name);

            Description = description?.Trim();

            IsActive = true;
        }

        // Update category information
        public void UpdateInformation(
            string name,
            string? description)
        {
            SetName(name);

            Description = description?.Trim();

            SetUpdatedByIfAvailable();
        }

        // Rename category
        public void Rename(string name)
        {
            SetName(name);
        }

        // Activate category
        public void Activate()
        {
            if (IsActive)
                throw new DomainException(
                    "Category is already active.");

            IsActive = true;
        }

        // Deactivate category
        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException(
                    "Category is already inactive.");

            IsActive = false;
        }

        // Internal validation
        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "Category name is required.");

            name = name.Trim();

            if (name.Length > 100)
                throw new DomainException(
                    "Category name cannot exceed 100 characters.");

            Name = name;
        }

        private void SetUpdatedByIfAvailable()
        {
            // Audit identity is supplied by the application layer.
            // The persistence layer can also set update timestamps.
        }
    }
}
