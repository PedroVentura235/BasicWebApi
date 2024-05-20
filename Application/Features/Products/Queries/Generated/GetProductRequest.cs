//Auto generated Code
using MediatR;
using BasicWebApi.Application.Features.Products;
using BasicWebApi.Application.Persistence;
using BasicWebApi.Domain.Entities;
using Mapster;
using BasicWebApi.Application.Common.Response;

public class GetProductsRequest : IRequest<Result<List<ProductDto>, Error>>
{
}

public class GetProductsRequestHandler : IRequestHandler<GetProductsRequest, Result<List<ProductDto>, Error>>
{
    private IReadRepository<Product> _repository;

    public GetProductsRequestHandler(IReadRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<ProductDto>, Error>> Handle(GetProductsRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<Product> entities = await _repository.ListAsync(cancellationToken);
        List<ProductDto> response = entities.Adapt<List<ProductDto>>();
        return response;
    }
}