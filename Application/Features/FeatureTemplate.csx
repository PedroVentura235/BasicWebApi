using CodegenCS.IO;
public class FeatureTemplate
{
    internal class Model
    {
        public string Name { get; set; }
        public string[] Properties { get; set; }
    }
    void Main(ICodegenContext context)
    {
        var entities = new
        {
            Tables = new Dictionary<string, List<Tuple<string, string>>>()
            {
                {
                    "Product",
                    new List<Tuple<string, string>>()
                    {
                        Tuple.Create("Id", "Guid"),
                        Tuple.Create("Name", "string"),
                        Tuple.Create("Description", "string"),
                        Tuple.Create("Price", "decimal")
                    }
                }
            }
        };

        foreach (var table in entities.Tables)
        {
            string properties = "";
            string idProperty = "";
            string idType = "";
            string create = $$"""
                {{table.Key}} entity = new(
                """;
            string update = "";
            string import = "";
            int totalProperties = table.Value.Count();
            int currentCount = 0;
            List<Tuple<string, string>> list = table.Value;
            foreach (var item in list)
            {
                currentCount++;
                if (item.Item1 != "Id")
                {
                    properties += $$"""
                    public {{item.Item2}} {{item.Item1}} { get; set; }

                    """;
                    if (currentCount == totalProperties)
                    {
                        create += $$"""request.{{item.Item1}});""";
                        update += $$"""request.{{item.Item1}});""";
                    }
                    else
                    {
                        create += $$"""request.{{item.Item1}}, """;
                        update += $$"""request.{{item.Item1}}, """;
                    }
                }
                else
                {
                    idProperty = $$"""
                    public {{item.Item2}} {{item.Item1}} { get; set; }

                    """;
                    idType = item.Item2;
                }
            }
            import = create.Replace("request", "dto");

            var folderCreate = $$"""{{table.Key}}s\\Commands\\Generated\\Create{{table.Key}}Request.cs""";
            var createContext = context[folderCreate];
            createContext.WriteLine(GenerateCreateRequest(table.Key, properties, create, idType));

            var folderImport = $$"""{{table.Key}}s\\Commands\\Generated\\Import{{table.Key}}Request.cs""";
            var importContext = context[folderImport];
            importContext.WriteLine(GenerateImportRequest(table.Key, import));

            var folderUpdate = $$"""{{table.Key}}s\\Commands\\Generated\\Update{{table.Key}}Request.cs""";
            var updateContext = context[folderUpdate];
            updateContext.WriteLine(GenerateUpdateRequest(table.Key, properties, idProperty, update, idType));

            var folderDto = $$"""{{table.Key}}s\\Generated\\{{table.Key}}Dto.cs""";
            //var folder = $"{table}s\\Generated";
            var dtoContext = context[folderDto];
            dtoContext.WriteLine(GenerateDto(table.Key, properties, idProperty));

            var folderGet = $$"""{{table.Key}}s\\Queries\\Generated\\Get{{table.Key}}Request.cs""";
            //var folder = $"{table}s\\Generated";
            var getContext = context[folderGet];
            getContext.WriteLine(GenerateGetRequest(table.Key));
        }
    }

    FormattableString GenerateCreateRequest(string tableName, string list, string create, string idType)
    {
        return $$"""
            //Auto generated Code
            using BasicWebApi.Application.Common.Response;
            using MediatR;
            using BasicWebApi.Application.Persistence;
            using BasicWebApi.Domain.Entities;

            namespace BasicWebApi.Application.Features.{{tableName}}s;

            public partial class Create{{tableName}}Request : IRequest<Result<{{idType}}, Error>>
            {
                {{list}}
            }

            public partial class Create{{tableName}}RequestHandler : IRequestHandler<Create{{tableName}}Request, Result<{{idType}}, Error>>
            {
                // Add Domain Events automatically by using IRepositoryWithEvents
                private readonly IRepository<{{tableName}}> _repository;

                public Create{{tableName}}RequestHandler(IRepository<{{tableName}}> repository) => _repository = repository;

                public async Task<Result<{{idType}}, Error>> Handle(Create{{tableName}}Request request, CancellationToken cancellationToken)
                {
                    {{create}}
                    await _repository.AddAsync(entity, cancellationToken);
                    return entity.Id;
                }
            }
            """;
    }

    FormattableString GenerateImportRequest(string tableName, string create)
    {
        return $$"""
            //Auto generated Code
            using BasicWebApi.Application.Common.Response;
            using MediatR;
            using BasicWebApi.Application.Persistence;
            using BasicWebApi.Domain.Entities;
            using BasicWebApi.Application.Common.Interfaces;

            namespace BasicWebApi.Application.Features.{{tableName}}s;

            public partial class Import{{tableName}}Request : IRequest<Result<List<{{tableName}}Dto>, Error>>
            {
                public string FilePath { get; set; }

                public Import{{tableName}}Request(string filePath)
                {
                    FilePath = filePath;
                }
            }

            public partial class Import{{tableName}}RequestHandler : IRequestHandler<Import{{tableName}}Request, Result<List<{{tableName}}Dto>, Error>>
            {
                // Add Domain Events automatically by using IRepositoryWithEvents
                private readonly IRepository<{{tableName}}> _repository;
                private readonly IExcelHelper _excelHelper;

                public Import{{tableName}}RequestHandler(IRepository<{{tableName}}> repository, IExcelHelper excelHelper)
                {
                    _repository = repository;
                    _excelHelper = excelHelper;
                }

                public async Task<Result<List<{{tableName}}Dto>, Error>> Handle(Import{{tableName}}Request request, CancellationToken cancellationToken)
                {
                    List<{{tableName}}Dto> listDto = _excelHelper.Import<{{tableName}}Dto>(request.FilePath);
                    List<{{tableName}}> entities = new();

                    foreach (var dto in listDto)
                    {
                        {{create}}
                        entities.Add(entity);
                    }
                    await _repository.AddRangeAsync(entities, cancellationToken);
                    return listDto;
                }
            }
            """;
    }

    FormattableString GenerateDto(string tableName, string list, string idProperty)
    {
        return $$"""
            //Auto generated Code

            namespace BasicWebApi.Application.Features.{{tableName}}s;
            public partial class {{tableName}}Dto
            {
                {{idProperty}}
                {{list}}
            }

            """;
    }

    FormattableString GenerateGetRequest(string tableName)
    {
        return $$"""
            //Auto generated Code
            using MediatR;
            using BasicWebApi.Application.Features.{{tableName}}s;
            using BasicWebApi.Application.Persistence;
            using BasicWebApi.Domain.Entities;
            using Mapster;
            using BasicWebApi.Application.Common.Response;

            public class Get{{tableName}}sRequest : IRequest<Result<List<{{tableName}}Dto>, Error>>
            {
            }

            public class Get{{tableName}}sRequestHandler : IRequestHandler<Get{{tableName}}sRequest, Result<List<{{tableName}}Dto>, Error>>
            {
                private IReadRepository<{{tableName}}> _repository;

                public Get{{tableName}}sRequestHandler(IReadRepository<{{tableName}}> repository)
                {
                    _repository = repository;
                }

                public async Task<Result<List<{{tableName}}Dto>, Error>> Handle(Get{{tableName}}sRequest request, CancellationToken cancellationToken)
                {
                    IEnumerable<{{tableName}}> entities = await _repository.ListAsync(cancellationToken);
                    List<{{tableName}}Dto> response = entities.Adapt<List<{{tableName}}Dto>>();
                    return response;
                }
            }
            """;
    }

    FormattableString GenerateUpdateRequest(string tableName, string list, string idProperty, string update, string idType)
    {
        return $$"""
            //Auto generated Code
            using MediatR;
            using BasicWebApi.Application.Persistence;
            using BasicWebApi.Domain.Entities;
            using BasicWebApi.Application.Common.Response;

            public class Update{{tableName}}Request : IRequest<Result<{{idType}}, Error>>
            {
                {{idProperty}}
                {{list}}
            }

            public class Update{{tableName}}RequestHandler : IRequestHandler<Update{{tableName}}Request, Result<{{idType}}, Error>>
            {
                // Add Domain Events automatically by using IRepositoryWithEvents
                private readonly IRepository<{{tableName}}> _repository;

                public Update{{tableName}}RequestHandler(IRepository<{{tableName}}> repository) =>
                    (_repository) = (repository);

                public async Task<Result<{{idType}}, Error>> Handle(Update{{tableName}}Request request, CancellationToken cancellationToken)
                {
                    var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

                    if(entity is null)
                        return new Error(System.Net.HttpStatusCode.NotFound, "{{tableName}} not found");

                    entity.Update({{update}}

                    await _repository.UpdateAsync(entity, cancellationToken);

                    return entity.Id;
                }
            }
            """;
    }
}