namespace Domain.Base;

public class AuditableEntity<TId> : BaseEntity<TId>, IAuditable, ISoftDeletable
{
    public DateTime Created { get; private set; } = DateTime.Now;

    public Guid CreatedBy { get; set; }

    public DateTime? LastModified { get; set; } = DateTime.Now;
    public Guid? LastModifiedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
}