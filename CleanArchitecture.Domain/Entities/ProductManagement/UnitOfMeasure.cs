using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities.ProductManagement
{
    public sealed class UnitOfMeasure : AuditableEntity
    {
        public string Code { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public UnitType UnitType { get; private set; }

        public bool IsActive { get; private set; }

        private readonly List<Product> _products = new();

        public IReadOnlyCollection<Product> Products =>
            _products.AsReadOnly();

        private UnitOfMeasure()
        {
        }

        public UnitOfMeasure(
            string code,
            string name,
            UnitType unitType)
        {
            SetCode(code);
            SetName(name);
            SetUnitType(unitType);

            IsActive = true;
        }

        public void UpdateInformation(
            string code,
            string name,
            UnitType unitType)
        {
            EnsureActive();

            SetCode(code);
            SetName(name);
            SetUnitType(unitType);
        }

        public void Rename(string name)
        {
            EnsureActive();
            SetName(name);
        }

        public void ChangeCode(string code)
        {
            EnsureActive();
            SetCode(code);
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException(
                    "Unit of measure is already active.");

            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException(
                    "Unit of measure is already inactive.");

            IsActive = false;
        }

        private void SetCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException(
                    "Unit code is required.");

            code = code.Trim().ToUpperInvariant();

            if (code.Length > 20)
                throw new DomainException(
                    "Unit code cannot exceed 20 characters.");

            Code = code;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(
                    "Unit name is required.");

            name = name.Trim();

            if (name.Length > 100)
                throw new DomainException(
                    "Unit name cannot exceed 100 characters.");

            Name = name;
        }

        private void SetUnitType(UnitType unitType)
        {
            if (!Enum.IsDefined(typeof(UnitType), unitType))
                throw new DomainException(
                    "Invalid unit type.");

            UnitType = unitType;
        }

        private void EnsureActive()
        {
            if (!IsActive)
                throw new DomainException(
                    "Inactive units cannot be modified.");
        }
    }
}
