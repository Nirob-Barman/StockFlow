namespace CleanArchitecture.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; private set; }

        public Guid? CreatedBy { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public Guid? UpdatedBy { get; private set; }

        protected AuditableEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public void SetCreatedBy(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID.");

            CreatedBy = userId;
        }

        public void SetUpdatedBy(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID.");

            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId;
        }
    }
}
