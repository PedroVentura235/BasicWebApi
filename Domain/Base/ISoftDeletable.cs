namespace Domain.Base;

public interface ISoftDeletable
{
    DateTime? DeletedOn { get; set; }
    Guid? DeletedBy { get; set; }
}