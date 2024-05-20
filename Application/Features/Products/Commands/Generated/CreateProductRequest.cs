//Auto generated Code
using BasicWebApi.Application.Common.Response;
using MediatR;
using BasicWebApi.Application.Persistence;
using BasicWebApi.Domain.Entities;

namespace BasicWebApi.Application.Features.Products;

public partial class CreateProductRequest : IRequest<Result<Guid, Error>>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

}

public partial class CreateProductRequestHandler : IRequestHandler<CreateProductRequest, Result<Guid, Error>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepository<Product> _repository;

    public CreateProductRequestHandler(IRepository<Product> repository) => _repository = repository;

    public async Task<Result<Guid, Error>> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        Product entity = new(request.Name, request.Description, request.Price);
        await _repository.AddAsync(entity, cancellationToken);
        return entity.Id;
    }
}
