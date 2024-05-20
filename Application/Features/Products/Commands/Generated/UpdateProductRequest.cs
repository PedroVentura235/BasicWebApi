//Auto generated Code
using MediatR;
using BasicWebApi.Application.Persistence;
using BasicWebApi.Domain.Entities;
using BasicWebApi.Application.Common.Response;

public class UpdateProductRequest : IRequest<Result<Guid, Error>>
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

}

public class UpdateProductRequestHandler : IRequestHandler<UpdateProductRequest, Result<Guid, Error>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepository<Product> _repository;

    public UpdateProductRequestHandler(IRepository<Product> repository) =>
        (_repository) = (repository);

    public async Task<Result<Guid, Error>> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if(entity is null)
            return new Error(System.Net.HttpStatusCode.NotFound, "Product not found");

        entity.Update(request.Name, request.Description, request.Price);

        await _repository.UpdateAsync(entity, cancellationToken);

        return entity.Id;
    }
}
