namespace Infrastructure.Persistence;
internal interface IDatabaseInitializer
{
    Task InitializeDatabasesAsync(CancellationToken cancellationToken);
    Task InitializeApplicationDbForTenantAsync(CancellationToken cancellationToken);
}
