namespace Domain.Base;

public interface IAuditable
{
    DateTime Created { get; }
    Guid CreatedBy { get; }
    DateTime? LastModified { get; }
    Guid? LastModifiedBy { get; }
}