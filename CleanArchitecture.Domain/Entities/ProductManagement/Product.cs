using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities.WarehouseManagement;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities.ProductManagement
{
    public sealed class Product : AuditableEntity
    {
        public string ProductCode { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public string? Description { get; private set; }

        public Guid CategoryId { get; private set; }

        public Guid UnitOfMeasureId { get; private set; }

        public string? SKU { get; private set; }

        public string? Barcode { get; private set; }

        public decimal PurchasePrice { get; private set; }

        public decimal SellingPrice { get; private set; }        

        public ProductStatus Status { get; private set; }

        public ProductCategory Category { get; private set; } = null!;

        public UnitOfMeasure UnitOfMeasure { get; private set; } = null!;

        private readonly List<Stock> _stocks = []; // one product having stock records in multiple warehouses        

        public IReadOnlyCollection<Stock> Stocks =>
            _stocks.AsReadOnly();

        private Product()
        {
        }

        public Product(
            string productCode,
            string name,
            Guid categoryId,
            Guid unitOfMeasureId,
            decimal purchasePrice,
            decimal sellingPrice,
            string? description = null,
            string? sku = null,
            string? barcode = null)
        {
            SetProductCode(productCode);
            SetName(name);
            SetCategory(categoryId);
            SetUnitOfMeasure(unitOfMeasureId);
            SetPrices(purchasePrice, sellingPrice);

            Description = description?.Trim();
            SKU = NormalizeOptionalCode(sku);
            Barcode = NormalizeOptionalCode(barcode);

            Status = ProductStatus.Active;
        }

        // Update product information
        public void UpdateInformation(
            string name,
            string? description,
            Guid categoryId,
            Guid unitOfMeasureId)
        {
            EnsureNotDiscontinued();

            SetName(name);
            SetCategory(categoryId);
            SetUnitOfMeasure(unitOfMeasureId);

            Description = description?.Trim();
        }

        // Update product prices
        public void UpdatePrices(
            decimal purchasePrice,
            decimal sellingPrice)
        {
            EnsureNotDiscontinued();

            SetPrices(purchasePrice, sellingPrice);
        }

        // Update SKU and Barcode
        public void UpdateIdentification(
            string? sku,
            string? barcode)
        {
            EnsureNotDiscontinued();

            SKU = NormalizeOptionalCode(sku);
            Barcode = NormalizeOptionalCode(barcode);
        }

        // Activate product
        public void Activate()
        {
            //if (Status == ProductStatus.Discontinued)
            //    throw new DomainException(
            //        "Discontinued products cannot be activated.");

            if (Status == ProductStatus.Active)
                throw new DomainException(
                    "Product is already active.");

            Status = ProductStatus.Active;
        }

        // Deactivate product
        public void Deactivate()
        {
            if (Status != ProductStatus.Active)
                throw new DomainException(
                    "Only active products can be deactivated.");

            Status = ProductStatus.Inactive;
        }

        // Discontinue product
        public void Discontinue()
        {
            if (Status == ProductStatus.Discontinued)
                throw new DomainException(
                    "Product is already discontinued.");

            Status = ProductStatus.Discontinued;
        }

        private void SetProductCode(string productCode)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                throw new DomainException(
                    "Product code is required.");

            productCode = productCode.Trim().ToUpperInvariant();

            if (productCode.Length > 50)
                throw new DomainException(
                    "Product code cannot exceed 50 characters.");

            ProductCode = productCode;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "Product name is required.");

            name = name.Trim();

            if (name.Length > 200)
                throw new DomainException(
                    "Product name cannot exceed 200 characters.");

            Name = name;
        }

        private void SetCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new DomainException(
                    "Product category is required.");

            CategoryId = categoryId;
        }

        private void SetUnitOfMeasure(Guid unitOfMeasureId)
        {
            if (unitOfMeasureId == Guid.Empty)
                throw new DomainException(
                    "Unit of measure is required.");

            UnitOfMeasureId = unitOfMeasureId;
        }

        private void SetPrices(
            decimal purchasePrice,
            decimal sellingPrice)
        {
            if (purchasePrice < 0)
                throw new DomainException(
                    "Purchase price cannot be negative.");

            if (sellingPrice < 0)
                throw new DomainException(
                    "Selling price cannot be negative.");

            PurchasePrice = purchasePrice;
            SellingPrice = sellingPrice;
        }

        private string? NormalizeOptionalCode(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim().ToUpperInvariant();

            if (value.Length > 100)
                throw new DomainException(
                    "Identification code cannot exceed 100 characters.");

            return value;
        }

        private void EnsureNotDiscontinued()
        {
            if (Status == ProductStatus.Discontinued)
                throw new DomainException(
                    "Discontinued products cannot be modified.");
        }
    }
}
