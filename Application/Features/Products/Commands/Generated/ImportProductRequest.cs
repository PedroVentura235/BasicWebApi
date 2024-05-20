//Auto generated Code
using BasicWebApi.Application.Common.Response;
using MediatR;
using BasicWebApi.Application.Persistence;
using BasicWebApi.Domain.Entities;
using BasicWebApi.Application.Common.Interfaces;

namespace BasicWebApi.Application.Features.Products;

public partial class ImportProductRequest : IRequest<Result<List<ProductDto>, Error>>
{
    public string FilePath { get; set; }

    public ImportProductRequest(string filePath)
    {
        FilePath = filePath;
    }
}

public partial class ImportProductRequestHandler : IRequestHandler<ImportProductRequest, Result<List<ProductDto>, Error>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepository<Product> _repository;
    private readonly IExcelHelper _excelHelper;

    public ImportProductRequestHandler(IRepository<Product> repository, IExcelHelper excelHelper)
    {
        _repository = repository;
        _excelHelper = excelHelper;
    }

    public async Task<Result<List<ProductDto>, Error>> Handle(ImportProductRequest request, CancellationToken cancellationToken)
    {
        List<ProductDto> listDto = _excelHelper.Import<ProductDto>(request.FilePath);
        List<Product> entities = new();

        foreach (var dto in listDto)
        {
            Product entity = new(dto.Name, dto.Description, dto.Price);
            entities.Add(entity);
        }
        await _repository.AddRangeAsync(entities, cancellationToken);
        return listDto;
    }
}
